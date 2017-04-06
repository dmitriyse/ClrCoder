// <copyright file="IxHost.InstanceBuilders.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Attributes;

    using MoreLinq;

    using Threading;

    /// <content>Instance builders implementations.</content>
    public partial class IxHost
    {
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

                        // instance argument here is half-instantiated instance.
                        return new IxInstanceFactory(
                            (instance, parentInstance, resolveContext, frame) => ResolveList(
                                instance,
                                dependencies,
                                resolveContext,
                                frame,
                                async resolvedDependencies =>
                                    {
                                        try
                                        {
                                            object[] arguments = constructorInfo.GetParameters()
                                                .Select(
                                                    x =>
                                                        resolvedDependencies[new IxIdentifier(x.ParameterType)]
                                                            .Object)
                                                .ToArray();

                                            object instanceObj = constructorInfo.Invoke(arguments);

                                            Critical.Assert(
                                                instanceObj != null,
                                                "Constructor call through reflection should not return null.");

                                            lock (instance.ProviderNode.Host.InstanceTreeSyncRoot)
                                            {
                                                foreach (KeyValuePair<IxIdentifier, IIxInstance> kvp
                                                    in resolvedDependencies)
                                                {
                                                    // ReSharper disable once ObjectCreationAsStatement
                                                    new IxReferenceLock(kvp.Value, instance);
                                                }
                                            }

                                            // Just to avoid multiple functions.
                                            return instanceObj;
                                        }
                                        catch (TargetInvocationException ex)
                                        {
                                            throw ex.InnerException;
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
                            async (instance, parentInstance, resolveContext, frame) => instanceObj,
                            configTypeInfo.GenericTypeArguments[0]);
                    }

                    return next(factoryConfig);
                };
        }

        private InstanceFactoryBuilderDelegate DelegateInstanceBuilder(
            InstanceFactoryBuilderDelegate next)
        {
            return instanceBuilderConfig =>
                {
                    TypeInfo configTypeInfo = instanceBuilderConfig.GetType().GetTypeInfo();

                    var delegateInstanceBuilderConfig = instanceBuilderConfig as IxDelegateInstanceBuilderConfig;
                    if (delegateInstanceBuilderConfig == null)
                    {
                        return next(instanceBuilderConfig);
                    }

                    TypeInfo delegateType = delegateInstanceBuilderConfig.Func.GetType().GetTypeInfo();

                    MethodInfo methodInfo = delegateType.GetDeclaredMethod("Invoke");

                    HashSet<IxIdentifier> dependencies =
                        methodInfo.GetParameters().Select(x => new IxIdentifier(x.ParameterType)).ToHashSet();
                    if (dependencies.Count != methodInfo.GetParameters().Length)
                    {
                        throw new IxConfigurationException(
                            "Multiple parameters with the same type not supported by IxClassRawFactory.");
                    }

                    // TODO: Add tests for this feature
                    List<RequireAttribute> requireAttributes =
                        methodInfo.GetCustomAttributes<RequireAttribute>().ToList();
                    foreach (RequireAttribute requireAttribute in requireAttributes)
                    {
                        if (!dependencies.Add(new IxIdentifier(requireAttribute.Type)))
                        {
                            throw new IxConfigurationException(
                                "Multiple parameters with the same type not supported by IxClassRawFactory.");
                        }
                    }

                    return new IxInstanceFactory(
                        (instance, parentInstance, resolveContext, frame) => ResolveList(
                            instance,
                            dependencies,
                            resolveContext,
                            frame,
                            async resolvedDependencies =>
                                {
                                    object[] arguments = methodInfo.GetParameters()
                                        .Select(
                                            x =>
                                                resolvedDependencies[new IxIdentifier(x.ParameterType)].Object)
                                        .ToArray();

                                    Task instanceObjTask;
                                    try
                                    {
                                        instanceObjTask = (Task)methodInfo.Invoke(
                                            delegateInstanceBuilderConfig.Func,
                                            arguments);

                                        await instanceObjTask;
                                    }
                                    catch (TargetInvocationException ex)
                                    {
                                        throw ex.InnerException;
                                    }

                                    object instanceObj = instanceObjTask.GetResult();

                                    Critical.Assert(
                                        instanceObj != null,
                                        "Constructor call through reflection should not return null.");

                                    lock (instance.ProviderNode.Host.InstanceTreeSyncRoot)
                                    {
                                        foreach (KeyValuePair<IxIdentifier, IIxInstance> kvp
                                            in resolvedDependencies)
                                        {
                                            // ReSharper disable once ObjectCreationAsStatement
                                            new IxReferenceLock(kvp.Value, instance);
                                        }
                                    }

                                    return instanceObj;
                                }),
                        methodInfo.ReturnType.GenericTypeArguments[0]);
                };
        }
    }
}