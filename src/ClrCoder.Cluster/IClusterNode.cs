// <copyright file="IClusterNode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System.Threading.Tasks;

    using Threading;

    /// <summary>
    /// Cluster node.
    /// </summary>
    public interface IClusterNode : IAsyncDisposable
    {
        /// <summary>
        /// Waits termination events.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        Task<int> WaitTermination();
    }
}