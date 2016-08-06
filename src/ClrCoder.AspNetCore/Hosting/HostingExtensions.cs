// <copyright file="HostingExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.AspNetCore.Hosting
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Hosting;

    using Mono.Unix;
    using Mono.Unix.Native;

    /// <summary>
    /// Extensions related to Asp.Net Core hosting.
    /// </summary>
    [PublicAPI]
    public static class HostingExtensions
    {
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
                        var lifeTimeService =
                            services.FirstOrDefault(x => x.ServiceType == typeof(IApplicationLifetime))?
                                .ImplementationInstance as IApplicationLifetime;

                        if (lifeTimeService != null)
                        {
                            Task.Factory.StartNew(MonitorUnixSignals, lifeTimeService, TaskCreationOptions.LongRunning);
                        }
                    });
            return builder;
        }

        private static void MonitorUnixSignals(object lifeTimeServiceObj)
        {
            var lifeTimeService = (IApplicationLifetime)lifeTimeServiceObj;
            try
            {
                using (var sigInt = new UnixSignal(Signum.SIGINT))
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
}