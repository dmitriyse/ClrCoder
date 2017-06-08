// <copyright file="IxHost.IxResolver.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
#pragma warning disable 618
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Threading;

    /// <content><see cref="IxResolver"/> implementation.</content>
    public partial class IxHost
    {
        internal class IxResolver : IIxResolver, IIxInstance
        {
            public IxResolver(
                IxHost host,
                IIxInstance ownerInstance,
                [CanBeNull] IxResolveContext parentContext,
                [CanBeNull] IxResolveFrame parentFrame)
            {
                if (host == null)
                {
                    throw new ArgumentNullException(nameof(host));
                }

                if (ownerInstance == null)
                {
                    throw new ArgumentNullException(nameof(ownerInstance));
                }

                if (parentFrame != null)
                {
                    if (parentContext == null)
                    {
                        throw new ArgumentNullException();
                    }
                }

                Host = host;
                OwnerInstance = ownerInstance;
                ParentFrame = parentFrame;
                ParentContext = parentContext;
            }

            IxProviderNode IIxInstance.ProviderNode => Host._rootScope;

            Task IIxInstance.ObjectCreationTask => throw new NotSupportedException(
                                                       "This is virtual instance and always created.");

            public object Object => this;

            IIxInstance IIxInstance.ParentInstance
            {
                get
                {
                    if (!Monitor.IsEntered(Host.InstanceTreeSyncRoot))
                    {
                        Critical.Assert(
                            false,
                            "Inspecting instance parent should be performed under InstanceTreeLock.");
                    }

                    return OwnerInstance;
                }
            }

            IIxResolver IIxInstance.Resolver
            {
                get
                {
                    Critical.Assert(
                        false,
                        "This is too virtual instance and cannot have nested resolver.");
                    return null;
                }

                set => Critical.Assert(false, "This is too virtual instance and cannot have nested resolver.");
            }

            ////IReadOnlyCollection<IIxInstanceLock> IIxInstance.OwnedLocks => throw new NotSupportedException(
            ////                                                                   "This is too virtual.");

            ////IReadOnlyCollection<IIxInstanceLock> IIxInstance.Locks => throw new NotSupportedException(
            ////                                                              "This is too virtual.");
            Task IAsyncDisposable.DisposeTask
            {
                get
                {
                    Critical.Assert(false, "Too virtual for dispose.");
                    return null;
                }
            }

            public Task<object> ObjectTask { get; }

            [Obsolete("Try remove me")]
            public IxHost Host { get; }

            [NotNull]
            public IIxInstance OwnerInstance { get; }

            [CanBeNull]
            public IxResolveFrame ParentFrame { get; private set; }

            [CanBeNull]
            public IxResolveContext ParentContext { get; private set; }

            void IIxInstance.AddLock(IIxInstanceLock instanceLock)
            {
                ////throw new NotSupportedException(
                ////    "This is completely virtual object and should cannot be locked/unlocked.");

                // Do nothing.
            }

            void IIxInstance.AddOwnedLock(IIxInstanceLock instanceLock)
            {
                Critical.Assert(false, "This is completely virtual object and cannot own locks.");
            }

            [CanBeNull]
            object IIxInstance.GetData(IxProviderNode providerNode)
            {
                Critical.Assert(false, "This object not intendet to have children and children data.");
                return null;
            }

            void IIxInstance.RemoveLock(IIxInstanceLock instanceLock)
            {
                ////throw new NotSupportedException(
                ////    "This is completely virtual object and cannot be locked/unlocked.");

                // Do nothing. 
            }

            void IIxInstance.RemoveOwnedLock(IIxInstanceLock instanceLock)
            {
                Critical.Assert(false, "This is completely virtual object and cannot own locks.");
            }

            public async Task<IIxInstanceLock> Resolve(
                IxIdentifier identifier,
                IReadOnlyDictionary<IxIdentifier, object> arguments = null)
            {
                var context = new IxResolveContext(
                    OwnerInstance,
                    ParentContext,
                    arguments ?? new Dictionary<IxIdentifier, object>());

                using (new IxInstancePinLock(OwnerInstance))
                {
                    return await Host.Resolve(OwnerInstance, identifier, context, ParentFrame);
                }
            }

            void IIxInstance.SetData(IxProviderNode providerNode, [CanBeNull] object data)
            {
                Critical.Assert(false, "This object not intended to have children and children data.");
            }

            public void StartDispose()
            {
                Critical.CheckedAssert(false, "This method is too virtual to dispose it.");
            }

            public void ClearParentResolveContext()
            {
                ParentFrame = null;
                ParentContext = null;
            }
        }
    }
}