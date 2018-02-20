// <copyright file="DelegatedAsyncDisposable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Simple implementation of the <see cref="AsyncDisposableBase"/> that delegates dispose operation to an action.
    /// </summary>
    public class DelegatedAsyncDisposable : AsyncDisposableBase
    {
        [CanBeNull]
        private readonly Func<Task> _disposeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatedAsyncDisposable"/> class.
        /// </summary>
        /// <param name="disposeAction">The action that will be called on dispose.</param>
        [Obsolete("Use events")]
        public DelegatedAsyncDisposable(Func<Task> disposeAction)
        {
            if (disposeAction == null)
            {
                throw new ArgumentNullException(nameof(disposeAction));
            }

            _disposeAction = disposeAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatedAsyncDisposable"/> class.
        /// </summary>
        /// <param name="lifeTimeSyncRoot">The lifetime synchronization object.</param>
        public DelegatedAsyncDisposable([CanBeNull] object lifeTimeSyncRoot = null)
            : base(lifeTimeSyncRoot)
        {
        }

        /// <summary>
        /// Rises on object disposing.
        /// </summary>
        public event AsyncEventHandler AsyncDisposing;

        /// <summary>
        /// Fired when a dispose process has been started, event is raised under <see cref="AsyncDisposableBase.DisposeSyncRoot"/>
        /// lock.
        /// </summary>
        public event EventHandler DisposeStarted;

        /// <inheritdoc/>
        protected override async Task DisposeAsyncCore()
        {
            if (_disposeAction != null)
            {
                await Task.WhenAll(_disposeAction(), AsyncDisposing.InvokeAsync(this, new EventArgs()));
            }
            else
            {
                await AsyncDisposing.InvokeAsync(this, new EventArgs());
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposeStarted()
        {
            DisposeStarted?.Invoke(this, new EventArgs());
        }
    }
}