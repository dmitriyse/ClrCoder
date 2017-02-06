// <copyright file="ClusterNode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Hosting;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

    /// <summary>
    /// ClrCoder cluster node.
    /// </summary>
    public class ClusterNode : IClusterNode
    {
        private IWebHost _webHost;

        /// <inheritdoc/>
        public async Task AsyncDispose()
        {
            _webHost?.Dispose();
        }

        /// <inheritdoc/>
        public async Task<int> Run()
        {
            // TODO: Rewrite to true async.
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

        public Task DisposeTask
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void StartDispose()
        {
            throw new NotImplementedException();
        }

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}