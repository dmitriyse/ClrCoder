// <copyright file="HostUtils.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore.Hosting
{
#pragma warning disable 1998
#if NET46 || NETSTANDARD1_6
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Asp.Net core hosting utils.
    /// </summary>
    public static class HostUtils
    {
        /// <summary>
        /// Starts Asp.Net core host for the specified controller on the specified <c>urls</c>.
        /// </summary>
        /// <typeparam name="TController">Type of controller to host.</typeparam>
        /// <param name="urls">Hosting <c>urls</c>.</param>
        /// <param name="hostBuilderAction">Performs additional steps to build host.</param>
        /// <param name="exceptionHandler">Hosting global exception handler.</param>
        /// <returns>Configured and running asp.net core host.</returns>
        public static IWebHost HostController<TController>(
            string urls,
            Func<IWebHostBuilder, IWebHostBuilder> hostBuilderAction = null,
            Func<Exception, Task> exceptionHandler = null)
        {
            if (urls == null)
            {
                throw new ArgumentNullException(nameof(urls));
            }

            IWebHostBuilder builder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseOnlyController<TController>()
                .ConfigureServices(services => services.AddMvc())
                .Configure(
                    app => { app.UseMvc(); });

            if (exceptionHandler != null)
            {
                builder.ConfigureServices(
                    services =>
                        {
                            services.AddSingleton<IStartupFilter>(
                                new DelegatingStartupFilter(
                                    app =>
                                        {
                                            app.UseExceptionHandler(
                                                new ExceptionHandlerOptions
                                                    {
                                                        ExceptionHandler =
                                                            async context =>
                                                                {
                                                                    if (context.Response.StatusCode
                                                                        == (int)HttpStatusCode.InternalServerError)
                                                                    {
                                                                        var errorFeature = context
                                                                            .Features
                                                                            .Get<IExceptionHandlerFeature>();
                                                                        
                                                                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                                                                        if (errorFeature != null)
                                                                        {
                                                                            await exceptionHandler(errorFeature.Error);
                                                                        }
                                                                    }
                                                                }
                                                    });
                                        }));
                        });
            }

            if (hostBuilderAction != null)
            {
                builder = hostBuilderAction(builder);
            }

            IWebHost host = builder
                .UseUrls(urls)
                .Build();

            host.Start();

            return host;
        }
    }
#endif
}