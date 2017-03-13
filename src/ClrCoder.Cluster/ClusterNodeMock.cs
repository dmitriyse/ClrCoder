// <copyright file="ClusterNodeMock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
#pragma warning disable 1998
    using System.Threading.Tasks;

    using Threading;

    /// <summary>
    /// Cluster node mock, helpfull for standalone launch or in tests.
    /// </summary>
    public class ClusterNodeMock : AsyncDisposableBase, IClusterNode
    {
        private readonly TaskCompletionSource<int> _resultCompletionSource = new TaskCompletionSource<int>();

        /// <inheritdoc/>
        public Task<int> WaitTermination()
        {
            return _resultCompletionSource.Task;
        }

        /// <summary>
        /// Emulates exit.
        /// </summary>
        /// <param name="exitCode">Exit code.</param>
        public void Exit(int exitCode)
        {
            _resultCompletionSource.SetResult(exitCode);
        }

        /// <inheritdoc/>
        protected override async Task AsyncDispose()
        {
            // Do nothing.
        }
    }
}