// <copyright file="ClrCoderClusterIxExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using AspNetCore.Hosting;

    using ComponentModel.IndirectX;

    using Microsoft.AspNetCore.Hosting;

    public static class ClrCoderClusterIxExtensions
    {
        public static IIxHostBuilder BecomeClusterNode(this IIxHostBuilder hostBuilder)
        {
            hostBuilder.Nodes
                .AddScope(
                    "ClusterNodeWebApp",
                    nodes: x =>
                        x.Add<IWebHostBuilder>(
                                factory: new IxDelegateFactoryConfig(
                                    new Func<Task<IWebHostBuilder>>(
                                        async () =>
                                            new WebHostBuilder()
                                                .UseKestrel()
                                                .UseContentRoot(Directory.GetCurrentDirectory())
                                                .ConfigureJsonFormatters(JsonDefaults.JsonRestRpcSerializerSettings))))
                            .Add<IClusterNode>(factory: new IxClassRawFactoryConfig<ClusterNode>()));

            return hostBuilder;
        }
    }
}