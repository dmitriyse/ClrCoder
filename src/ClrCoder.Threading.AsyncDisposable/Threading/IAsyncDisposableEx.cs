// <copyright file="IAsyncDisposableEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Threading
{
    using JetBrains.Annotations;

    using Tasks;

    //// ReSharper disable once InheritdocConsiderUsage

    /// <summary>
    /// Extended version of the <see cref="IAsyncDisposable"/>, that allows to subscribe to disposed event, even when dispose
    /// operation is not yet started.
    /// </summary>
    [PublicAPI]
    public interface IAsyncDisposableEx : IAsyncDisposable
    {
        /// <summary>
        /// Disposed event task. Returns valid dispose task even <see cref="IAsyncDisposable.DisposeAsync"/> was not called yet.
        /// <br/>
        /// When something goes wrong and problem cannot be handled - task should be completed with unprocessable exception to hint
        /// application crash.
        /// </summary>
        /// <remarks>
        /// The <see cref="IAsyncDisposable.DisposeAsync"/> method should returns the same <see cref="Task"/> object as it was
        /// retrivied from this property.
        /// </remarks>
        Task Disposed { get; }
    }
}