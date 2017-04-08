// <copyright file="ScopedHttpContextAccessor.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public class ScopedHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }
    }

    public static class ScopedHttpContextAccessorExtensions
    {
        public static IServiceCollection AddScopedHttpContextAccessor(this IServiceCollection services)
        {
            services.AddScoped<IHttpContextAccessor, ScopedHttpContextAccessor>();
            return services;
        }

        public static IApplicationBuilder UseScopedHttpContextAccessor(this IApplicationBuilder app)
        {
            app.Use(
                next =>
                    {
                        return async context =>
                            {
                                var accessor = context.RequestServices.GetService<IHttpContextAccessor>();
                                accessor.HttpContext = context;
                                await next(context);
                            };
                    });

            return app;
        }
    }
}