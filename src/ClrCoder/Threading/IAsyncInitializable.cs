// <copyright file="IAsyncInitializable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System.Threading.Tasks;

    /// <summary>
    /// Async initializable <c>object</c>.
    /// </summary>
    public interface IAsyncInitializable
    {
        /// <summary>
        /// Task that is completed when initialization finished.
        /// </summary>
        /// <remarks>
        /// If <c>this</c> <c>object</c> is also disposable. Dispose can be started before initialization, after and in-progress.
        /// TODO: Explain behavior.
        /// </remarks>
        Task InitializedTask { get; }

        /// <summary>
        /// Starts initialization. Never throws an exception. Valid even after <c>object</c> is disposed.
        /// </summary>
        void StartInitialize();
    }
}