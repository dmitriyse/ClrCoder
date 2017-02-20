// <copyright file="IxHost.ResolveInterceptors.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using ObjectModel;

    public partial class IxHost
    {
        public InterceptableDelegate<ResolveDelegate> ResolveHandler = new InterceptableDelegate<ResolveDelegate>(
            (originInstance, identifier, context) =>
                {
                    throw new IxResolveTargetNotFound(
                        $"No any rule found to resolve {identifier.Type}|{identifier.Name} dependency.",
                        identifier);
                });

        public delegate Task<IIxInstanceLock> ResolveDelegate(
            IIxInstance originInstance,
            IxIdentifier identifier,
            IxResolveContext context);

        /// <summary>
        /// Resolves list of <c>dependencies</c> from the specified origin. TODO: Implement <c>this</c> method with parallel
        /// resolution.
        /// </summary>
        /// <typeparam name="TResult">Target operation result type.</typeparam>
        /// <param name="originInstance">Origin instance. Where <c>dependencies</c> are queried.</param>
        /// <param name="dependencies">List of dependency identifiers.</param>
        /// <param name="context">Resolve <c>context</c>.</param>
        /// <param name="targetOperation">Operation that should be performed with resolved <c>dependencies</c>.</param>
        /// <returns>Result of target opration.</returns>
        public async Task<TResult> ResolveList<TResult>(
            IIxInstance originInstance,
            HashSet<IxIdentifier> dependencies,
            IxResolveContext context,
            Func<Dictionary<IxIdentifier, IIxInstance>, Task<TResult>> targetOperation)
        {
            if (originInstance == null)
            {
                throw new ArgumentNullException(nameof(originInstance));
            }

            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (targetOperation == null)
            {
                throw new ArgumentNullException(nameof(targetOperation));
            }

            using (HashSet<IxIdentifier>.Enumerator enumerator = dependencies.GetEnumerator())
            {
                var result = new Dictionary<IxIdentifier, IIxInstance>();
                Func<Task<TResult>> resolveItem = null;
                resolveItem = async () =>
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        if (!enumerator.MoveNext())
                        {
                            return await targetOperation(result);
                        }

                        // ReSharper disable once AccessToDisposedClosure
                        using (IIxInstanceLock instanceLock = await Resolve(originInstance, enumerator.Current, context)
                        )
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            result.Add(enumerator.Current, instanceLock.Target);
                            return await resolveItem();
                        }
                    };

                return await resolveItem();
            }
        }

        private async Task<IIxInstanceLock> RegistrationScopeBinder(
            IIxInstance originInstance,
            IxResolvePath resolvePath,
            IxResolveContext context,
            IxResolveBoundDelegate resolveBound)
        {
            IIxInstance curInstance = originInstance;

            Critical.Assert(resolvePath.Path.Any(), "Registration scope binder does not support empty path.");

            lock (InstanceTreeSyncRoot)
            {
                while (curInstance != null)
                {
                    if (curInstance.ProviderNode == resolvePath.Root)
                    {
                        break;
                    }

                    curInstance = curInstance.ParentInstance;
                }
            }

            if (curInstance == null)
            {
                throw new InvalidOperationException("Resolve algorithms problems");
            }

            Func<IIxInstance, int, Task<IIxInstanceLock>> resolvePathElements = null;

            resolvePathElements = async (parentInstance, index) =>
                {
                    if (index < resolvePath.Path.Count - 1)
                    {
                        using (IIxInstanceLock instanceLock = await Resolve(
                                                                  parentInstance,
                                                                  resolvePath.Path[index].Identifier,
                                                                  context))
                        {
                            return await resolvePathElements(instanceLock.Target, index + 1);
                        }
                    }

                    return await resolveBound(parentInstance, resolvePath.Path.Last(), context);
                };

            IIxInstanceLock resultInstanceLock = await resolvePathElements(curInstance, 0);
            return resultInstanceLock;
        }

        private ResolveDelegate ResolverResolveInterceptor(ResolveDelegate next)
        {
            return async (originInstance, identifier, context) =>
                {
                    if (identifier.Type != typeof(IIxResolver))
                    {
                        return await next(originInstance, identifier, context);
                    }

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

                    return new IxInstanceNoLock(originInstance.Resolver);
                };
        }

        private ResolveDelegate SelfToChildrenResolver(ResolveDelegate next)
        {
            return async (originInstance, identifier, context) =>
                {
                    if (identifier.Name == null)
                    {
                        IxResolvePath resolvePath;
                        if (originInstance.ProviderNode.VisibleNodes.TryGetValue(identifier, out resolvePath))
                        {
                            if (!resolvePath.Path.Any())
                            {
                                lock (InstanceTreeSyncRoot)
                                {
                                    IIxInstance curInstance = originInstance;
                                    while (curInstance != null)
                                    {
                                        if (curInstance.ProviderNode == resolvePath.Root)
                                        {
                                            break;
                                        }

                                        curInstance = curInstance.ParentInstance;
                                    }
                                    
                                    Critical.Assert(curInstance != null, "Instance of an appropriate provider should be found.");

                                    return new IxInstanceTempLock(curInstance);
                                }
                            }
                        }
                    }

                    return await next(originInstance, identifier, context);
                };
        }

        private ResolveDelegate StdResolveInterceptor(ResolveDelegate next)
        {
            return async (originInstance, identifier, context) =>
                {
                    IxResolvePath resolvePath;
                    if (!originInstance.ProviderNode.VisibleNodes.TryGetValue(identifier, out resolvePath))
                    {
                        return await next(originInstance, identifier, context);
                    }

                    return await resolvePath.Target.ScopeBinder(
                               originInstance,
                               resolvePath,
                               context,
                               async (parentInstance, provider, c) =>
                                   {
                                       // While we have temporary lock, we needs to put permanent lock.
                                       IIxInstanceLock resolvedInstanceTempLock =
                                           await provider.GetInstance(parentInstance, c);

                                       return resolvedInstanceTempLock;
                                   });
                };
        }
    }
}