// <copyright file="AsyncComponent.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The async component.
    /// </summary>
    public abstract class AsyncComponent : AsyncDisposableBase
    {
#pragma warning disable 1998

        private readonly CancellationTokenSource _processingCts;

        private int _asyncOperationsCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncComponent"/> class.
        /// </summary>
        protected AsyncComponent()
        {
            _processingCts = new CancellationTokenSource();
            ProcessingCancellationToken = _processingCts.Token;
        }

        protected CancellationToken ProcessingCancellationToken { get; }

        protected override async Task DisposeAsyncCore()
        {
            _processingCts.Dispose();
        }

        /// <summary>
        /// Starts an operation. Component dispose suspended if at least one operation is opened.
        /// </summary>
        /// <returns>The operation finalize token.</returns>
        protected IDisposable StartOperation()
        {
            StartOperationInternal();

            return new AsyncOperationDisposeToken(this);
        }

        /// <summary>
        /// Starts new cancellable operation. This operation have it's own cancellation token source.
        /// </summary>
        /// <returns>The cancellable operation.</returns>
        protected CancellableAsyncComponentOperation StartCancellableOperation()
        {
            StartOperationInternal();

            return new CancellableAsyncComponentOperation(EndAsyncOperation);
        }

        /// <summary>
        /// Starts new cancellable operation. This operation have it's own cancellation token source.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can cancel this operation.</param>
        /// <returns>The cancellable operation.</returns>
        protected CancellableAsyncComponentOperation StartCancellableOperation(CancellationToken cancellationToken)
        {
            StartOperationInternal();

            return new CancellableAsyncComponentOperation(EndAsyncOperation, cancellationToken);
        }

        /// <summary>
        /// Starts new cancellable operation. This operation have it's own cancellation token source.
        /// </summary>
        /// <param name="cancellationTokens">Cancellation tokens that can cancel this operation.</param>
        /// <exception cref="ComponentDisposedException">Component disposed.</exception>
        /// <returns>The cancellable operation.</returns>
        protected CancellableAsyncComponentOperation StartCancellableOperation(
            params CancellationToken[] cancellationTokens)
        {
            StartOperationInternal();

            return new CancellableAsyncComponentOperation(EndAsyncOperation, cancellationTokens);
        }

        private void StartOperationInternal()
        {
            try
            {
                Interlocked.Increment(ref _asyncOperationsCount);
                SetDisposeSuspended(true);
            }
            catch (InvalidOperationException)
            {
                Interlocked.Decrement(ref _asyncOperationsCount);
                throw new ComponentDisposedException();
            }
        }

        private void EndAsyncOperation()
        {
            if (Interlocked.Decrement(ref _asyncOperationsCount) == 0)
            {
                SetDisposeSuspended(false);
            }
        }

        /// <summary>
        /// Verifies that component not in the "disposing" or "disposed" state.
        /// </summary>
        /// <exception cref="ComponentDisposedException">Component disposed.</exception>
        protected void VerifyComponentAlive()
        {
            if (ProcessingCancellationToken.IsCancellationRequested)
            {
                throw new ComponentDisposedException();
            }
        }

        private class AsyncOperationDisposeToken : IDisposable
        {
            private readonly AsyncComponent _owner;

            public AsyncOperationDisposeToken(AsyncComponent owner)
            {
                _owner = owner;
            }

            public void Dispose()
            {
                _owner.EndAsyncOperation();
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposeStarted()
        {
            base.OnDisposeStarted();

            _processingCts.Cancel();
        }
    }
}