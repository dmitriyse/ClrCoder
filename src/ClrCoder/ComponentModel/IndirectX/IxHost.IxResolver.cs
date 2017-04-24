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
                        throw new InvalidOperationException(
                            "Inspecting instance parent should be performed under InstanceTreeLock.");
                    }

                    return OwnerInstance;
                }
            }

            IIxResolver IIxInstance.Resolver
            {
                get => throw new InvalidOperationException(
                           "This is too virtual instance and cannot have nested resolver.");

                set => throw new InvalidOperationException(
                           "This is too virtual instance and cannot have nested resolver.");
            }

            ////IReadOnlyCollection<IIxInstanceLock> IIxInstance.OwnedLocks => throw new NotSupportedException(
            ////                                                                   "This is too virtual.");

            ////IReadOnlyCollection<IIxInstanceLock> IIxInstance.Locks => throw new NotSupportedException(
            ////                                                              "This is too virtual.");
            Task IAsyncDisposable.DisposeTask => throw new NotSupportedException("Too virtual for dispose.");

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

            public async Task<IIxInstanceLock> Resolve(
                IxIdentifier identifier,
                IReadOnlyDictionary<IxIdentifier, object> arguments = null)
            {
                var context = new IxResolveContext(
                    OwnerInstance,
                    ParentContext,
                    arguments ?? new Dictionary<IxIdentifier, object>());

                using (new IxInstanceTempLock(OwnerInstance))
                {
                    return await Host.Resolve(OwnerInstance, identifier, context, ParentFrame);
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

            public void ClearParentResolveContext()
            {
                ParentFrame = null;
                ParentContext = null;
            }
        }
    }
}