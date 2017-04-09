// <copyright file="ClrCoderClusterModuleExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System;
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
        /// Temporary solution for cluster references, until full support will be implemented in the IndirectX.
        /// </summary>
        /// <typeparam name="T">The type of the reference target.</typeparam>
        /// <param name="resolver">The IndirectX resolver.</param>
        /// <param name="clusterRef">The reference to resolve.</param>
        /// <returns>Lock on accessor proxy to the reference target.</returns>
        public static IxLock<T> ClusterGet<T>(this IIxResolver resolver, IClusterRef<T> clusterRef)
            where T : class
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            if (clusterRef == null)
            {
                throw new ArgumentNullException(nameof(clusterRef));
            }

            var runtimeRef = clusterRef as RuntimeLocalRef<T>;
            if (runtimeRef == null)
            {
                throw new NotImplementedException();
            }

            return new IxLock<T>(runtimeRef.Target);
        }

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