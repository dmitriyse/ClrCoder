// <copyright file="LoggerWebApp.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AspNetCore;
    using AspNetCore.Hosting;

    using ComponentModel.IndirectX;

    using Json;

    using Logger;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.Extensions.DependencyInjection;

    using Std;

    using Threading;

    /// <summary>
    /// Fron Media Web application. Provides Vimy media related REST API.
    /// </summary>
    public class LoggerWebApp : AsyncDisposableBase, ILoggerWebApp
    {
        private readonly IWebHost _webHost;

        [Require(typeof(IWebAppComponent))]

        // [Require(typeof(IClusterNode))]
        private LoggerWebApp(
            IIxResolver resolver,
            IWebHostBuilder webHostBuilder,
            IJsonLogger logger,
            ILogReader logReader,
            LoggerWebAppConfig config)
        {
            Log = new ClassJsonLogger<LoggerWebApp>(logger);

            Log.Info(_ => _("Starting Asp.Net core..."));
            _webHost = webHostBuilder

                // Should be removed after adoption IndirectX to Asp.Net core.
                .ConfigureServices(
                    x => x
                        .AddHttpRequestScopeService()
                        .AddScopedHttpContextAccessor()
                        .AddSingleton(new Tuple<IJsonLogger, IIxResolver>(Log, resolver))
                        .AddSingleton<ILogReader>(logReader)
                        .AddSingleton(config))
                .UseStartup<LoggerWebAppStartup>()
                .ConfigureJsonFormatters(JsonDefaults.RestRpcSerializerSource.Settings)
                .Build();
            _webHost.Start();

            var addressesFeature = _webHost.ServerFeatures.Get<IServerAddressesFeature>();
            var addresses = new HashSetEx<string>();

            foreach (string addressesFeatureAddress in addressesFeature.Addresses)
            {
                addresses.Add(addressesFeatureAddress);
                Log.Info(
                    addressesFeatureAddress,
                    (_, addr) => _($"Now listening on: {addr}").Data(new { ListenAddress = addr }));
            }

            HostingUrls = addresses;
        }

        /// <inheritdoc/>
        public IReadOnlySet<string> HostingUrls { get; }

        private IJsonLogger Log { get; }

        /// <inheritdoc/>
        protected override async Task DisposeAsyncCore()
        {
            try
            {
                Log.Trace(_ => _("Waiting Asp.Net core shutdown..."));
                _webHost.Dispose();
                Log.Info(_ => _("Asp.Net core shutdown completed"));
            }
            catch (Exception ex) when (ex.IsProcessable())
            {
                // Do nothing.
            }
        }
    }
}