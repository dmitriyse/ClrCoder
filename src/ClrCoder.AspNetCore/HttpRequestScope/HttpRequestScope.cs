// <copyright file="HttpRequestScope.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
#pragma warning disable 1998
    using System.Threading.Tasks;

    using ComponentModel.IndirectX;

    using Microsoft.AspNetCore.Http;

    using Threading;

    /// <summary>
    /// Http request scope implementation.
    /// </summary>
    public class HttpRequestScope : AsyncDisposableBase, IHttpRequestScope
    {
        private readonly IIxSelf _self;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestScope"/> class.
        /// </summary>
        /// <param name="self">Container agent object to control this instance.</param>
        /// <param name="resolver">The resolver for dependencies of this scope.</param>
        /// <param name="context">The http context of the request of this scope.</param>
        public HttpRequestScope(IIxSelf self, IIxResolver resolver, HttpContext context)
        {
            _self = self;
            Resolver = resolver;
            Context = context;
        }

        /// <inheritdoc/>
        public IIxResolver Resolver { get; }

        /// <inheritdoc/>
        public HttpContext Context { get; }

        /// <summary>
        /// Notifies that request processing is terminating.
        /// TODO: Refactor this to auto-dispose IndirectX feature.
        /// </summary>
        internal void NotifyAutoDispose()
        {
            _self.DisposeAsync();
        }

        /// <inheritdoc/>
        protected override async Task AsyncDispose()
        {
            // Do nothing.
        }
    }
}