﻿// <copyright file="LoggerWebApp.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AspNetCore;

    using ComponentModel.IndirectX;
    using ComponentModel.IndirectX.Attributes;

    using Logger;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.Extensions.DependencyInjection;

    using Std;

    using Threading;

    /// <summary>
    /// Fron Media Web application. Provides Vimy media related REST API.
    /// </summary>
    public class LoggerWebApp : AsyncDisposableBase,  ILoggerWebApp
    {
        
        private readonly IWebHost _webHost;

        [Require(typeof(IWebAppComponent))]
        //[Require(typeof(IClusterNode))]
        private LoggerWebApp(
            IIxResolver resolver,
            IWebHostBuilder webHostBuilder,
            IJsonLogger logger,
            ILogReader logReader,
            LoggerWebAppConfig config)
        {
            Log = new ClassJsonLogger<LoggerWebApp>(logger);

            LoggerExtensions.Info(Log, _ => _("Starting Asp.Net core..."));
            _webHost = webHostBuilder

                // Should be removed after adoption IndirectX to Asp.Net core.
                .ConfigureServices(
                    x => x
                        .AddSingleton(new Tuple<IJsonLogger, IIxResolver>(Log, resolver))
                        .AddSingleton(config))
                .UseStartup<LoggerWebAppStartup>()
                .Build();
            _webHost.Start();

            var addressesFeature = _webHost.ServerFeatures.Get<IServerAddressesFeature>();
            var addresses = new HashSetEx<string>();

            foreach (string addressesFeatureAddress in addressesFeature.Addresses)
            {
                addresses.Add(addressesFeatureAddress);
                LoggerExtensions.Info(
                    Log,
                    addressesFeatureAddress,
                    (_, addr) => _($"Now listening on: {addr}").Data(new { ListenAddress = addr }));
            }

            HostingUrls = addresses;
        }

        /// <inheritdoc/>
        public IReadOnlySet<string> HostingUrls { get; }

        private IJsonLogger Log { get; }

        /// <inheritdoc/>
        protected override async Task AsyncDispose()
        {
            try
            {
                LoggerExtensions.Trace(Log, _ => _("Waiting Asp.Net core shutdown..."));
                _webHost.Dispose();
                LoggerExtensions.Info(Log, _ => _("Asp.Net core shutdown completed"));
            }
            catch (Exception ex)
            {
                if (!ex.IsProcessable())
                {
                    throw;
                }
            }
        }
    }
}