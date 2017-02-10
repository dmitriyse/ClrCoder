// <copyright file="ClusterNode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System.Threading;
    using System.Threading.Tasks;

    using ComponentModel.IndirectX;

    using Logging;

    using Microsoft.AspNetCore.Hosting;

    using Threading;

    /// <summary>
    /// ClrCoder cluster node.
    /// </summary>
    public class ClusterNode : AsyncDisposableBase, IClusterNode
    {
        private readonly IIxHost _indirectXHost;

        private IWebHost _webHost;

        private CancellationTokenSource _executeCts;

        private CancellationToken _executeCancellationToken;

        private ClusterNode(
            IIxHost indirectXHost,
            IWebHostBuilder webHostBuilder,
            IJsonLogger logger)
        {
            _executeCts = new CancellationTokenSource();
            _indirectXHost = indirectXHost;
            _webHost = webHostBuilder
                .UseStartup<ClusterNodeStartup>()
                .Build();
            _webHost.Start();
        }

        /// <inheritdoc/>
        public async Task<int> WaitTermination()
        {
            await _executeCancellationToken;
            return 0;
        }

        /// <inheritdoc/>
        protected override Task AsyncDispose()
        {
            _executeCts.Cancel();
            return Task.CompletedTask;
        }
    }
}