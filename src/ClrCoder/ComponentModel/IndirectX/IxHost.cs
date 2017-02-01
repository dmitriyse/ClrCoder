// <copyright file="IxHost.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// IndirectX host.
    /// </summary>
    public partial class IxHost : IIxHost
    {
        private readonly List<IxHostPlugin> _plugins = new List<IxHostPlugin>();

        private readonly FancyLock _globalLock;

        private IxScope _rootScope;

        /// <summary>
        /// Creates new host.
        /// </summary>
        public IxHost()
        {
            _plugins.Add(new ScopePlugin(this));
            _plugins.Add(new ComponentPlugin(this));
            _globalLock = new FancyLock(this);
        }

        public IIxResolver Resolver { get; }

        /// <inheritdoc/>
        public async Task AsyncDispose()
        {
            // Do Nothing.
        }

        public IDisposable GlobalLock()
        {
            Monitor.Enter(_globalLock);
            return _globalLock;
        }

        public async Task Initialize(IxHostConfig config)
        {
            var allConfigNodes = new HashSet<IxScopeBaseConfig>();
            Action<IxScopeBase, IxScopeBaseConfig> buildNodeAction = null;
            buildNodeAction = (parentNode, nodeConfig) =>
                {
                    if (!allConfigNodes.Add(nodeConfig))
                    {
                        throw new InvalidOperationException(
                            "Configuration contains the same object multiple times. Currently it's not allowed to avoid visitor cycles.");
                    }

                    IxScopeBase node = null;
                    foreach (IxHostPlugin plugin in _plugins)
                    {
                        if (plugin.TryCreateNode(nodeConfig, parentNode, out node))
                        {
                            break;
                        }
                    }

                    // parent node is null only for root scope.
                    if (parentNode == null)
                    {
                        _rootScope = (IxScope)node;
                    }

                    if (node == null)
                    {
                        throw new NotSupportedException($"Node with type {nodeConfig.GetType()} is not supported.");
                    }

                    foreach (IxScopeBaseConfig childConfig in nodeConfig.Nodes)
                    {
                        buildNodeAction(node, childConfig);
                    }
                };

            buildNodeAction(null, config);
        }

        private async Task<object> Resolve(IxScopeBase node, Type type, [CanBeNull]string name)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (node.Host != this)
            {
                throw new InvalidOperationException("You cannot use resolver from different host.");
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO: Implement lock validation.
        /// </summary>
        private class FancyLock : IDisposable
        {
            private readonly IxHost _host;

            public FancyLock(IxHost host)
            {
                _host = host;
            }

            public void Dispose()
            {
                Monitor.Exit(this);
            }
        }
    }
}