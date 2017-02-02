// <copyright file="IxHost.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using ObjectModel;

    /// <summary>
    /// IndirectX host.
    /// </summary>
    public partial class IxHost : IIxHost
    {
        private readonly Dictionary<Func<CreateNodeDelegate, CreateNodeDelegate>, int> _createNodeInterceptors =
            new Dictionary<Func<CreateNodeDelegate, CreateNodeDelegate>, int>();

        private readonly Dictionary<Func<CreateRawInstanceFactoryDelegate, CreateRawInstanceFactoryDelegate>, int>
            _createRawInstanceFactoryInterceptors =
                new Dictionary<Func<CreateRawInstanceFactoryDelegate, CreateRawInstanceFactoryDelegate>, int>();

        private IxScope _rootScope;

        private IxScopeInstance _rootScopeInstance;

        /// <summary>
        /// Creates new host.
        /// </summary>
        public IxHost()
        {
            // TODO: Load priorities from configs.

            _createRawInstanceFactoryInterceptors.Add(
                next => factoryConfig =>
                    {
                        TypeInfo configTypeInfo = factoryConfig.GetType().GetTypeInfo();

                        if (configTypeInfo.IsGenericType
                            && configTypeInfo.GetGenericTypeDefinition() == typeof(IxExistingInstanceFactoryConfig<>))
                        {
                            object instance = configTypeInfo.GetDeclaredProperty("Instance").GetValue(factoryConfig);
                            if (instance == null)
                            {
                                throw new InvalidOperationException(
                                    "Existing instance factory config should have not null instance.");
                            }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                            return async (parentValue, resolveContext) => instance;
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                        }

                        return next(factoryConfig);
                    },
                100);

            CreateRawInstanceFactoryDelegate createRawInstanceFactory = _createRawInstanceFactoryInterceptors
                .OrderBy(x => x.Value)
                .Select(x => x.Key)
                .BuildInterceptorsChain(
                    factoryConfig =>
                    {
                        throw new NotSupportedException(
                            $"Node with type {factoryConfig.GetType()} is not supported.");
                    });

            // Registering IxScope creator interceptor.
            AddCreateNodeInterceptor(
                next => (nodeConfig, parentNode) =>
                    {
                        if (parentNode == null)
                        {
                            if (nodeConfig.GetType() != typeof(IxHostConfig))
                            {
                                return next(nodeConfig, null);
                            }
                        }
                        else
                        {
                            if (nodeConfig.GetType() != typeof(IxScopeConfig))
                            {
                                return next(nodeConfig, parentNode);
                            }
                        }

                        return new IxScope(this, parentNode, (IxScopeConfig)nodeConfig);
                    },
                100);

            // Registering IxInstanceProvider creator interceptor.
            AddCreateNodeInterceptor(
                next => (nodeConfig, parentNode) =>
                    {
                        if (nodeConfig.GetType() != typeof(IxStdProviderConfig))
                        {
                            return next(nodeConfig, parentNode);
                        }

                        if (parentNode == null)
                        {
                            throw new InvalidOperationException("Instance provider cannot be used as root scope.");
                        }

                        var cfg = (IxStdProviderConfig)nodeConfig;

                        if (cfg.Multiplicity == null)
                        {
                            // TODO: Replace to configuration validation exception.
                            throw new InvalidOperationException("Multiplicity should be specified.");
                        }

                        if (!(cfg.Multiplicity is IxSingletonMultiplicityConfig))
                        {
                            return next(nodeConfig, parentNode);
                        }

                        if (cfg.Factory == null)
                        {
                            throw new InvalidOperationException("Instance factory should be configured.");
                        }

                        RawInstanceFactory rawInstanceFactory = createRawInstanceFactory(cfg.Factory);

                        return new IxSingletonProvider(this, parentNode, cfg, rawInstanceFactory);
                    },
                101);

            //AddProvideInstanceInterceptor(
            //    next => (scope, identifier, context) =>
            //        {
            //            if (!(scope is IxProviderBase))
            //            {
            //                throw new InvalidOperationException("Only providers can inject resolver.");
            //            }

            //            return new IxResolver(this, scope, context);
            //        },
            //    100);
        }

        /// <summary>
        /// Creates dependency node from <paramref name="config"/>.
        /// </summary>
        /// <param name="config">Configuration node.</param>
        /// <param name="parentNode">Parent node.</param>
        /// <returns>Created node.</returns>
        public delegate IxProviderNode CreateNodeDelegate(
            IxScopeBaseConfig config,
            [CanBeNull] IxProviderNode parentNode);

        /// <summary>
        /// Creates dependency node from <paramref name="config"/>.
        /// </summary>
        /// <param name="config">Configuration node.</param>
        /// <returns>Created node.</returns>
        public delegate RawInstanceFactory CreateRawInstanceFactoryDelegate(
            IIxFactoryConfig config);

        /// <summary>
        /// Raw instance factory <c>delegate</c>. No any registrations just obtain instance according to config.
        /// </summary>
        /// <param name="parentInstance">Parent instance.</param>
        /// <param name="context"><c>Resolve</c> <c>context</c>.</param>
        /// <returns>Create instance.</returns>
        public delegate Task<object> RawInstanceFactory(IIxInstance parentInstance, IxResolveContext context);

        [CanBeNull]
        public IIxResolver Resolver { get; private set; }

        /// <inheritdoc/>
        public async Task AsyncDispose()
        {
            // Do Nothing.
        }

        /// <summary>
        /// Registers node creation interceptor.
        /// </summary>
        /// <param name="interceptorBuilder">Interceptor builder.</param>
        /// <param name="priority">Interceptor <c>priority</c> (interceptor with gratest <c>priority</c> will intercept firstly).</param>
        public void AddCreateNodeInterceptor(
            Func<CreateNodeDelegate, CreateNodeDelegate> interceptorBuilder,
            int priority)
        {
            if (interceptorBuilder == null)
            {
                throw new ArgumentNullException(nameof(interceptorBuilder));
            }

            _createNodeInterceptors.Add(interceptorBuilder, priority);
        }

        public async Task Initialize(IxHostConfig config)
        {
            CreateNodeDelegate createNodeFromConfig = _createNodeInterceptors
                .OrderBy(x => x.Value)
                .Select(x => x.Key)
                .BuildInterceptorsChain(
                    (cfg, parent) =>
                        {
                            throw new NotSupportedException($"Node with type {cfg.GetType()} is not supported.");
                        });

            var allConfigNodes = new HashSet<IxScopeBaseConfig>();
            Action<IxScopeBaseConfig, IxProviderNode> buildNodeAction = null;
            buildNodeAction = (nodeConfig, parentNode) =>
                {
                    if (!allConfigNodes.Add(nodeConfig))
                    {
                        throw new InvalidOperationException(
                            "Configuration contains the same object multiple times. Currently it's not allowed to avoid visitor cycles.");
                    }

                    IxProviderNode node = createNodeFromConfig(nodeConfig, parentNode);

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

            _rootScopeInstance = _rootScope.GetRootInstance();
            var resolveContext = new IxResolveContext(null);
            Resolver =
                (IIxResolver)
                (await Resolve(_rootScopeInstance, new IxIdentifier(typeof(IIxResolver), null), resolveContext))
                .Object;
        }

        private async Task<IIxInstance> Resolve(
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

            // TODO: Extension point here!
            if (identifier.Type == typeof(IIxResolver))
            {
                if (identifier.Name != null)
                {
                    throw new InvalidOperationException("IIxResolver cannot be queried with name.");
                }

                if (originInstance.Resolver == null)
                {
                    lock (originInstance)
                    {
                        if (originInstance.Resolver == null)
                        {
                            originInstance.Resolver = new IxResolver(this, originInstance, context);
                        }
                    }
                }

                return originInstance.Resolver;
            }

            // TODO: Search extension point here!
            // TODO: Search algorithm.

            IxProviderNode resolvePerformerNode;
            IIxInstance resolvePerformerInstance = /*TODO: valid search*/ originInstance;

            if (originInstance.ProviderNode.NodesById.TryGetValue(identifier, out resolvePerformerNode))
            {
                await resolvePerformerNode.GetInstance(resolvePerformerInstance, context);
            }

            throw new NotImplementedException();
        }
    }
}