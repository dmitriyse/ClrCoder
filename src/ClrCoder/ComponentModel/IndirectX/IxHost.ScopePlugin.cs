// <copyright file="IxHost.ScopePlugin.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

    /// <content><see cref="ScopePlugin"/> implementation.</content>
    public partial class IxHost
    {
        private class ScopePlugin : IxHostPlugin
        {
            public ScopePlugin(IxHost host)
                : base(host)
            {
            }

            public override bool TryCreateNode(
                IxScopeBaseConfig nodeConfig,
                [CanBeNull] IxScopeBase parentNode,
                [CanBeNull] out IxScopeBase node)
            {
                if (nodeConfig == null)
                {
                    throw new ArgumentNullException(nameof(nodeConfig));
                }
                if (parentNode == null)
                {
                    if (nodeConfig.GetType() != typeof(IxHostConfig))
                    {
                        node = null;
                        return false;
                    }
                }
                else
                {
                    if (nodeConfig.GetType() != typeof(IxScopeConfig))
                    {
                        node = null;
                        return false;
                    }
                }

                node = new IxScope(Host, parentNode, (IxScopeConfig)nodeConfig);
                return true;
            }
        }
    }
}