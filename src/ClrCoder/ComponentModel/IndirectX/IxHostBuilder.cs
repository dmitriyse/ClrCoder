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
    public class IxHostBuilder : IxBuilder<IxHostConfig>, IIxHostBuilder
    {

        public IxHostBuilder()
        {

            Config = new IxHostConfig();
            Nodes = new IxBuilder<List<IxScopeBaseConfig>>
                        {
                            Config = Config.Nodes
                        };
        }

        /// <inheritdoc/>
        public IIxBuilder<List<IxScopeBaseConfig>> Nodes { get; }

        /// <inheritdoc/>
        public async Task<IIxHost> Build()
        {
            var host = new IxHost();
            await host.Initialize(Config);
            return host;
        }

        public IxHostBuilder Configure([CanBeNull] Action<IIxBuilder<List<IxScopeBaseConfig>>> nodes = null)
        {
            nodes?.Invoke(
                new IxBuilder<List<IxScopeBaseConfig>>
                    {
                        Config = Config.Nodes
                    });

            return this;
        }

    }
}