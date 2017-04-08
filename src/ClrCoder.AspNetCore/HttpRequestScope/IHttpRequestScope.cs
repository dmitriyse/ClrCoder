// <copyright file="IHttpRequestScope.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
    using ComponentModel.IndirectX;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Http request scope.
    /// </summary>
    /// <remarks>
    /// This is temporary solution, until IndirectX will be able to replace default Asp.Net core container.
    /// </remarks>
    public interface IHttpRequestScope
    {
        /// <summary>
        /// Resolver of the scope dependencies.
        /// </summary>
        [NotNull]
        IIxResolver Resolver { get; }

        /// <summary>
        /// The http context of the request of this scope.
        /// </summary>
        HttpContext Context { get; }
    }
}