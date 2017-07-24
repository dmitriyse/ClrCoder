// <copyright file="ClusterNode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>


#pragma warning disable 1998

namespace ClrCoder.Cluster
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using BigMath;

    using ComponentModel.IndirectX;

    using IO;

    using Logging;
    using Logging.Std;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using ObjectModel;

    using Threading;

    /// <summary>
    /// ClrCoder cluster node.
    /// </summary>
    public class ClusterNode : AsyncDisposableBase, IClusterNode
    {
        private const string ClusterNodeIdKey = "ClrCoder:Cluster:NodeId";

        private readonly IIxHost _indirectXHost;

        private readonly IConfigurationRoot _configRoot;

        private readonly ConditionalWeakTable<object, object> _objectToKey = new ConditionalWeakTable<object, object>();

        private IWebHost _webHost;

        private CancellationTokenSource _executeCts;

        private CancellationToken _executeCancellationToken;

        private IApplicationLifetime _hostLifetimeService;

        private ClusterNode(
            IIxHost indirectXHost,
            IWebHostBuilder webHostBuilder,
            IConfigurationRoot configRoot,
            IJsonLogger logger)
        {
            Log = new ClassJsonLogger<ClusterNode>(logger);
            _executeCts = new CancellationTokenSource();
            _executeCancellationToken = _executeCts.Token;

            _indirectXHost = indirectXHost;
            _configRoot = configRoot;

            Log.Info(_ => _("Starting Asp.Net core..."));

            _webHost = webHostBuilder
                .UseStartup<ClusterNodeStartup>()
                .Build();

            _webHost.Start();
            _hostLifetimeService = _webHost.Services.GetRequiredService<IApplicationLifetime>();

            Key = new ClusterNodeKey(_configRoot.GetValue(ClusterNodeIdKey, "single-node"));

            var addressesFeature = _webHost.ServerFeatures.Get<IServerAddressesFeature>();

            foreach (string addressesFeatureAddress in addressesFeature.Addresses)
            {
                Log.Info(
                    addressesFeatureAddress,
                    (_, addr) => _($"NodeId = '{Key.Code}' listening on: {addr}").Data(new { ListenAddress = addr }));
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

        /// <inheritdoc/>
        public ClusterNodeKey Key { get; }

        ClusterObjectKey IKeyed<ClusterObjectKey>.Key { get; }

        private ClassJsonLogger<ClusterNode> Log { get; }

        /// <inheritdoc/>
        public Int128 GetPromiseId<T>(Task<T> task)
        {
            return (Int128)_objectToKey.GetValue(task, k => Guid.NewGuid().ToInt());
        }

        /// <inheritdoc/>
        public Int128 GetPromiseId<T>(TaskCompletionSource<T> completionSource)
        {
            return (Int128)_objectToKey.GetValue(completionSource, k => Guid.NewGuid().ToInt());
        }

        /// <inheritdoc/>
        Task IClusterIoObject.ReceiveCall(
            string method,
            IClusterIoMessage message,
            IClusterIoMessageBuilder responseBuilder)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<T> RestoreFuture<T>(Int128 promiseId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public TaskCompletionSource<T> RestorePromise<T>(Int128 promiseId)
        {
            throw new NotImplementedException();
        }

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
                    _webHost?.Dispose();
                }
                catch (Exception ex) when (ex.IsProcessable())
                {
                    // Do nothing.
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