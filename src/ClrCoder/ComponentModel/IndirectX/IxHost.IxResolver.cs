﻿// <copyright file="IxHost.IxResolver.cs" company="ClrCoder project">
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

    using Threading;

    /// <content><see cref="IxResolver"/> implementation.</content>
    public partial class IxHost
    {
        private class IxResolver : IIxResolver
        {
            public IxResolver(IxHost host, IIxInstance instance, [CanBeNull] IxResolveContext context)
            {
                if (host == null)
                {
                    throw new ArgumentNullException(nameof(host));
                }

                if (instance == null)
                {
                    throw new ArgumentNullException(nameof(instance));
                }

                Context = context;
                Host = host;
                Instance = instance;
            }

            IxProviderNode IIxInstance.ProviderNode => Host._rootScope;

            object IIxInstance.Object
            {
                get
                {
                    return this;
                }

                set
                {
                    throw new InvalidOperationException(
                        "IIxResolver should never pass as instance to Resolve routines.");
                }
            }

            IIxInstance IIxInstance.ParentInstance
            {
                get
                {
                    if (!Monitor.IsEntered(Host.InstanceTreeSyncRoot))
                    {
                        throw new InvalidOperationException(
                            "Inspecting instance parent should be performed under InstanceTreeLock.");
                    }

                    return Instance;
                }
            }

            IIxResolver IIxInstance.Resolver
            {
                get
                {
                    throw new InvalidOperationException("This is too virtual instance and cannot have nested resolver.");
                }

                set
                {
                    throw new InvalidOperationException("This is too virtual instance and cannot have nested resolver.");
                }
            }

            object IIxInstance.DataSyncRoot
            {
                get
                {
                    throw new NotSupportedException("This object not intendet to have children and children data.");
                }
            }

            IReadOnlyCollection<IIxInstanceLock> IIxInstance.OwnedLocks
            {
                get
                {
                    throw new NotSupportedException("This is too virtual.");
                }
            }

            IReadOnlyCollection<IIxInstanceLock> IIxInstance.Locks
            {
                get
                {
                    throw new NotSupportedException("This is too virtual.");
                }
            }

            Task IAsyncDisposable.DisposeTask
            {
                get
                {
                    throw new NotSupportedException("Too virtual for dispose.");
                }
            }

            public IxHost Host { get; }

            public bool IsDisposing
            {
                get
                {
                    throw new NotSupportedException("Lifetime methods cannot be used for this virtual object.");
                }
            }

            public IIxInstance Instance { get; }

            [CanBeNull]
            public IxResolveContext Context { get; }

            void IIxInstance.AddLock(IIxInstanceLock instanceLock)
            {
                ////throw new NotSupportedException(
                ////    "This is completely virtual object and should cannot be locked/unlocked.");
                
                // Do nothing.
            }

            void IIxInstance.AddOwnedLock(IIxInstanceLock instanceLock)
            {
                throw new NotSupportedException(
                    "This is completely virtual object and cannot own locks.");
            }

            object IIxInstance.GetData(IxProviderNode providerNode)
            {
                throw new NotSupportedException("This object not intendet to have children and children data.");
            }

            void IIxInstance.RemoveLock(IIxInstanceLock instanceLock)
            {
                ////throw new NotSupportedException(
                ////    "This is completely virtual object and cannot be locked/unlocked.");
                
                // Do nothing. 
            }

            void IIxInstance.RemoveOwnedLock(IIxInstanceLock instanceLock)
            {
                throw new NotSupportedException(
                    "This is completely virtual object and cannot own locks.");
            }

            public async Task<IIxInstanceLock> Resolve(IxIdentifier identifier)
            {
                var context = new IxResolveContext(Context);
                using (new IxInstanceTempLock(Instance))
                {
                    return await Host.Resolve(Instance, identifier, context);
                }
            }

            void IIxInstance.SetData(IxProviderNode providerNode, object data)
            {
                throw new NotSupportedException("This object not intendet to have children and children data.");
            }

            public void StartDispose()
            {
                throw new NotSupportedException("This method is too virtual to dispose it.");
            }
        }
    }
}