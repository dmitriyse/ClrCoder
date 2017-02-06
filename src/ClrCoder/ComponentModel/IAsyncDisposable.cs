// <copyright file="IAsyncDisposable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel
{
    using System.Threading.Tasks;

    /// <summary>
    /// Asynchronously disposable.
    /// </summary>
    public interface IAsyncDisposable
    {
        /// <summary>
        /// Dispose task. Returns valid dispose task even <see cref="StartDispose"/> was not called yet.
        /// </summary>
        Task DisposeTask { get; }

        /// <summary>
        /// Initiates async disposing, allowed to be called multiple times.
        /// </summary>
        void StartDispose();
    }
}