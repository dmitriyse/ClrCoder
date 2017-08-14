// <copyright file="ClusterNodeMock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
#pragma warning disable 1998
    using System;
    using System.Threading.Tasks;

    using BigMath;

    using IO;

    using ObjectModel;

    using Threading;

    /// <summary>
    /// Cluster node mock, helpfull for standalone launch or in tests.
    /// </summary>
    public class ClusterNodeMock : AsyncDisposableBase, IClusterNode
    {
        private readonly TaskCompletionSource<int> _resultCompletionSource = new TaskCompletionSource<int>();

        ClusterObjectKey IKeyed<ClusterObjectKey>.Key
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ClusterNodeKey IKeyed<ClusterNodeKey>.Key
        {
            get
            {
                // TODO: Implement me.
                return new ClusterNodeKey("single-node");
            }
        }

        /// <inheritdoc/>
        public Int128 GetPromiseId<T>(ValueTask<T> task)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Int128 GetPromiseId<T>(TaskCompletionSource<T> completionSource)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveCall(string method, IClusterIoMessage message, IClusterIoMessageBuilder responseBuilder)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<T> RestoreFuture<T>(Int128 promiseId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public TaskCompletionSource<T> RestorePromise<T>(Int128 promiseId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<int> WaitTermination()
        {
            return new ValueTask<int>(_resultCompletionSource.Task);
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
        protected override async Task DisposeAsyncCore()
        {
            // Do nothing.
        }
    }
}