// <copyright file="AsyncLocked.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ObjectModel
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Threading;

    /// <summary>
    /// Lock on some <c>object</c> that releases asynchronously.
    /// </summary>
    /// <typeparam name="T">Type of <c>object</c>.</typeparam>
    public class AsyncLocked<T> : AsyncDisposableBase, IAsyncLocked<T>
    {
        [CanBeNull]
        private readonly Func<Task> _disposeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLocked{T}"/> class.
        /// </summary>
        /// <param name="target"> The target object that will be locked. </param>
        /// <param name="disposeAction"> The dispose action. </param>
        public AsyncLocked(T target, [CanBeNull] Func<Task> disposeAction = null)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            _disposeAction = disposeAction;

            Target = target;
        }

        /// <inheritdoc/>
        public T Target { get; private set; }

        /// <inheritdoc/>
        protected override async Task DisposeAsyncCore()
        {
            if (_disposeAction != null)
            {
                await _disposeAction();
            }
        }
    }
}