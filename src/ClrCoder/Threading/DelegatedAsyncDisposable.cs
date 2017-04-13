// <copyright file="DelegatedAsyncDisposable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Simple implementation of the <see cref="AsyncDisposableBase"/> that delegates dispose operation to an action.
    /// </summary>
    public class DelegatedAsyncDisposable : AsyncDisposableBase
    {
        private readonly Func<Task> _disposeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatedAsyncDisposable"/> class.
        /// </summary>
        /// <param name="disposeAction">The action that will be called on dispose.</param>
        public DelegatedAsyncDisposable(Func<Task> disposeAction)
        {
            if (disposeAction == null)
            {
                throw new ArgumentNullException(nameof(disposeAction));
            }

            _disposeAction = disposeAction;
        }

        /// <inheritdoc/>
        protected override Task AsyncDispose()
        {
            return _disposeAction();
        }
    }
}