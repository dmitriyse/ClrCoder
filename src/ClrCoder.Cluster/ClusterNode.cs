// <copyright file="ClusterNode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using ComponentModel.IndirectX;

    using Logging;
    using Logging.Std;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.Extensions.DependencyInjection;

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

        private IApplicationLifetime _hostLifetimeService;

        private ClusterNode(
            IIxHost indirectXHost,
            IWebHostBuilder webHostBuilder,
            IJsonLogger logger)
        {
            Log = new ClassJsonLogger<ClusterNode>(logger);
            _executeCts = new CancellationTokenSource();
            _executeCancellationToken = _executeCts.Token;

            _indirectXHost = indirectXHost;

            Log.Info(_ => _("Starting Asp.Net core..."));

            _webHost = webHostBuilder
                .UseStartup<ClusterNodeStartup>()
                .Build();

            _webHost.Start();
            _hostLifetimeService = _webHost.Services.GetRequiredService<IApplicationLifetime>();

            var addressesFeature = _webHost.ServerFeatures.Get<IServerAddressesFeature>();

            foreach (string addressesFeatureAddress in addressesFeature.Addresses)
            {
                Log.Info(
                    addressesFeatureAddress,
                    (_, addr) => _($"Now listening on: {addr}").Data(new { ListenAddress = addr }));
            }

            Console.CancelKeyPress += (sender, e) =>
                {
                    if (!_executeCancellationToken.IsCancellationRequested)
                    {
                        Log.Trace(_ => _("Termination raised by system."));
                        _executeCts.Cancel();
                    }

                    e.Cancel = true;
                };
        }

        private ClassJsonLogger<ClusterNode> Log { get; }

        /// <inheritdoc/>
        public async Task<int> WaitTermination()
        {
            await _executeCancellationToken;
            Log.Trace(_ => _("Termination started..."));
            return 0;
        }

        /// <inheritdoc/>
        protected override async Task AsyncDispose()
        {
            try
            {
                if (!_executeCancellationToken.IsCancellationRequested)
                {
                    Log.Trace(_ => _("Termination raised by component dispose."));
                    _executeCts.Cancel();
                }

                Log.Trace(_ => _("Waiting Asp.Net core shutdown..."));

                try
                {
                    _hostLifetimeService.StopApplication();
                    _webHost?.Dispose();
                }
                catch (Exception ex)
                {
                    if (!ex.IsProcessable())
                    {
                        throw;
                    }
                }

                Log.Trace(_ => _("Asp.Net core shutdown completed"));
            }
            finally
            {
                _executeCts?.Dispose();
                Log.Info(_ => _("Shutdown completed"));
            }
        }
    }
}