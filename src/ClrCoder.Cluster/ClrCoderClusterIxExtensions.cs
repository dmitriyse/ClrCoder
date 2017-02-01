// <copyright file="ClrCoderClusterIxExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System.IO;

    using AspNetCore.Hosting;

    using ComponentModel.IndirectX;

    using Microsoft.AspNetCore.Hosting;

    public static class ClrCoderClusterIxExtensions
    {
        public static IIxHostBuilder BecomeClusterNode(this IIxHostBuilder hostBuilder)
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            hostBuilder.Nodes
                .AddScope(
                    "ClusterNodeWebApp",
                    nodes: x =>
                        x.Add(
                                factory:
                                async r =>
                                    new WebHostBuilder()
                                        .UseKestrel()
                                        .UseContentRoot(Directory.GetCurrentDirectory())
                                        .ConfigureJsonFormatters(JsonDefaults.JsonRestRpcSerializerSettings))
                            .Add<IClusterNode>(factoryType: typeof(ClusterNode)));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            return hostBuilder;
        }
    }
}