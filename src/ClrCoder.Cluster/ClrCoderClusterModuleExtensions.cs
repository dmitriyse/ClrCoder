// <copyright file="ClrCoderClusterModuleExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System.IO;

    using ComponentModel.IndirectX;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Cluster node helper methods.
    /// </summary>
    [PublicAPI]
    public static class ClrCoderClusterModuleExtensions
    {
        /// <summary>
        /// Simple way to register cluster node dependencies.
        /// </summary>
        /// <param name="hostBuilder">IndirectX host builder.</param>
        /// <returns>Fluent syntax continuation. (The same host builder).</returns>
        public static IIxHostBuilder UseAsClusterNode(this IIxHostBuilder hostBuilder)
        {
            hostBuilder.Nodes
                .Add<IClusterNode>(
                    instanceBuilder: new IxClassInstanceBuilderConfig<ClusterNode>(),
                    nodes: x =>
                        x.Add<IWebHostBuilder>(
                            instanceBuilder: new IxExistingInstanceFactoryConfig<IWebHostBuilder>(
                                new WebHostBuilder()
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory()))));

            return hostBuilder;
        }
    }
}