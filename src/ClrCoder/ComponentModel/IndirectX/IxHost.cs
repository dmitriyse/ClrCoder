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
        /// Initializes a new instance of the <see cref="IxHost"/> class.
        /// This is first build phase. Second phase performed by <see cref="Initialize"/>.
        /// </summary>
        public IxHost()
        {
            // TODO: Load priorities from configs.
            InstanceFactoryBuilder.Add(ExistingInstanceRawFactoryBuilder, 100);
            InstanceFactoryBuilder.Add(ClassInstanceFactoryBuilder, 200);
            InstanceFactoryBuilder.Add(DelegateInstanceBuilder, 300);

            VisibilityFilterBuilder.Add(StdVisibilityFilterBuilder, 100);

            ProviderNodeBuilder.Add(StdProviderConfigDefaultsSetter, 100);
            ProviderNodeBuilder.Add(ScopeBuilder, 200);
            ProviderNodeBuilder.Add(SingletonProviderBuilder, 300);

            ScopeBinderBuilder.Add(RegistrationScopeBinderBuilder, 100);

            DisposeHandlerBuilder.Add(DisposableDisposeHandlerBuilder, 100);
            DisposeHandlerBuilder.Add(AsyncDisposableDisposeHandlerBuilder, 200);

            ResolveHandler.Add(ResolverResolveInterceptor, 100);
            ResolveHandler.Add(SelfToChildrenResolver, 200);
            ResolveHandler.Add(StdResolveInterceptor, 300);
        }

        /// <summary>
        /// Root resolver.
        /// </summary>
        /// <remarks>
        /// TODO: Add initialized assurance.
        /// </remarks>
        [CanBeNull]
        public IIxResolver Resolver { get; private set; }

        /// <summary>
        /// Object that synchronizes instances tree manipulations.
        /// </summary>
        /// <remarks>
        /// Currently there are many cases where control flow have cycles on tree, thus we need centralized <c>lock</c>.
        /// </remarks>
        public object InstanceTreeSyncRoot { get; } = new object();

        /// <inheritdoc/>
        public void StartDispose()
        {
            _rootScopeInstance.StartDispose();
        }

        /// <summary>
        /// Second initialization phase.
        /// </summary>
        /// <param name="config">Host configuration.</param>
        /// <returns>Async execution TPL task.</returns>
        public async Task Initialize(IIxHostConfig config)
        {
            var allConfigNodes = new HashSet<IIxProviderNodeConfig>();

            config.Nodes.Add(
                new IxStdProviderConfig
                    {
                        Identifier = new IxIdentifier(typeof(IIxHost)),
                        InstanceBuilder = new IxExistingInstanceFactoryConfig<IIxHost>(this),
                        DisposeHandler = obj => Task.CompletedTask
                    });

            Action<IIxProviderNodeConfig, IxProviderNode> buildNodeAction = null;
            buildNodeAction = (nodeConfig, parentNode) =>
                {
                    if (!allConfigNodes.Add(nodeConfig))
                    {
                        throw new InvalidOperationException(
                            "Configuration contains the same object multiple times. Currently it's not allowed to avoid visitor cycles.");
                    }

                    IxProviderNode node = ProviderNodeBuilder.Delegate(nodeConfig, parentNode);

                    // parent node is null only for root scope.
                    if (parentNode == null)
                    {
                        _rootScope = (IxScope)node;
                    }

                    foreach (IIxProviderNodeConfig childConfig in nodeConfig.Nodes)
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
                                                          new IxIdentifier(typeof(IIxResolver)),
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

    /// <summary>
    /// Handles instance disposing.
    /// </summary>
    /// <param name="object">Instance <c>object</c> to dispose.</param>
    /// <returns>Async execution TPL task.</returns>
    public delegate Task IxDisposeHandlerDelegate(object @object);

    /// <summary>
    /// Visibility filter.
    /// </summary>
    /// <param name="identifier">Identifier of provider to filter.</param>
    /// <returns><see langword="true"/>, identifier is visible, <see langword="false"/> otherwise.</returns>
    public delegate bool IxVisibilityFilter(IxIdentifier identifier);

    /// <summary>
    /// Scope binder, chooses scope/instance to which created <c>object</c> should belongs.
    /// </summary>
    /// <param name="originInstance">Instance from which resolve is performed.</param>
    /// <param name="resolvePath">Path to provider node that <c>finally</c> resolves instance.</param>
    /// <param name="context">Resolve <c>context</c>.</param>
    /// <param name="resolveBound">Handler that resolves instance when scope resolved.</param>
    /// <returns>Resolved instance temp <c>lock</c>.</returns>
    public delegate Task<IIxInstanceLock> IxScopeBinderDelegate(
        IIxInstance originInstance,
        IxResolvePath resolvePath,
        IxHost.IxResolveContext context,
        IxResolveBoundDelegate resolveBound);

    /// <summary>
    /// Resolves instance when scope resolved.
    /// </summary>
    /// <param name="parentInstance">Scope/parent instance.</param>
    /// <param name="provider">Provider node that should resolve instance.</param>
    /// <param name="context">Resolve <c>context</c>.</param>
    /// <returns>Resolved instance temp <c>lock</c>.</returns>
    public delegate Task<IIxInstanceLock> IxResolveBoundDelegate(
        IIxInstance parentInstance,
        IxProviderNode provider,
        IxHost.IxResolveContext context);

    /// <summary>
    /// Creates dependency node from <paramref name="config"/>.
    /// </summary>
    /// <param name="config">Configuration node.</param>
    /// <returns>Created node.</returns>
    public delegate IxInstanceFactory InstanceFactoryBuilderDelegate(
        IIxInstanceBuilderConfig config);

    /// <summary>
    /// Builds instance <c>object</c> disposing handler.
    /// </summary>
    /// <param name="type">Type of instance.</param>
    /// <returns>Dispose handler.</returns>
    public delegate IxDisposeHandlerDelegate DisposeHandlerBuilderDelegate([CanBeNull] Type type);

    /// <summary>
    /// Builds visibility filter.
    /// </summary>
    /// <param name="config">Visibility filter <c>config</c>.</param>
    /// <returns>Visibility filter.</returns>
    public delegate IxVisibilityFilter VisibilityFilterBuilderDelegate(IIxVisibilityFilterConfig config);

    /// <summary>
    /// Builds scope binder.
    /// </summary>
    /// <param name="bindingConfig">Bining config.</param>
    /// <returns>Scope binder.</returns>
    public delegate IxScopeBinderDelegate ScopeBinderBuilderDelegate(IIxScopeBindingConfig bindingConfig);
}