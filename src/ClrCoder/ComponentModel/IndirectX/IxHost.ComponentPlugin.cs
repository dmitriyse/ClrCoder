// <copyright file="IxHost.ComponentPlugin.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

    /// <content><see cref="ComponentPlugin"/> implementation.</content>
    public partial class IxHost
    {
        private class ComponentPlugin : IxHostPlugin
        {
            public ComponentPlugin(IxHost host)
                : base(host)
            {
            }

            /// <inheritdoc/>
            public override bool TryCreateNode(
                IxScopeBaseConfig nodeConfig,
                [CanBeNull] IxScopeBase parentNode,
                [CanBeNull] out IxScopeBase node)
            {
                if (nodeConfig == null)
                {
                    throw new ArgumentNullException(nameof(nodeConfig));
                }

                if (nodeConfig.GetType() != typeof(IxComponentConfig))
                {
                    node = null;
                    return false;
                }

                node = new IxComponent(Host, parentNode, (IxComponentConfig)nodeConfig);
                return true;
            }
        }
    }
}