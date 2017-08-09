// <copyright file="IClusterNode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using BigMath;

    using IO;

    using Threading;

    /// <summary>
    /// Cluster node.
    /// </summary>
    public interface IClusterNode : IClusterIoObject, IRemoteClusterNode, IAsyncDisposable
    {
        /// <summary>
        /// Gets globally unique identifier for the in-process C# promise.
        /// </summary>
        /// <typeparam name="T">The type of the promise result.</typeparam>
        /// <param name="task">The TPL task, that will be passed across cluster nodes.</param>
        /// <returns>The permanent globally unique identifier for the specified promise.</returns>
        Int128 GetPromiseId<T>(Task<T> task);

        /// <summary>
        /// Gets globally unique identifier for the in-process C# promise.
        /// </summary>
        /// <typeparam name="T">The type of the promise result.</typeparam>
        /// <param name="completionSource">The TPL completion source, that will be passed across cluster nodes.</param>
        /// <returns>The permanent globally unique identifier for the specified promise.</returns>
        Int128 GetPromiseId<T>(TaskCompletionSource<T> completionSource);

        /// <summary>
        /// Restores C# future from the identifier obtained in the previous node run.
        /// </summary>
        /// <typeparam name="T">The type of the promise result.</typeparam>
        /// <param name="promiseId">The promise unique identifier.</param>
        /// <returns>The promise object corresponding to the .</returns>
        /// <exception cref="KeyNotFoundException">Specified identifier is unknown for the communication subsystem.</exception>
        Task<T> RestoreFuture<T>(Int128 promiseId);

        /// <summary>
        /// Restores C# promise from the identifier obtained in the previous node run.
        /// </summary>
        /// <typeparam name="T">The type of the promise result.</typeparam>
        /// <param name="promiseId">The promise unique identifier.</param>
        /// <returns>The promise object corresponding to the .</returns>
        /// <exception cref="KeyNotFoundException">Specified identifier is unknown for the communication subsystem.</exception>
        TaskCompletionSource<T> RestorePromise<T>(Int128 promiseId);

        /// <summary>
        /// Waits termination events.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        Task<int> WaitTermination();
    }
}