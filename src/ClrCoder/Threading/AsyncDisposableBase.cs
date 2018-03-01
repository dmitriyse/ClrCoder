// <copyright file="AsyncDisposableBase.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Standard implementation of <see cref="IAsyncDisposableEx"/> pattern suited for basing.
    /// </summary>
    /// <remarks>
    /// TODO: Add async-initializable optional feature.
    /// </remarks>
    [PublicAPI]
    public abstract class AsyncDisposableBase : IAsyncDisposableEx
    {
        /// <summary>
        /// TODO: Replace with CancellationTokenSource.
        /// </summary>
        [CanBeNull]
        private TaskCompletionSource<ValueVoid> _disposeCompletionSource;

        [CanBeNull]
        private Task _disposeTask;

        private bool _isDisposeStarted;

        private bool _isDisposeSuspended;

        private bool _isActualDisposeCalled;

        private int _usageCounter;

        [CanBeNull]
        private Exception _lastError;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDisposableBase"/> class with synchronization root object.
        /// </summary>
        /// <param name="disposeSyncRoot">Synchronization root.</param>
        protected AsyncDisposableBase([CanBeNull]object disposeSyncRoot = null)
        {
            DisposeSyncRoot = disposeSyncRoot ?? new object();
        }

        /// <inheritdoc/>
        [DebuggerHidden]
        public Task Disposed
        {
            get
            {
                if (Volatile.Read(ref _disposeTask) == null)
                {
                    lock (DisposeSyncRoot)
                    {
                        // Implicit memory barrier here
                        if (_disposeTask == null)
                        {
                            _disposeCompletionSource = new TaskCompletionSource<ValueVoid>();
                            _disposeTask = _disposeCompletionSource.Task;
                        }

                        // Implicit memory barrier here
                    }
                }

                return _disposeTask;
            }
        }

        /// <summary>
        /// Shows that dispose was started. Monitor <see cref="Disposed"/> to know dispose status.
        /// </summary>
        /// <remarks>
        /// This value can be outdated, but never returns true when object fully switched to disposing state.
        /// </remarks>
        public bool IsDisposeStarted => Volatile.Read(ref _isDisposeStarted);

        /// <summary>
        /// Dispose synchronization root.
        /// TODO: Rename to LifetimeSyncRoot
        /// </summary>
        public object DisposeSyncRoot { get; }

        /// <inheritdoc/>
        public Task DisposeAsync()
        {
            // !!!! This method is reenterant.
            if (!Volatile.Read(ref _isDisposeStarted))
            {
                // But here we are free of reentrancy.
                lock (DisposeSyncRoot)
                {
                    // -------Implicit memory barrier here-----------
                    if (!_isDisposeStarted)
                    {
                        _isDisposeStarted = true;

                        // Reentrancy source #1.
                        try
                        {
                            OnDisposeStarted();
                        }
                        catch (Exception ex)
                        {
                            // Caching even not processable exception.
                            // Last error will be thrown at the end if the dispose process.
                            _lastError = ex;
                        }

                        if (!_isDisposeSuspended && !_isActualDisposeCalled)
                        {
                            StartAsyncDisposeWrapped();
                        }
                    }

                    // -------Implicit memory barrier here-----------
                }
            }

            Task disposed = Disposed;

            Debug.Assert(disposed != null, "disposed != null");

            return disposed;
        }

        /// <summary>
        /// Decreases usage counter. Dispose only allowed when usage counter is zero.
        /// </summary>
        protected void DecreaseUsageCounter()
        {
            lock (DisposeSyncRoot)
            {
                if (_usageCounter == 0)
                {
                    throw new InvalidOperationException(
                        "You cannot decrease usage counter because it's already equals to zero.");
                }

                if (--_usageCounter == 0)
                {
                    SetDisposeSuspended(false);
                }
            }
        }

        /// <summary>
        /// Performs actual dispose work. This method is called only once.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        protected abstract Task DisposeAsyncCore();

        /// <summary>
        /// Increases usage counter. Dispose only allowed when usage counter is zero.
        /// </summary>
        protected void IncreaseUsageCounter()
        {
            lock (DisposeSyncRoot)
            {
                if (_usageCounter++ == 0)
                {
                    SetDisposeSuspended(true);
                }
            }
        }

        /// <summary>
        /// Allows to handle dispose started event.
        /// This method is called under <c>lock</c> of SyncRoot.
        /// </summary>
        /// <remarks>
        /// This method is called before call to <see cref="DisposeAsyncCore"/>.
        /// </remarks>
        protected virtual void OnDisposeStarted()
        {
            // We are not reenterant here.
            // Do nothing.
        }

        /// <summary>
        /// Fancy way to control usages with c# using operator.
        /// </summary>
        /// <returns>The dispose token.</returns>
        protected UsageDecrementToken RegisterUsage()
        {
            IncreaseUsageCounter();
            return new UsageDecrementToken(this);
        }

        /// <summary>
        /// Allow to suspend actual call to <see cref="DisposeAsyncCore"/>. If StartDispose called while in suspended state, actual
        /// dispose process will not begins untl suspend turn off. It' is impossible to turn on suspend state if DisposeAsyncCore
        /// was
        /// already called.
        /// </summary>
        /// <remarks>This method can be called as many times as required until actual dispose was started.</remarks>
        /// <param name="isDisposeSuspended">true to suspend actual dispose, false otherwise.</param>
        /// <exception cref="InvalidOperationException">
        /// It's not allowed to enter suspend state while actual dispose was already
        /// started.
        /// </exception>
        protected void SetDisposeSuspended(bool isDisposeSuspended)
        {
            // This method is reentrant.
            // ----------------------------------
            lock (DisposeSyncRoot)
            {
                if (_usageCounter != 0)
                {
                    throw new InvalidOperationException(
                        "You cannot directly control suspended state if usage counter is non zero.");
                }

                // -------Implicit memory barrier here-----------
                if (isDisposeSuspended && _isActualDisposeCalled)
                {
                    throw new InvalidOperationException("Cannot suspend already started dispose process.");
                }

                if (_isDisposeSuspended != isDisposeSuspended)
                {
                    // Here we are free from reentrancy.
                    _isDisposeSuspended = isDisposeSuspended;
                    if (_isDisposeStarted && !_isDisposeSuspended && !_isActualDisposeCalled)
                    {
                        // Reentrancy point.
                        StartAsyncDisposeWrapped();
                    }
                }

                // -------Implicit memory barrier here-----------
            }
        }

        private void HandleDisposeResult(Task disposeResult)
        {
            lock (DisposeSyncRoot)
            {
                Exception error = null;

                if (disposeResult.IsFaulted || disposeResult.IsCanceled)
                {
                    try
                    {
                        disposeResult.GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        error =
                            ex.IsProcessable() ? new CriticalException("Async dispose error.", ex) : ex;
                    }
                }

                if (error == null)
                {
                    error = _lastError;
                }

                if (error == null)
                {
                    if (_disposeTask == null)
                    {
                        _disposeTask = TaskEx.CompletedTaskValue;
                    }
                    else
                    {
                        _disposeCompletionSource?.SetResult(default);
                    }
                }
                else
                {
                    if (_disposeTask == null)
                    {
                        _disposeTask = TaskEx.FromException(error);
                    }
                    else
                    {
                        _disposeCompletionSource?.SetException(error);
                    }
                }
            }
        }

        private void StartAsyncDisposeWrapped()
        {
            Debug.Assert(!_isActualDisposeCalled, "Actual dispose can be performed only once.");

            _isActualDisposeCalled = true;

            Task disposeResult;
            try
            {
                disposeResult = DisposeAsyncCore().EnsureStarted();
            }
            catch (Exception ex)
            {
                disposeResult = TaskEx.FromException(ex);
            }

            if (disposeResult.IsCompleted)
            {
                Debug.Assert(
                    (_disposeTask == null)
                    || ((_disposeCompletionSource != null) && (_disposeTask == _disposeCompletionSource.Task)),
                    "Dispose task should be null here, except when completion source was created.");

                HandleDisposeResult(disposeResult);
            }
            else
            {
                Task task = disposeResult.ContinueWith(HandleDisposeResult);
                if (_disposeTask == null)
                {
                    _disposeTask = task;
                }
            }
        }

        /// <summary>
        /// Dispose token that is helpful to decrease usage counter.
        /// </summary>
        public struct UsageDecrementToken : IDisposable
        {
            private readonly AsyncDisposableBase _owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="UsageDecrementToken"/> struct.
            /// </summary>
            /// <param name="owner">The owner component.</param>
            internal UsageDecrementToken(AsyncDisposableBase owner)
            {
                _owner = owner;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                _owner.DecreaseUsageCounter();
            }
        }
    }
}