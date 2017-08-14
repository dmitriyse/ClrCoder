// <copyright file="IIxHostBuilder.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Host builder, allows fluent syntax for IndirectX host configuration.
    /// </summary>
    public interface IIxHostBuilder : IIxBuilder<IIxHostConfig>
    {
        /// <summary>
        /// Builds root nodes.
        /// </summary>
        IIxBuilder<ICollection<IIxProviderNodeConfig>> Nodes { get; }

        /// <summary>
        /// Performs actual build.
        /// </summary>
        /// <returns>Configured IndirectX host.</returns>
        ValueTask<IIxHost> Build();
    }
}