// <copyright file="HostingExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore.Hosting
{
    using System.Linq;
    using JetBrains.Annotations;

#if NET46 || NETSTANDARD1_6
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Reflection;


    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.ObjectPool;
    using Microsoft.Extensions.Options;

    using Newtonsoft.Json;

#endif
#if NET46
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;

    using Mono.Unix;
    using Mono.Unix.Native;

#endif

    /// <summary>
    /// Extensions related to Asp.Net Core hosting.
    /// </summary>
    [PublicAPI]
    public static class HostingExtensions
    {
#if NET46 || NETSTANDARD1_6

        [UsedImplicitly]
        private class CustomSerializerSettingsSetup : IConfigureOptions<MvcOptions>
        {
            private readonly ILoggerFactory _loggerFactory;

            private readonly ArrayPool<char> _charPool;

            private readonly ObjectPoolProvider _objectPoolProvider;

            public CustomSerializerSettingsSetup(
                ILoggerFactory loggerFactory,
                ArrayPool<char> charPool,
                ObjectPoolProvider objectPoolProvider)
            {
                _loggerFactory = loggerFactory;
                _charPool = charPool;
                _objectPoolProvider = objectPoolProvider;
            }

            public JsonSerializerSettings SerializerSettings { get; set; }

            public void Configure([NotNull] MvcOptions options)
            {
                options.OutputFormatters.RemoveType<JsonOutputFormatter>();
                options.InputFormatters.RemoveType<JsonInputFormatter>();
                options.InputFormatters.RemoveType<JsonPatchInputFormatter>();

                JsonSerializerSettings outputSettings = SerializerSettings;
                options.OutputFormatters.Add(new JsonOutputFormatter(outputSettings, _charPool));

                JsonSerializerSettings inputSettings = SerializerSettings;
                ILogger<JsonInputFormatter> jsonInputLogger = _loggerFactory.CreateLogger<JsonInputFormatter>();
                options.InputFormatters.Add(
                    new JsonInputFormatter(jsonInputLogger, inputSettings, _charPool, _objectPoolProvider));

                ILogger<JsonPatchInputFormatter> jsonInputPatchLogger =
                    _loggerFactory.CreateLogger<JsonPatchInputFormatter>();
                options.InputFormatters.Add(
                    new JsonPatchInputFormatter(jsonInputPatchLogger, inputSettings, _charPool, _objectPoolProvider));
            }
        }

        /// <summary>
        /// Applies serializer <c>settings</c> for input/output json communication.
        /// </summary>
        /// <param name="hostBuilder">Host builder.</param>
        /// <param name="settings">Json serializer <c>settings</c>.</param>
        /// <returns>Fluent syntax continuation.</returns>
        public static IWebHostBuilder ConfigureJsonFormatters(
            this IWebHostBuilder hostBuilder,
            JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            hostBuilder.ConfigureServices(
                services =>
                    {
                        services.AddTransient<CustomSerializerSettingsSetup>();
                        services.AddTransient<IConfigureOptions<MvcOptions>>(
                            provider =>
                                {
                                    var s = provider.GetService<CustomSerializerSettingsSetup>();
                                    s.SerializerSettings = settings;
                                    return s;
                                });
                    });

            return hostBuilder;
        }

        /// <summary>
        /// Allows MVC to discover only one specified controller.
        /// </summary>
        /// <typeparam name="TController">Controller type.</typeparam>
        /// <param name="builder">Web host <c>builder</c>.</param>
        /// <returns>Fluent syntax continuation.</returns>
        public static IWebHostBuilder UseOnlyController<TController>(this IWebHostBuilder builder)
        {
            builder.ConfigureServices(
                x =>
                    {
                        x.AddMvcCore()
                        .AddApplicationPart(typeof(TController).GetTypeInfo().Assembly)
                        .ConfigureApplicationPartManager(
                            m =>
                                {
                                    var controllerProviders =
                                        m.FeatureProviders.Where(f => f is ControllerFeatureProvider).ToArray();
                                    foreach (var controllerProvider in controllerProviders)
                                    {
                                        m.FeatureProviders.Remove(controllerProvider);
                                    }

                                    m.FeatureProviders.Add(
                                        new SingleControllerProvider(
                                            new HashSet<TypeInfo> { typeof(TController).GetTypeInfo() }));
                                });
                    });
            return builder;
        }

        private class SingleControllerProvider : ControllerFeatureProvider
        {
            private readonly HashSet<TypeInfo> _allowedControllers;

            public SingleControllerProvider(HashSet<TypeInfo> allowedControllers)
            {
                if (allowedControllers == null)
                {
                    throw new ArgumentNullException(nameof(allowedControllers));
                }

                _allowedControllers = allowedControllers;
            }

            protected override bool IsController(TypeInfo typeInfo)
            {
                return _allowedControllers.Contains(typeInfo);
            }
        }
#endif
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
                                int id = UnixSignal.WaitAny(signals);

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