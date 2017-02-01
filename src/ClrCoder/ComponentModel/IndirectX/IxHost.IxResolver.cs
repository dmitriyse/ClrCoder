// <copyright file="IxHost.IxResolver.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Threading.Tasks;

    /// <content><see cref="IxResolver"/> implementation.</content>
    public partial class IxHost
    {
        private class IxResolver : IIxResolver
        {
            public IxScopeBase Node { get; }

            public IxResolver(IxHost host, IxScopeBase node)
            {
                Node = node;
                if (host == null)
                {
                    throw new ArgumentNullException(nameof(host));
                }
            }

            public IxHost Host { get; }

            public async Task<TContract> Resolve<TContract>(string name = null)
            {
                var obj = await Host.Resolve(Node, typeof(TContract), name);
                return (TContract)obj;
            }
        }
    }
}