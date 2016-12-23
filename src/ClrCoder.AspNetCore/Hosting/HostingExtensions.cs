// <copyright file="HostingExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.AspNetCore.Hosting
{

    using JetBrains.Annotations;

#if NET46
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Mono.Unix;
    using Mono.Unix.Native;
#endif
    /// <summary>
    /// Extensions related to Asp.Net Core hosting.
    /// </summary>
    [PublicAPI]
    public static class HostingExtensions
    {

#if NET46
        /// <summary>
        /// Allow self-host to be terminated by posix termination signals (SIGINT, SIGTERM).
        /// </summary>
        /// <param name="builder">Web host builder.</param>
        /// <returns>The same builder for fluent syntax.</returns>
        [NotNull]
        public static IWebHostBuilder UsePosixSignalsListener([NotNull] this IWebHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureServices(
                services =>
                    {
                        services.AddSingleton(
                            typeof(IStartupFilter),
                            serviceProvider => new StartupFilterForUnixSignals(serviceProvider));
                    });
            return builder;
        }

        private static void MonitorUnixSignals(object lifeTimeServiceObj)
        {
            if (EnvironmentEx.OSFamily.HasFlag(OSFamilyTypes.Posix))
            {
                var lifeTimeService = (IApplicationLifetime)lifeTimeServiceObj;
                try
                {
                    using (var sigInt = new UnixSignal(Signum.SIGINT))
                    {
                        using (var sigTerm = new UnixSignal(Signum.SIGTERM))
                        {
                            var signals = new[] { sigInt, sigTerm };
                            for (;;)
                            {
                                var id = UnixSignal.WaitAny(signals);

                                if (id >= 0 && id < signals.Length)
                                {
                                    if (signals[id].IsSet)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // This place is only reachable, if SIGINT or SIGTERM signaled.
                    lifeTimeService.StopApplication();
                }
                catch (ThreadAbortException)
                {
                    // Place for some gracefull termination.
                }

                // Unreachable code.
            }
        }

        private class StartupFilterForUnixSignals : IStartupFilter
        {
            [NotNull]
            private readonly IServiceProvider _serviceProvider;

            public StartupFilterForUnixSignals([NotNull] IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                var lifeTimeService = _serviceProvider.GetRequiredService<IApplicationLifetime>();

                if (lifeTimeService != null)
                {
                    Task.Factory.StartNew(MonitorUnixSignals, lifeTimeService, TaskCreationOptions.LongRunning);
                }

                return next;
            }
        }
#endif
    }
}