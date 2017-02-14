// <copyright file="WebHostDeployment.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Helps to controls WebHost urls.
    /// </summary>
    public class WebHostDeployment : IWebAppComponent
    {
        private WebHostDeployment(IWebHostBuilder webHostBuilder, WebHostDeploymentConfig config)
        {
            webHostBuilder
                .UseUrls();
        }
    }
}