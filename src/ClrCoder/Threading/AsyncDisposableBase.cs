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
    /// Standard implementation of <see cref="IAsyncDisposable"/> pattern suited for basing.
    /// </summary>
    /// <remarks>
    /// TODO: Add async-initializable optional feature.
    /// </remarks>
    [PublicAPI]
    public abstract class AsyncDisposableBase : IAsyncDisposable
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

        [CanBeNull]
        private Exception _lastError;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDisposableBase"/> class.
        /// </summary>
        protected AsyncDisposableBase()
            : this(new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDisposableBase"/> class with synchronization root object.
        /// </summary>
        /// <param name="disposeSyncRoot">Synchronization root.</param>
        protected AsyncDisposableBase(object disposeSyncRoot)
        {
            DisposeSyncRoot = disposeSyncRoot;
        }

        /// <inheritdoc/>
        [DebuggerHidden]
        public Task DisposeTask
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
        /// Shows that dispose was started. Monitor <see cref="DisposeTask"/> to know dispose status.
        /// </summary>
        /// <remarks>
        /// This value can be outdated, but never returns true when object fully switched to disposing state.
        /// </remarks>
        public bool IsDisposeStarted => Volatile.Read(ref _isDisposeStarted);

        /// <summary>
        /// Dispose synchronization root.
        /// </summary>
        public object DisposeSyncRoot { get; }

        /// <inheritdoc/>
        public void StartDispose()
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
        }

        /// <summary>
        /// Performs actual dispose work. This method is called only once.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        protected abstract Task AsyncDispose();

        /// <summary>
        /// Allows to handle dispose started event.
        /// This method is called under <c>lock</c> of SyncRoot.
        /// </summary>
        /// <remarks>
        /// This method is called before call to <see cref="AsyncDispose"/>.
        /// </remarks>
        protected virtual void OnDisposeStarted()
        {
            // We are not reenterant here.
            // Do nothing.
        }

        /// <summary>
        /// Allow to suspend actual call to <see cref="AsyncDispose"/>. If StartDispose called while in suspended state, actual
        /// dispose process will not begins untl suspend turn off. It' is impossible to turn on suspend state if AsyncDispose was
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

        [SuppressMessage(
            "StyleCop.CSharp.MaintainabilityRules",
            "SA1408:ConditionalExpressionsMustDeclarePrecedence",
            Justification = "Reviewed. Suppression is OK here.")]
        private async Task AsynDisposeWrapped()
        {
            // We are under lock here.
            var isAsyncRun = false;

            // This line will never throw any exception.
            Task disposeResult = AsyncDispose();

            try
            {
                await disposeResult.WithSyncDetection(isSync => isAsyncRun = !isSync);

                if (_lastError != null)
                {
                    throw _lastError;
                }

                if (isAsyncRun)
                {
                    // We are out of lock (not luck :) ) here.
                    lock (DisposeSyncRoot)
                    {
                        // -------Implicit memory barrier here-----------
                        Debug.Assert(
                            _disposeTask != null,
                            "Dispose task initialized here to completion source task or to DoAsyncDispose wrapper result Task");

                        // If completion source was used prior to StartDispose method we should finalize completion source.
                        _disposeCompletionSource?.SetResult(default(ValueVoid));

                        // -------Implicit memory barrier here-----------
                    }
                }
                else
                {
                    // We are still under lock here.
                    Debug.Assert(
                        _disposeTask == null
                        || _disposeCompletionSource != null && _disposeTask == _disposeCompletionSource.Task,
                        "Dispose task should be null here, except when completion source was created.");
                    _disposeCompletionSource?.SetResult(default(ValueVoid));
                }
            }
            catch (Exception ex)
            {
                Exception finalError = ex.IsProcessable() ? new CriticalException("Async dispose error.", ex) : ex;

                if (isAsyncRun)
                {
                    // We are out of lock here.
                    lock (DisposeSyncRoot)
                    {
                        // -------Implicit memory barrier here-----------
                        Debug.Assert(
                            _disposeTask != null,
                            "Dispose task initialized here to completion source task or to DoAsyncDispose wrapper result Task");

                        if (_disposeCompletionSource == null)
                        {
                            // This exception will be wrapped into task.
                            throw finalError;
                        }

                        _disposeCompletionSource.SetException(finalError);

                        // -------Implicit memory barrier here-----------
                    }
                }
                else
                {
                    // We are still under lock here.
                    Debug.Assert(
                        _disposeTask == null
                        || _disposeCompletionSource != null && _disposeTask == _disposeCompletionSource.Task,
                        "Dispose task should be null here, except when completion source was created.");

                    if (_disposeCompletionSource == null)
                    {
                        // This exception will be wrapped into task !!! even in a synchronous execution path !!!.
                        throw finalError;
                    }

                    _disposeCompletionSource.SetException(finalError);
                }
            }
        }

        private void StartAsyncDisposeWrapped()
        {
            Debug.Assert(!_isActualDisposeCalled, "Actual dispose can be performed only once.");

            _isActualDisposeCalled = true;

            // When _disposeTask was not asked, we are free to set it to result of DoAsyncDispose wrapper.
            Task task = AsynDisposeWrapped().EnsureStarted();

            if (_disposeTask == null)
            {
                Debug.Assert(
                    _disposeCompletionSource == null,
                    "When dispose task not set, completion source also should be uninitialized");

                _disposeTask = task;
            }
            else
            {
                Debug.Assert(
                    _disposeCompletionSource != null && _disposeTask == _disposeCompletionSource.Task,
                    "When disposeTask was set before first call to StartDispose, it should be completion source task.");
            }
        }
    }
}