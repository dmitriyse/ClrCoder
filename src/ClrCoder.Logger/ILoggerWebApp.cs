// <copyright file="ILoggerWebApp.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System.Collections.Generic;
    using System.Threading;

    using Annotations;

    using Threading;

    /// <summary>
    /// Logger Web application. Provides Vimy media related REST API.
    /// </summary>
    public interface ILoggerWebApp : IAsyncDisposable
    {
        /// <summary>
        /// Hosting urls.
        /// </summary>
        [NotEmpty]
        IReadOnlySet<string> HostingUrls { get; }
    }
}