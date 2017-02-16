// <copyright file="IxHostBuilder.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// IndirectX Host builder implementation.
    /// </summary>
    public class IxHostBuilder : IxBuilder<IIxHostConfig>, IIxHostBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxHostBuilder"/> class.
        /// </summary>
        public IxHostBuilder()
        {
            Config = new IxHostConfig();
            Nodes = new IxBuilder<ICollection<IIxProviderNodeConfig>>
                        {
                            Config = Config.Nodes
                        };
        }

        /// <inheritdoc/>
        public IIxBuilder<ICollection<IIxProviderNodeConfig>> Nodes { get; }

        /// <inheritdoc/>
        public async Task<IIxHost> Build()
        {
            var host = new IxHost();
            await host.Initialize(Config);
            return host;
        }

        /// <summary>
        /// Configures IndirectX fluently.
        /// </summary>
        /// <param name="nodes">Action that configures node.</param>
        /// <returns>Host configuration fluent syntax continuation.</returns>
        public IIxHostBuilder Configure(
            [CanBeNull] Func<IIxBuilder<ICollection<IIxProviderNodeConfig>>, IIxBuilder<ICollection<IIxProviderNodeConfig>>> nodes =
                null)
        {
            nodes?.Invoke(
                new IxBuilder<ICollection<IIxProviderNodeConfig>>
                    {
                        Config = Config.Nodes
                    });

            return this;
        }
    }
}