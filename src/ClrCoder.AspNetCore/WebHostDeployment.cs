// <copyright file="WebHostDeployment.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
    using ClrCoder.Validation;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Helps to controls WebHost urls.
    /// </summary>
    [PublicAPI]
    public class WebHostDeployment : IWebAppComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebHostDeployment"/> class.
        /// </summary>
        /// <param name="webHostBuilder">Web host builder to inject to.</param>
        /// <param name="appConfig">Application configuration root.</param>
        /// <param name="config">Deployment configuration.</param>
        public WebHostDeployment(
            IWebHostBuilder webHostBuilder,
            IConfigurationRoot appConfig,
            WebHostDeploymentConfig config)
        {
            VxArgs.NotNull(webHostBuilder, nameof(webHostBuilder));
            VxArgs.NotNull(appConfig, nameof(appConfig));
            VxArgs.NotNull(config, nameof(config));

            string urls = null;

            if (config.UrlsConfigKey != null)
            {
                urls = appConfig.GetValue<string>(config.UrlsConfigKey);
            }

            if (urls == null && config.DefaultUrls != null)
            {
                urls = config.DefaultUrls;
            }

            if (urls != null)
            {
                webHostBuilder
                    .UseUrls(urls);
            }
        }
    }
}