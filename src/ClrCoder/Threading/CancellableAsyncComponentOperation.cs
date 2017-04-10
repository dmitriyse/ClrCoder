// <copyright file="CancellableAsyncComponentOperation.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Linq;
    using System.Threading;

    using JetBrains.Annotations;

    using MoreLinq;

    /// <summary>
    /// Cancellable operation in an instance of the <see cref="AsyncComponent"/>.
    /// </summary>
    /// <remarks>
    /// TODO: Optimize me.
    /// </remarks>
    public class CancellableAsyncComponentOperation : IDisposable
    {
        private readonly Action _disposeAction;

        [CanBeNull]
        private readonly CancellationTokenSource _selfCts;

        internal CancellableAsyncComponentOperation(Action disposeAction)
        {
            _disposeAction = disposeAction;
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
        }

        internal CancellableAsyncComponentOperation(
            Action disposeAction,
            CancellationToken cancellationToken)
        {
            _disposeAction = disposeAction;
            _selfCts = new CancellationTokenSource();
            CancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(_selfCts.Token, cancellationToken);
            CancellationToken = CancellationTokenSource.Token;
        }

        internal CancellableAsyncComponentOperation(
            Action disposeAction,
            params CancellationToken[] tokens)
        {
            _disposeAction = disposeAction;
            _selfCts = new CancellationTokenSource();
            CancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(
                    _selfCts.Token.Concat(tokens).ToArray());
            CancellationToken = CancellationTokenSource.Token;
        }

        /// <summary>
        /// The cancellation token of the current operation.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// The cancellation token source of this operation.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            try
            {
                CancellationTokenSource.Dispose();
                _selfCts?.Dispose();
            }
            catch
            {
                // Do nothing.
            }

            _disposeAction();
        }
    }
}