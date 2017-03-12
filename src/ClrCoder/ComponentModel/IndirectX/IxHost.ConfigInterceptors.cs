﻿// <copyright file="IxHost.ConfigInterceptors.cs" company="ClrCoder project">
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

    using Attributes;

    using JetBrains.Annotations;

    using MoreLinq;

    using ObjectModel;

    using Threading;

    /// <content>Configuration loading related routines.</content>
    [NoReorder]
    public partial class IxHost
    {
        /// <summary>
        /// Instance factory from config builder.
        /// </summary>
        public InterceptableDelegate<InstanceFactoryBuilderDelegate> InstanceFactoryBuilder { get; } =
            new InterceptableDelegate<InstanceFactoryBuilderDelegate>(
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
            IIxProviderNodeConfig config,
            [CanBeNull] IxProviderNode parentNode);

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

        /// <summary>
        /// Interceptors chain that builds node provider.
        /// </summary>
        public InterceptableDelegate<ProviderNodeBuilderDelegate> ProviderNodeBuilder { get; } =
            new InterceptableDelegate<ProviderNodeBuilderDelegate>(
                (cfg, parent) =>
                    {
                        throw new NotSupportedException($"Node with type {cfg.GetType()} is not supported.");
                    });

        /// <summary>
        /// Interceptors chain that builds scope binder.
        /// </summary>
        public InterceptableDelegate<ScopeBinderBuilderDelegate> ScopeBinderBuilder { get; } =
            new InterceptableDelegate<ScopeBinderBuilderDelegate>(
                config =>
                    {
                        throw new NotSupportedException($"Scope binder with type {config.GetType()} is not supported.");
                    })
            ;

        /// <summary>
        /// Interceptors chain that builds dispose handler.
        /// </summary>
        public InterceptableDelegate<DisposeHandlerBuilderDelegate> DisposeHandlerBuilder { get; } =
            new InterceptableDelegate<DisposeHandlerBuilderDelegate>(
                type =>
                    {
                        if (type == null)
                        {
                            return obj =>
                                {
                                    try
                                    {
                                        Critical.Assert(obj != null, "Dispose handler called on not null object.");
                                    }
                                    catch (Exception ex)
                                    {
                                        return Task.FromException(ex);
                                    }

                                    return Task.CompletedTask;
                                };
                        }

                        // By default no any dispose action required on object.
                        return obj => Task.CompletedTask;
                    });

        #region Provider Node Builders

        private ProviderNodeBuilderDelegate StdProviderConfigDefaultsSetter(ProviderNodeBuilderDelegate next)
        {
            return (nodeConfig, parentNode) =>
                {
                    TypeInfo nodeConfigType = nodeConfig.GetType().GetTypeInfo();
                    if (!typeof(IIxStdProviderConfig).GetTypeInfo().IsAssignableFrom(nodeConfigType))
                    {
                        return next(nodeConfig, parentNode);
                    }

                    var cfg = nodeConfig as IxStdProviderConfig;
                    var cfgContract = (IIxStdProviderConfig)nodeConfig;
                    if (cfg == null || nodeConfig.GetType() != typeof(IxStdProviderConfig))
                    {
                        cfg = new IxStdProviderConfig
                                  {
                                      ScopeBinding = cfgContract.ScopeBinding,
                                      Identifier = cfgContract.Identifier,
                                      Factory = cfgContract.Factory,
                                      Multiplicity = cfgContract.Multiplicity,
                                      DisposeHandler = cfgContract.DisposeHandler,
                                      ExportFilter = cfgContract.ExportFilter,
                                      ExportToParentFilter = cfgContract.ExportToParentFilter,
                                      ImportFilter = cfgContract.ImportFilter
                                  };
                    }

                    // TODO: Write test on this feature.
                    if (cfgContract.Identifier == null && nodeConfig is IIxBasicIdentificationConfig)
                    {
                        var basicIdentificationConfig = nodeConfig as IIxBasicIdentificationConfig;
                        if (basicIdentificationConfig.ContractType != null)
                        {
                            cfg.Identifier = new IxIdentifier(
                                basicIdentificationConfig.ContractType,
                                basicIdentificationConfig.Name);
                        }
                    }

                    IIxProviderNodeConfig configProviderConfig = null;

                    if (nodeConfigType.GetCustomAttribute<ProvideConfigAttribute>() != null)
                    {
                        configProviderConfig = new IxStdProviderConfig
                                                   {
                                                       Identifier = new IxIdentifier(nodeConfig.GetType()),
                                                       Factory = new IxExistingInstanceFactoryConfig<object>(nodeConfig),
                                                   };
                    }

                    if (cfg.Multiplicity == null)
                    {
                        cfg.Multiplicity = new IxSingletonMultiplicityConfig();
                    }

                    // By default allow exporting to children.
                    if (cfg.ExportFilter == null)
                    {
                        cfg.ExportFilter = new IxStdVisibilityFilterConfig();
                    }

                    // By default allow importing all from parent.
                    if (cfg.ImportFilter == null)
                    {
                        cfg.ImportFilter = new IxStdVisibilityFilterConfig();
                    }

                    // By default exporting nothing to parent.
                    if (cfg.ExportToParentFilter == null)
                    {
                        cfg.ExportToParentFilter = new IxStdVisibilityFilterConfig
                                                       {
                                                           WhiteList = new HashSet<IxIdentifier>()
                                                       };
                    }

                    // Binding to registration by default.
                    if (cfg.ScopeBinding == null)
                    {
                        cfg.ScopeBinding = new IxRegistrationScopeBindingConfig();
                    }

                    IxProviderNode result = next(cfg, parentNode);
                    if (configProviderConfig != null)
                    {
                        ProviderNodeBuilder.Delegate(configProviderConfig, result);
                    }

                    return result;
                };
        }

        #endregion

        private InstanceFactoryBuilderDelegate ClassInstanceFactoryBuilder(
            InstanceFactoryBuilderDelegate next)
        {
            return factoryConfig =>
                {
                    TypeInfo configTypeInfo = factoryConfig.GetType().GetTypeInfo();

                    if (configTypeInfo.IsGenericType
                        && configTypeInfo.GetGenericTypeDefinition() == typeof(IxClassInstanceBuilderConfig<>))
                    {
                        TypeInfo instanceClass = configTypeInfo.GenericTypeArguments[0].GetTypeInfo();
                        ConstructorInfo[] constructors =
                            instanceClass.DeclaredConstructors.Where(x => !x.IsStatic).ToArray();
                        if (constructors.Length == 0)
                        {
                            // Currently impossible case.
                            throw new IxConfigurationException(
                                $"Cannot use IxClassInstanceFactory because no any constructors found in the class {instanceClass.FullName}.");
                        }

                        if (constructors.Length > 1)
                        {
                            throw new IxConfigurationException(
                                $"Cannot use IxClassInstanceFactory because more than one constructors defined in the class {instanceClass.FullName}.");
                        }

                        ConstructorInfo constructorInfo = constructors.Single();

                        HashSet<IxIdentifier> dependencies =
                            constructorInfo.GetParameters().Select(x => new IxIdentifier(x.ParameterType)).ToHashSet();
                        if (dependencies.Count != constructorInfo.GetParameters().Length)
                        {
                            throw new IxConfigurationException(
                                "Multiple parameters with the same type not supported by IxClassRawFactory.");
                        }

                        // TODO: Add tests for this feature
                        List<RequireAttribute> requireAttributes =
                            constructorInfo.GetCustomAttributes<RequireAttribute>().ToList();
                        foreach (RequireAttribute requireAttribute in requireAttributes)
                        {
                            if (!dependencies.Add(new IxIdentifier(requireAttribute.Type)))
                            {
                                throw new IxConfigurationException(
                                    "Multiple parameters with the same type not supported by IxClassRawFactory.");
                            }
                        }

                        return new IxInstanceFactory(
                            (instance, parentInstance, resolveContext) => ResolveList(
                                instance,
                                dependencies,
                                resolveContext,
                                resolvedDependencies =>
                                    {
                                        try
                                        {
                                            object[] arguments = constructorInfo.GetParameters()
                                                .Select(
                                                    x =>
                                                        resolvedDependencies[new IxIdentifier(x.ParameterType)].Object)
                                                .ToArray();

                                            object instanceObj = constructorInfo.Invoke(arguments);

                                            Critical.Assert(
                                                instanceObj != null,
                                                "Constructor call through reflection should not return null.");

                                            lock (instance.ProviderNode.Host.InstanceTreeSyncRoot)
                                            {
                                                instance.Object = instanceObj;

                                                foreach (KeyValuePair<IxIdentifier, IIxInstance> kvp
                                                    in resolvedDependencies)
                                                {
                                                    // ReSharper disable once ObjectCreationAsStatement
                                                    new IxReferenceLock(kvp.Value, instance);
                                                }
                                            }

                                            // Result true or false does not make sence, it's just to avoid of creation parameterless ResolveList method.
                                            return Task.FromResult(true);
                                        }
                                        catch (Exception ex)
                                        {
                                            return Task.FromException<bool>(ex);
                                        }
                                    }),
                            configTypeInfo.GenericTypeArguments[0]);
                    }

                    return next(factoryConfig);
                };
        }

        private InstanceFactoryBuilderDelegate ExistingInstanceRawFactoryBuilder(
            InstanceFactoryBuilderDelegate next)
        {
            return factoryConfig =>
                {
                    TypeInfo configTypeInfo = factoryConfig.GetType().GetTypeInfo();

                    if (configTypeInfo.IsGenericType
                        && configTypeInfo.GetGenericTypeDefinition() == typeof(IxExistingInstanceFactoryConfig<>))
                    {
                        object instanceObj = configTypeInfo.GetDeclaredProperty("Instance").GetValue(factoryConfig);
                        if (instanceObj == null)
                        {
                            throw new InvalidOperationException(
                                "Existing instance factory config should have not null instance.");
                        }

                        return new IxInstanceFactory(
                            (instance, parentInstance, resolveContext) =>
                                {
                                    instance.Object = instanceObj;
                                    return Task.CompletedTask;
                                },
                            configTypeInfo.GenericTypeArguments[0]);
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
            return (nodeConfig, parentNode) =>
                {
                    TypeInfo configType = nodeConfig.GetType().GetTypeInfo();
                    if (parentNode == null)
                    {
                        if (!typeof(IIxHostConfig).GetTypeInfo().IsAssignableFrom(configType))
                        {
                            return next(nodeConfig, null);
                        }
                    }
                    else
                    {
                        if (!typeof(IIxScopeConfig).GetTypeInfo().IsAssignableFrom(configType))
                        {
                            return next(nodeConfig, parentNode);
                        }
                    }

                    var cfg = nodeConfig as IxScopeConfig;
                    var cfgContract = (IIxScopeConfig)nodeConfig;
                    if (cfg == null)
                    {
                        cfg = new IxScopeConfig
                                  {
                                      Identifier = cfgContract.Identifier,
                                      ExportFilter = cfgContract.ExportFilter,
                                      ExportToParentFilter = cfgContract.ExportToParentFilter,
                                      ImportFilter = cfgContract.ImportFilter,
                                      IsInstanceless = cfgContract.IsInstanceless
                                  };
                    }

                    if (cfg.Identifier == null)
                    {
                        cfg.Identifier = new IxIdentifier(typeof(IxScope));
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

                    IxVisibilityFilter exportFilter = VisibilityFilterBuilder.Delegate(cfg.ExportFilter);
                    IxVisibilityFilter importFilter = VisibilityFilterBuilder.Delegate(cfg.ImportFilter);
                    IxVisibilityFilter exportToParent = VisibilityFilterBuilder.Delegate(cfg.ExportToParentFilter);

                    return new IxScope(
                        this,
                        parentNode,
                        cfg,
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

                    IxInstanceFactory instanceFactory = InstanceFactoryBuilder.Delegate(cfg.Factory);

                    if (cfg.DisposeHandler == null)
                    {
                        cfg.DisposeHandler = DisposeHandlerBuilder.Delegate(instanceFactory.InstanceBaseType);
                    }

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

                    IxVisibilityFilter exportFilter = VisibilityFilterBuilder.Delegate(cfg.ExportFilter);
                    IxVisibilityFilter exportToParent = VisibilityFilterBuilder.Delegate(cfg.ExportToParentFilter);
                    IxVisibilityFilter importFilter = VisibilityFilterBuilder.Delegate(cfg.ImportFilter);

                    if (cfg.ScopeBinding == null)
                    {
                        throw new InvalidOperationException("Scope binding should be specified.");
                    }

                    if (cfg.DisposeHandler == null)
                    {
                        throw new InvalidOperationException("Dispose handler should be specified.");
                    }

                    IxScopeBinderDelegate scopeBinder = ScopeBinderBuilder.Delegate(cfg.ScopeBinding);

                    return new IxSingletonProvider(
                        this,
                        parentNode,
                        cfg,
                        instanceFactory,
                        exportFilter,
                        exportToParent,
                        importFilter,
                        scopeBinder,
                        cfg.DisposeHandler);
                };
        }

        private VisibilityFilterBuilderDelegate StdVisibilityFilterBuilder(
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

        #region Dispose handler builders

        private DisposeHandlerBuilderDelegate DisposableDisposeHandlerBuilder(DisposeHandlerBuilderDelegate next)
        {
            return type =>
                {
                    if (type == null)
                    {
                        IxDisposeHandlerDelegate nextHandler = next(null);
                        return obj =>
                            {
                                var disposable = obj as IDisposable;
                                if (disposable != null)
                                {
                                    try
                                    {
                                        disposable.Dispose();
                                    }
                                    catch (Exception ex)
                                    {
                                        if (!ex.IsProcessable())
                                        {
                                            return Task.FromException(ex);
                                        }
                                    }

                                    return Task.CompletedTask;
                                }

                                return nextHandler(obj);
                            };
                    }

                    if (!typeof(IDisposable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                    {
                        return next(type);
                    }

                    return obj =>
                        {
                            try
                            {
                                Critical.Assert(obj != null, "Dispose handler called for null object");
                                ((IDisposable)obj).Dispose();
                            }
                            catch (Exception ex)
                            {
                                if (!ex.IsProcessable())
                                {
                                    return Task.FromException(ex);
                                }
                            }

                            return Task.CompletedTask;
                        };
                };
        }

        private DisposeHandlerBuilderDelegate AsyncDisposableDisposeHandlerBuilder(DisposeHandlerBuilderDelegate next)
        {
            return type =>
                {
                    if (type == null)
                    {
                        IxDisposeHandlerDelegate nextHandler = next(null);
                        return obj =>
                            {
                                var disposable = obj as IAsyncDisposable;
                                if (disposable != null)
                                {
                                    try
                                    {
                                        disposable.StartDispose();
                                    }
                                    catch (Exception ex)
                                    {
                                        if (!ex.IsProcessable())
                                        {
                                            return Task.FromException(ex);
                                        }
                                    }

                                    return Task.CompletedTask;
                                }

                                return nextHandler(obj);
                            };
                    }

                    if (!typeof(IAsyncDisposable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                    {
                        return next(type);
                    }

                    return obj =>
                        {
                            try
                            {
                                Critical.Assert(obj != null, "Dispose handler called for null object");
                                ((IAsyncDisposable)obj).StartDispose();
                            }
                            catch (Exception ex)
                            {
                                if (!ex.IsProcessable())
                                {
                                    return Task.FromException(ex);
                                }
                            }

                            return Task.CompletedTask;
                        };
                };
        }

        #endregion
    }
}