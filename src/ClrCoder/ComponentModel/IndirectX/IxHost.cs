// <copyright file="IxHost.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// IndirectX host.
    /// </summary>
    public partial class IxHost : IIxHost
    {
        private IxScope _rootScope;

        private IxScopeInstance _rootScopeInstance;

        /// <summary>
        /// Creates new host.
        /// </summary>
        public IxHost()
        {
            // TODO: Load priorities from configs.
            RawInstanceFactoryBuilder.Add(ExistingInstanceRawFactoryBuilder, 100);

            VisibilityFilterBuilder.Add(StdVisibilityFilterBuilder, 100);

            ProviderNodeBuilder.Add(ScopeBuilder, 100);
            ProviderNodeBuilder.Add(SingletonProviderBuilder, 200);

            ScopeBinderBuilder.Add(RegistrationScopeBinderBuilder, 100);

            ResolveHandler.Add(ResolverResolveInterceptor, 100);
            ResolveHandler.Add(StdResolveInterceptor, 200);
        }

        /// <summary>
        /// Raw instance factory <c>delegate</c>. No any registrations just obtain instance according to config.
        /// </summary>
        /// <param name="parentInstance">Parent instance.</param>
        /// <param name="context"><c>Resolve</c> <c>context</c>.</param>
        /// <returns>Create instance.</returns>
        public delegate Task<object> RawInstanceFactory(IIxInstance parentInstance, IxResolveContext context);

        /// <summary>
        /// Creates dependency node from <paramref name="config"/>.
        /// </summary>
        /// <param name="config">Configuration node.</param>
        /// <returns>Created node.</returns>
        public delegate RawInstanceFactory RawInstanceFactoryBuilderDelegate(
            IIxFactoryConfig config);

        public delegate Task<IIxInstance> ResolveBoundDelegate(
            IIxInstance parentInstance,
            IxProviderNode provider,
            IxResolveContext context);

        public delegate Task<IIxInstance> ScopeBinderDelegate(
            IIxInstance originInstance,
            IxResolvePath resolvePath,
            IxResolveContext context,
            ResolveBoundDelegate resolveBound);

        public delegate bool VisibilityFilter(IxIdentifier identifier);

        public delegate VisibilityFilter VisibilityFilterBuilderDelegate(IIxVisibilityFilterConfig config);

        [CanBeNull]
        public IIxResolver Resolver { get; private set; }

        public object InstanceTreeSyncRoot { get; } = new object();

        /// <inheritdoc/>
        public async Task AsyncDispose()
        {
            // Do Nothing.
        }

        public async Task Initialize(IxHostConfig config)
        {
            var allConfigNodes = new HashSet<IxScopeBaseConfig>();

            Action<IxScopeBaseConfig, IxProviderNode> buildNodeAction = null;
            buildNodeAction = (nodeConfig, parentNode) =>
                {
                    if (!allConfigNodes.Add(nodeConfig))
                    {
                        throw new InvalidOperationException(
                            "Configuration contains the same object multiple times. Currently it's not allowed to avoid visitor cycles.");
                    }

                    IxProviderNode node = ProviderNodeBuilder.Delegate(nodeConfig, parentNode);

                    // parent node is null only for root scope.
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (parentNode == null)
                    {
                        _rootScope = (IxScope)node;
                    }

                    foreach (IxScopeBaseConfig childConfig in nodeConfig.Nodes)
                    {
                        buildNodeAction(childConfig, node);
                    }
                };

            buildNodeAction(config, null);

            Action<IxProviderNode> importRegistrationsFromChildren = null;

            importRegistrationsFromChildren = node =>
                {
                    foreach (IxProviderNode child in node.Nodes)
                    {
                        importRegistrationsFromChildren(child);

                        foreach (KeyValuePair<IxIdentifier, IxResolvePath> kvp in
                            child.VisibleNodes.Where(x => child.ExportToParentFilter(x.Key)))
                        {
                            if (node.VisibleNodes.ContainsKey(kvp.Key))
                            {
                                throw new InvalidOperationException("Export to parent node conflict.");
                            }

                            node.VisibleNodes.Add(kvp.Key, kvp.Value.ReRoot(node));
                        }
                    }
                };

            importRegistrationsFromChildren(_rootScope);

            Action<IxProviderNode> exportRegistrationsToChildren = null;
            exportRegistrationsToChildren = node =>
                {
                    KeyValuePair<IxIdentifier, IxResolvePath>[] registrationsToExport =
                        node.VisibleNodes.Where(x => node.ExportFilter(x.Key)).ToArray();
                    if (!registrationsToExport.Any())
                    {
                        return;
                    }

                    foreach (IxProviderNode child in node.Nodes)
                    {
                        foreach (
                            KeyValuePair<IxIdentifier, IxResolvePath> kvp in
                            registrationsToExport.Where(x => child.ImportFilter(x.Key)))
                        {
                            if (!child.VisibleNodes.ContainsKey(kvp.Key))
                            {
                                child.VisibleNodes.Add(kvp.Key, kvp.Value);
                            }
                        }

                        exportRegistrationsToChildren(child);
                    }
                };

            exportRegistrationsToChildren(_rootScope);

            _rootScopeInstance = _rootScope.GetRootInstance();
            var resolveContext = new IxResolveContext(null);
            Resolver =
                (IIxResolver)
                (await Resolve(_rootScopeInstance, new IxIdentifier(typeof(IIxResolver), null), resolveContext))
                .Object;
        }

        private Task<IIxInstance> Resolve(
            IIxInstance originInstance,
            IxIdentifier identifier,
            IxResolveContext context)
        {
            if (originInstance == null)
            {
                throw new ArgumentNullException(nameof(originInstance));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (originInstance.Host != this)
            {
                throw new InvalidOperationException("You cannot use resolver from different host.");
            }

            if (context.IsFailed)
            {
                throw new InvalidOperationException("You cannot do anything inside failed resolve context.");
            }

            return ResolveHandler.Delegate(originInstance, identifier, context);
        }
    }
}