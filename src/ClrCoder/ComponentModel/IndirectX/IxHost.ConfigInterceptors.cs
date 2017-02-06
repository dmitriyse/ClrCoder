// <copyright file="IxHost.ConfigInterceptors.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using ObjectModel;

    public partial class IxHost
    {
        /// <summary>
        /// Raw instance factory from config builder.
        /// </summary>
        public InterceptableDelegate<RawInstanceFactoryBuilderDelegate> RawInstanceFactoryBuilder =
            new InterceptableDelegate<RawInstanceFactoryBuilderDelegate>(
                config =>
                    {
                        throw new NotSupportedException(
                            $"Raw instance factory with type {config.GetType()} is not supported.");
                    });

        /// <summary>
        /// Creates dependency node from <paramref name="config"/>.
        /// </summary>
        /// <param name="config">Configuration node.</param>
        /// <param name="parentNode">Parent node.</param>
        /// <returns>Created node.</returns>
        public delegate IxProviderNode ProviderNodeBuilderDelegate(
            IxScopeBaseConfig config,
            [CanBeNull] IxProviderNode parentNode);

        public delegate ScopeBinderDelegate ScopeBinderBuilderDelegate(IIxScopeBindingConfig bindingConfig);

        /// <inheritdoc/>
        public Task DisposeTask => _rootScopeInstance.DisposeTask;

        /// <summary>
        /// Visibility filter builder.
        /// </summary>
        public InterceptableDelegate<VisibilityFilterBuilderDelegate> VisibilityFilterBuilder { get; } =
            new InterceptableDelegate<VisibilityFilterBuilderDelegate>(
                config =>
                    {
                        throw new NotSupportedException(
                            $"Visibility filter with type {config.GetType()} is not supported.");
                    });

        public InterceptableDelegate<ProviderNodeBuilderDelegate> ProviderNodeBuilder { get; } =
            new InterceptableDelegate<ProviderNodeBuilderDelegate>(
                (cfg, parent) =>
                    {
                        throw new NotSupportedException($"Node with type {cfg.GetType()} is not supported.");
                    });

        public InterceptableDelegate<ScopeBinderBuilderDelegate> ScopeBinderBuilder { get; } =
            new InterceptableDelegate<ScopeBinderBuilderDelegate>(
                config =>
                    {
                        throw new NotSupportedException($"Scope binder with type {config.GetType()} is not supported.");
                    })
            ;

        private static VisibilityFilterBuilderDelegate StdVisibilityFilterBuilder(
            VisibilityFilterBuilderDelegate next)
        {
            return visibilityConfig =>
                {
                    if (!(visibilityConfig is IxStdVisibilityFilterConfig))
                    {
                        return next(visibilityConfig);
                    }

                    var stdConfig = visibilityConfig as IxStdVisibilityFilterConfig;
                    return id =>
                        {
                            if (stdConfig.WhiteList != null)
                            {
                                if (!stdConfig.WhiteList.Contains(id))
                                {
                                    return false;
                                }
                            }

                            if (stdConfig.BlackList != null)
                            {
                                if (stdConfig.BlackList.Contains(id))
                                {
                                    return false;
                                }
                            }

                            return true;
                        };
                };
        }

        private RawInstanceFactoryBuilderDelegate ClassRawFactoryBuilder(
            RawInstanceFactoryBuilderDelegate next)
        {
            return factoryConfig =>
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
                };
        }

        private RawInstanceFactoryBuilderDelegate ExistingInstanceRawFactoryBuilder(
            RawInstanceFactoryBuilderDelegate next)
        {
            return factoryConfig =>
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
                };
        }

        private ScopeBinderBuilderDelegate RegistrationScopeBinderBuilder(ScopeBinderBuilderDelegate next)
        {
            return config =>
                {
                    if (!(config is IxRegistrationScopeBindingConfig))
                    {
                        return next(config);
                    }

                    return RegistrationScopeBinder;
                };
        }

        private ProviderNodeBuilderDelegate ScopeBuilder(ProviderNodeBuilderDelegate next)
        {
            return (cfg, parentNode) =>
                {
                    if (parentNode == null)
                    {
                        if (cfg.GetType() != typeof(IxHostConfig))
                        {
                            return next(cfg, null);
                        }
                    }
                    else
                    {
                        if (cfg.GetType() != typeof(IxScopeConfig))
                        {
                            return next(cfg, parentNode);
                        }
                    }

                    if (cfg.ExportFilter == null)
                    {
                        cfg.ExportFilter = new IxStdVisibilityFilterConfig();
                    }

                    if (cfg.ExportToParentFilter == null)
                    {
                        cfg.ExportToParentFilter = new IxStdVisibilityFilterConfig
                                                       {
                                                           WhiteList = new HashSet<IxIdentifier>()
                                                       };
                    }

                    if (cfg.ImportFilter == null)
                    {
                        cfg.ImportFilter = new IxStdVisibilityFilterConfig();
                    }

                    VisibilityFilter exportFilter = VisibilityFilterBuilder.Delegate(cfg.ExportFilter);
                    VisibilityFilter importFilter = VisibilityFilterBuilder.Delegate(cfg.ImportFilter);
                    VisibilityFilter exportToParent = VisibilityFilterBuilder.Delegate(cfg.ExportToParentFilter);

                    return new IxScope(
                        this,
                        parentNode,
                        (IxScopeConfig)cfg,
                        exportFilter,
                        exportToParent,
                        importFilter);
                };
        }

        private ProviderNodeBuilderDelegate SingletonProviderBuilder(ProviderNodeBuilderDelegate next)
        {
            return (nodeConfig, parentNode) =>
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

                    RawInstanceFactory rawInstanceFactory = RawInstanceFactoryBuilder.Delegate(cfg.Factory);

                    if (cfg.ExportFilter == null)
                    {
                        throw new InvalidOperationException("Export filter should be defined for provider node.");
                    }

                    if (cfg.ExportToParentFilter == null)
                    {
                        throw new InvalidOperationException(
                            "Export to parent filter should be defined for provider node.");
                    }

                    if (cfg.ImportFilter == null)
                    {
                        throw new InvalidOperationException("Import filter should be defined for provider node.");
                    }

                    VisibilityFilter exportFilter = VisibilityFilterBuilder.Delegate(cfg.ExportFilter);
                    VisibilityFilter exportToParent = VisibilityFilterBuilder.Delegate(cfg.ExportToParentFilter);
                    VisibilityFilter importFilter = VisibilityFilterBuilder.Delegate(cfg.ImportFilter);

                    if (cfg.ScopeBinding == null)
                    {
                        throw new InvalidOperationException("Scope binding should be specified.");
                    }

                    ScopeBinderDelegate scopeBinder = ScopeBinderBuilder.Delegate(cfg.ScopeBinding);

                    return new IxSingletonProvider(
                        this,
                        parentNode,
                        cfg,
                        rawInstanceFactory,
                        exportFilter,
                        exportToParent,
                        importFilter,
                        scopeBinder);
                };
        }
    }
}