// <copyright file="AwaitableEvent.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;

    using JetBrains.Annotations;

    /// <summary>
    /// ManualResetEvent like synchronization primitive but optimized for async/await functions.
    /// </summary>
    /// <remarks>
    /// Current implementation based on <see cref="CancellationToken"/>.
    /// </remarks>
    [PublicAPI]
    public class AwaitableEvent : IDisposable
    {
        private readonly CancellationTokenSource _cts;

        private CancellationToken _cancellationToken;

        private bool _isSuspended;

        private bool _isSet;

        private bool _actualCancellationRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="AwaitableEvent"/> class.
        /// </summary>
        public AwaitableEvent()
        {
            _cts = new CancellationTokenSource();
            _cancellationToken = _cts.Token;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="AwaitableEvent"/> class.
        /// </summary>
        ~AwaitableEvent()
        {
            ReleaseUnmanagedResources();
        }

        /// <summary>
        /// Identifies that event is set. But even Event is set trigger can be suspended.
        /// </summary>
        public bool IsSet => Volatile.Read(ref _isSet);

        /// <inheritdoc/>
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Allows "await" syntax.
        /// </summary>
        /// <returns>Event awaiter.</returns>
        public AwaitableEventAwaiter GetAwaiter()
        {
            return new AwaitableEventAwaiter(this);
        }

        /// <summary>
        /// Turns event into "Set" state. Triggering can be suspended with <see cref="SuspendTrigger"/> method.
        /// </summary>
        public void Set()
        {
            // This method is reenterant.
            // ---------------------------------
            var doCancel = false;
            lock (_cts)
            {
                if (_isSet)
                {
                    return;
                }

                _isSet = true;
                if (!_isSuspended && !_actualCancellationRequested)
                {
                    doCancel = true;
                    _actualCancellationRequested = true;
                }
            }

            if (doCancel)
            {
                // Reenterance can occur here.
                _cts.Cancel();
            }
        }

        /// <summary>
        /// Suspends event triggering. Once event becomes set and suspend mode turns off, triggering occurs.
        /// </summary>
        /// <param name="isSuspended">New suspend mode. After event triggered you cannot set suspend mode.</param>
        public void SuspendTrigger(bool isSuspended)
        {
            var doCancel = false;

            // This method is reenterant.
            // --------------------------------
            lock (_cts)
            {
                if (_actualCancellationRequested && isSuspended)
                {
                    throw new InvalidOperationException("You cannot suspend already triggered event.");
                }

                if (isSuspended == _isSuspended)
                {
                    return;
                }

                _isSuspended = isSuspended;
                if (!isSuspended && IsSet && !_actualCancellationRequested)
                {
                    doCancel = true;
                    _actualCancellationRequested = true;
                }
            }

            if (doCancel)
            {
                // Await methods can be raised synchronously here.
                // So this method can produce reenterance.
                _cts.Cancel();
            }
        }

        private void ReleaseUnmanagedResources()
        {
            _cts.Dispose();
        }

        /// <summary>
        /// Awaiter for <see cref="AwaitableEvent"/>.
        /// </summary>
        public struct AwaitableEventAwaiter : INotifyCompletion
        {
            private readonly AwaitableEvent _awaitableEvent;

            /// <summary>
            /// Initializes a new instance of the <see cref="AwaitableEventAwaiter"/> struct.
            /// </summary>
            /// <param name="awaitableEvent">Event that owns this awaiter.</param>
            internal AwaitableEventAwaiter(AwaitableEvent awaitableEvent)
            {
                _awaitableEvent = awaitableEvent;
            }

            private void EnsureNotDefault()
            {
                if (_awaitableEvent == null)
                {
                    throw new InvalidOperationException("Cannot use default(AwaitableEvent.Awaiter).");
                }
            }

            /// <summary>
            /// Registers <c>continuation</c> <c>action</c>..
            /// </summary>
            /// <param name="continuation">Operation <c>continuation</c>.</param>
            [UsedImplicitly]
            public void OnCompleted([NotNull] Action continuation)
            {
                if (continuation == null)
                {
                    throw new ArgumentNullException(nameof(continuation));
                }

                EnsureNotDefault();

                _awaitableEvent._cancellationToken.Register(continuation);
            }

            /// <summary>
            /// Gets result - none in our case.
            /// </summary>
            [UsedImplicitly]
            public void GetResult()
            {
                EnsureNotDefault();
            }

            /// <summary>
            /// Shows that awaitable operation finished.
            /// </summary>
            [UsedImplicitly]
            public bool IsCompleted
            {
                get
                {
                    EnsureNotDefault();

                    return _awaitableEvent._cancellationToken.IsCancellationRequested;
                }
            }
        }
    }
}