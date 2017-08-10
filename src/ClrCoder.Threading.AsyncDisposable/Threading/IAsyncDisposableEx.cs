// <copyright file="IAsyncDisposableEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Threading
{
    using Tasks;

    /// <summary>
    /// Asynchronously disposable.
    /// </summary>
    public interface IAsyncDisposableEx: IAsyncDisposable
    {
        /// <summary>
        /// Dispose task. Returns valid dispose task even <see cref="IAsyncDisposable.DisposeAsync"/> was not called yet. <br/>
        /// When something goes wrong and problem cannot be handled - task should be completed with unprocessable exception to hint
        /// application crash.
        /// </summary>
        Task DisposeTask { get; }
    }
}