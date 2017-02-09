// <copyright file="ClusterNode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System;
    using System.Threading.Tasks;

    using ComponentModel.IndirectX;

    using JetBrains.Annotations;

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

        private ClusterNode(IIxHost indirectXHost, IWebHostBuilder webHostBuilder, IJsonLogger logger)
        {
            _indirectXHost = indirectXHost;
        }

        /// <inheritdoc/>
        public async Task<int> Run()
        {
            _webHost.Run();
            return 0;
        }

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="webHostBuilder">Asp.Net core web host builder.</param>
        /// <returns>Async execution task.</returns>
        [UsedImplicitly]
        public async Task Initialize(IWebHostBuilder webHostBuilder)
        {
            _webHost = webHostBuilder
                .UseUrls("http://*:5000")
                .UseStartup<Startup>()
                .Build();
        }

        /// <inheritdoc/>
        protected override Task AsyncDispose()
        {
            throw new NotImplementedException();
        }
    }
}