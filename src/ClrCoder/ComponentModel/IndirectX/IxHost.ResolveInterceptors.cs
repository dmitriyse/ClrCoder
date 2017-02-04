// <copyright file="IxHost.ResolveInterceptors.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using ObjectModel;

    public partial class IxHost
    {
        public InterceptableDelegate<ResolveDelegate> ResolveHandler = new InterceptableDelegate<ResolveDelegate>(
            (originInstance, identifier, context) =>
                {
                    throw new NotSupportedException(
                        $"No any rule found to resolve {identifier.Type}|{identifier.Name} dependency.");
                });

        public delegate Task<IIxInstance> ResolveDelegate(
            IIxInstance originInstance,
            IxIdentifier identifier,
            IxResolveContext context);

        private async Task<IIxInstance> RegistrationScopeBinder(
            IIxInstance originInstance,
            IxResolvePath resolvePath,
            IxResolveContext context,
            ResolveBoundDelegate resolveBound)
        {
            IIxInstance curInstance = originInstance;

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

            Func<IIxInstance, int, Task<IIxInstance>> resolvePathElements = null;

            resolvePathElements = async (parentInstance, index) =>
                {
                    if (index < resolvePath.Path.Count - 1)
                    {
                        IIxInstance instance = await Resolve(
                                                   parentInstance,
                                                   resolvePath.Path[index].Identifier,
                                                   context);

                        try
                        {
                            return await resolvePathElements(instance, index + 1);
                        }
                        catch
                        {
                            try
                            {
                                await instance.AsyncDispose();
                            }
                            catch
                            {
                                // Silently disposing intermediate element.
                            }

                            throw;
                        }
                    }

                    return await resolveBound(parentInstance, resolvePath.Path.Last(), context);
                };

            IIxInstance resultInstance = await resolvePathElements(curInstance, 0);
            return resultInstance;
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

                    return originInstance.Resolver;
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
                               (parentInstance, provider, c) => provider.GetInstance(parentInstance, c));
                };
        }
    }
}