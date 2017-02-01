// <copyright file="IxHost.IxHostPlugin.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

    public partial class IxHost
    {
        /// <summary>
        /// Base implementation for host plugins.
        /// </summary>
        public abstract class IxHostPlugin
        {
            protected IxHostPlugin(IxHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException(nameof(host));
                }
                Host = host;
            }

            public IxHost Host { get; }

            /// <summary>
            /// Tries to create <c>node</c> from the specified configuration.
            /// </summary>
            /// <param name="nodeConfig">Node configuration.</param>
            /// <param name="parentNode">Parent <c>node</c>.</param>
            /// <param name="node">Created <c>node</c>.</param>
            /// <returns><see langword="true"/>, if plugin is capable to create node for the specified config type, false otherwise.</returns>
            public abstract bool TryCreateNode(
                IxScopeBaseConfig nodeConfig,
                [CanBeNull] IxScopeBase parentNode,
                [CanBeNull] out IxScopeBase node);
        }
    }
}