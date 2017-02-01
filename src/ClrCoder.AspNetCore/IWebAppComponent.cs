// <copyright file="IWebAppComponent.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
    /// <summary>
    /// Web application component. Helps to decompose Asp.Net core application into encapsulated components.
    /// </summary>
    /// <remarks>
    /// Asp.Net core have bunch of extension points (IWebHostBuilder, IApplicationBuilder, IMvc, multiple components can subscribes to those points.
    /// </remarks>
    public interface IWebAppComponent
    {
    }
}