// <copyright file="IIxHostBuilder.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IIxHostBuilder : IIxBuilder<IxHostConfig>
    {
        IIxBuilder<List<IxScopeBaseConfig>> Nodes { get; }
        Task<IIxHost> Build();
    }
}