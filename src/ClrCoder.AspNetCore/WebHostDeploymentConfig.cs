// <copyright file="WebHostDeploymentConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
    using ComponentModel.IndirectX;

    using JetBrains.Annotations;

    /// <summary>
    /// Web host deployment configuration.
    /// </summary>
    [ProvideConfig]
    public class WebHostDeploymentConfig : IxStdProviderConfig, IIxStdProviderConfig
    {
        IxIdentifier IIxProviderNodeConfig.Identifier => new IxIdentifier(typeof(IWebAppComponent), Name);

        IIxInstanceBuilderConfig IIxStdProviderConfig.Factory => new IxClassInstanceBuilderConfig<WebHostDeployment>();

        /// <summary>
        /// Environment variable that can be used to <c>override</c> urls.
        /// </summary>
        [CanBeNull]
        public string UrlsEnvironmentVariableName { get; set; }

        /// <summary>
        /// Default urls that should be used, when urls are not specified by any other means.
        /// </summary>
        [CanBeNull]
        public string DefaultUrls { get; set; }
    }
}