// <copyright file="HttpRequestScopeExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
    using ComponentModel.IndirectX;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public static class HttpRequestScopeExtensions
    {
        public static IHttpRequestScope GetRequestScope(this HttpContext context)
        {
            return (IHttpRequestScope)context.Items[WebAppHttpScopeBinderMiddleware.RequestScopeKey];
        }

        public static IApplicationBuilder UseRequestScope(this IApplicationBuilder builder, IIxResolver resolver)
        {
            builder.UseMiddleware<WebAppHttpScopeBinderMiddleware>(resolver);
            return builder;
        }

        public static IServiceCollection AddHttpRequestScopeService(this IServiceCollection services)
        {
            services.AddScoped(sp => sp.GetService<IHttpContextAccessor>().HttpContext.GetRequestScope());
            return services;
        }
    }
}