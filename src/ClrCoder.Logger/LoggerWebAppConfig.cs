// <copyright file="LoggerWebAppConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using AspNetCore;

    using ComponentModel.IndirectX;

    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Media web application configuration.
    /// </summary>
    [ProvideConfig]
    public class LoggerWebAppConfig : IxStdProviderConfig, IIxBasicIdentificationConfig, IIxStdProviderConfig
    {
        ICollection<IIxProviderNodeConfig> IIxProviderNodeConfig.Nodes =>
            new HashSet<IIxProviderNodeConfig>(Nodes)
                {
                    new IxStdProviderConfig
                        {
                            ContractType = typeof(IWebHostBuilder),
                            InstanceBuilder = new IxExistingInstanceFactoryConfig<IWebHostBuilder>(
                                new WebHostBuilder()
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory()))
                        }
                };

        Type IIxBasicIdentificationConfig.ContractType { get; } = typeof(ILoggerWebApp);

        IIxInstanceBuilderConfig IIxStdProviderConfig.InstanceBuilder { get; } =
            new IxClassInstanceBuilderConfig<LoggerWebApp>();

        /// <summary>
        /// Specifies if mode of the site.
        /// </summary>
        public bool IsSandBox { get; set; }
    }
}