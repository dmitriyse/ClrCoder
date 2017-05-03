// <copyright file="LoggerWebAppStartup.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logger
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    using System;
    using System.Net;
    using System.Reflection;

    using AspNetCore;
    using AspNetCore.Hosting;

    using ComponentModel.IndirectX;

    using JetBrains.Annotations;

    using Logging;
    using Logging.Std;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Asp.Net Core application initialized class.
    /// </summary>
    public class LoggerWebAppStartup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerWebAppStartup"/> class.
        /// </summary>
        /// <param name="env">Hosting environment.</param>
        public LoggerWebAppStartup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        /// <summary>
        /// Application configuration.
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">Hosting environment.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        /// <param name="webAppConfig">Web application global configuration.</param>
        [UsedImplicitly]
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            Tuple<IJsonLogger, IIxResolver> arguments,
            LoggerWebAppConfig webAppConfig)
        {
            IJsonLogger jsonLogger = arguments.Item1;

            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                    {
                        KnownNetworks =
                            {
                                new IPNetwork(IPAddress.Any, 0)
                            },
                        ForwardedHeaders =
                            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                            | ForwardedHeaders.XForwardedHost
                    });

            // TODO: Add request dumping.
            app.UseDelegateExceptionHandler(
                async (context, ex) =>
                    {
                        jsonLogger.Error(
                            ex,
                            (_, e) =>
                                _("WebApp pipeline error").Exception(e));
                    });
               app.UseMvc();
        }

        /// <summary>
        /// This method gets called by the runtime. Use <c>this</c> method to add <c>services</c> to the container.
        /// </summary>
        /// <param name="services">Registered <c>services</c> collection.</param>
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc().AddApplicationPart(typeof(LogsController).GetTypeInfo().Assembly);
        }
    }
}