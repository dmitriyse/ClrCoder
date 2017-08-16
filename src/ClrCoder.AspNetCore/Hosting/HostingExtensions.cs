// <copyright file="HostingExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore.Hosting
{
    using JetBrains.Annotations;

#if NETSTANDARD1_6 || NETSTANDARD2_0
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.ObjectPool;
    using Microsoft.Extensions.Options;

    using Newtonsoft.Json;

#endif

    /// <summary>
    /// Extensions related to Asp.Net Core hosting.
    /// </summary>
    [PublicAPI]
    public static class HostingExtensions
    {
#if NETSTANDARD1_6 || NETSTANDARD2_0

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
                        x.AddMvc()
                            .AddApplicationPart(typeof(TController).GetTypeInfo().Assembly)
                            .ConfigureApplicationPartManager(
                                m =>
                                    {
                                        IApplicationFeatureProvider[] controllerProviders =
                                            m.FeatureProviders.Where(f => f is ControllerFeatureProvider).ToArray();
                                        foreach (IApplicationFeatureProvider controllerProvider in controllerProviders)
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

        /// <summary>
        /// Subscribes handler delegate on pipeline exceptions.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="exceptionHandler">Exception handler delegate.</param>
        /// <returns>Application builder fluent syntax continuation.</returns>
        public static IApplicationBuilder UseDelegateExceptionHandler(
            this IApplicationBuilder app,
            Func<Exception, Task> exceptionHandler)
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
            return app;
        }

        /// <summary>
        /// Subscribes handler delegate on pipeline exceptions.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="exceptionHandler">Exception handler delegate.</param>
        /// <returns>Application builder fluent syntax continuation.</returns>
        public static IApplicationBuilder UseDelegateExceptionHandler(
            this IApplicationBuilder app,
            Func<HttpContext, Exception, Task> exceptionHandler)
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
                                            await exceptionHandler(context, errorFeature.Error);
                                        }
                                    }
                                }
                    });
            return app;
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
    }
}