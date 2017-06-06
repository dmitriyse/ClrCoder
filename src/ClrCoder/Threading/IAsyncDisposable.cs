// <copyright file="IAsyncDisposable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System.Threading.Tasks;

    using Annotations;

    /// <summary>
    /// Asynchronously disposable.
    /// </summary>
    public interface IAsyncDisposable
    {
        /// <summary>
        /// Dispose task. Returns valid dispose task even <see cref="StartDispose"/> was not called yet. <br/>
        /// When something goes wrong and problem cannot be handled - task should be completed with unprocessable exception to hint
        /// application crash.
        /// </summary>
        Task DisposeTask { get; }

        /// <summary>
        /// Initiates async disposing, allowed to be called multiple times. Should never <see langword="throw"/> an exception.
        /// </summary>
        [Robust]
        void StartDispose();
    }
}