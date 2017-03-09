// <copyright file="DelegatingStartupFilter.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore.Hosting
{
    using System;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Startup filter that delegates it work.
    /// </summary>
    public class DelegatingStartupFilter : IStartupFilter
    {
        private readonly Action<IApplicationBuilder> _appInitializedDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingStartupFilter"/> class.
        /// </summary>
        /// <param name="appInitializedDelegate">App initializer delegate.</param>
        public DelegatingStartupFilter(Action<IApplicationBuilder> appInitializedDelegate)
        {
            _appInitializedDelegate = appInitializedDelegate;
        }

        /// <inheritdoc/>
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
                {
                    _appInitializedDelegate(app);
                    next(app);
                };
        }
    }
}