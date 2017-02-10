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
            RawInstanceFactoryBuilder.Add(ClassRawFactoryBuilder, 100);

            VisibilityFilterBuilder.Add(StdVisibilityFilterBuilder, 100);

            ProviderNodeBuilder.Add(StdProviderConfigDefaultsSetter, 100);
            ProviderNodeBuilder.Add(ScopeBuilder, 200);
            ProviderNodeBuilder.Add(SingletonProviderBuilder, 300);

            ScopeBinderBuilder.Add(RegistrationScopeBinderBuilder, 100);

            DisposeHandlerBuilder.Add(DisposableDisposeHandlerBuilder, 100);

            ResolveHandler.Add(ResolverResolveInterceptor, 100);
            ResolveHandler.Add(SelfToDirectChildrenResolver, 200);
            ResolveHandler.Add(StdResolveInterceptor, 300);
        }

        /// <summary>
        /// Creates dependency node from <paramref name="config"/>.
        /// </summary>
        /// <param name="config">Configuration node.</param>
        /// <returns>Created node.</returns>
        public delegate IxInstanceFactory RawInstanceFactoryBuilderDelegate(
            IIxInstanceBuilderConfig config);

        public delegate IxDisposeHandlerDelegate DisposeHandlerBuilderDelegate([CanBeNull] Type type);

        public delegate IxVisibilityFilter VisibilityFilterBuilderDelegate(IIxVisibilityFilterConfig config);

        [CanBeNull]
        public IIxResolver Resolver { get; private set; }

        public object InstanceTreeSyncRoot { get; } = new object();

        /// <inheritdoc/>
        public void StartDispose()
        {
            _rootScopeInstance.StartDispose();
        }

        public async Task Initialize(IxHostConfig config)
        {
            var allConfigNodes = new HashSet<IxScopeBaseConfig>();

            config.Nodes.Add(
                new IxStdProviderConfig
                    {
                        Identifier = new IxIdentifier(typeof(IIxHost)),
                        Factory = new IxExistingInstanceFactoryConfig<IIxHost>(this)
                    });

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

                        // Applying export to parent filter 
                        // while Blocking from exporting resolve pathes with zero-length.
                        foreach (KeyValuePair<IxIdentifier, IxResolvePath> kvp in
                            child.VisibleNodes.Where(
                                x => child.ExportToParentFilter(x.Key)
                                     && x.Value.Path.Any()))
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
            using (IIxInstanceLock rootResolverLock = await Resolve(
                                                          _rootScopeInstance,
                                                          new IxIdentifier(typeof(IIxResolver), null),
                                                          resolveContext))
            {
                Resolver = (IIxResolver)rootResolverLock.Target.Object;
            }
        }

        private async Task<IIxInstanceLock> Resolve(
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

            if (originInstance.ProviderNode.Host != this)
            {
                throw new InvalidOperationException("You cannot use resolver from different host.");
            }

            if (context.IsFailed)
            {
                throw new InvalidOperationException("You cannot do anything inside failed resolve context.");
            }

            return await ResolveHandler.Delegate(originInstance, identifier, context);
        }

        /// <inheritdoc/>
        public Task DisposeTask => _rootScopeInstance.DisposeTask;
    }

    public delegate Task IxDisposeHandlerDelegate(object @object);

    public delegate bool IxVisibilityFilter(IxIdentifier identifier);

    public delegate Task<IIxInstanceLock> IxScopeBinderDelegate(
        IIxInstance originInstance,
        IxResolvePath resolvePath,
        IxHost.IxResolveContext context,
        IxResolveBoundDelegate resolveBound);

    public delegate Task<IIxInstanceLock> IxResolveBoundDelegate(
        IIxInstance parentInstance,
        IxProviderNode provider,
        IxHost.IxResolveContext context);
}