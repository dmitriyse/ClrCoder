// <copyright file="WebAppHttpScopeBinderMiddleware.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
    using System.Threading.Tasks;

    using ComponentModel.IndirectX;

    using Microsoft.AspNetCore.Http;

    public class WebAppHttpScopeBinderMiddleware
    {
        internal static readonly object RequestScopeKey = new object();

        private readonly IIxResolver _resolver;

        private readonly RequestDelegate _next;

        public WebAppHttpScopeBinderMiddleware(IIxResolver resolver, RequestDelegate next)
        {
            _resolver = resolver;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            using (IxLock<HttpRequestScope> requestScopeLock =
                await _resolver.Get<HttpRequestScope, HttpContext>(null, context))
            {
                context.Items[RequestScopeKey] = requestScopeLock.Target;
                try
                {
                    await _next.Invoke(context);
                }
                finally
                {
                    context.Items.Remove(RequestScopeKey);
                    requestScopeLock.Target.NotifyAutoDispose();
                }
            }
        }
    }
}