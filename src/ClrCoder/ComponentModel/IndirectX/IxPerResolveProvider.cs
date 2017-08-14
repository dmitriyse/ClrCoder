// <copyright file="IxPerResolveProvider.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Validation;

    /// <summary>
    /// Creates one instance per resolve request. TODO: Perform some generalize with IxSingletonProvider.
    /// </summary>
    public class IxPerResolveProvider : IxProviderNode
    {
        public IxPerResolveProvider(
            IxHost host,
            [CanBeNull] IxProviderNode parentNode,
            IxProviderNodeConfig config,
            [CanBeNull] IxInstanceFactory instanceFactory,
            IxVisibilityFilter exportFilter,
            IxVisibilityFilter exportToParentFilter,
            IxVisibilityFilter importFilter,
            IxScopeBinderDelegate scopeBinder,
            IxDisposeHandlerDelegate disposeHandler)
            : base(
                host,
                parentNode,
                config,
                instanceFactory,
                exportFilter,
                exportToParentFilter,
                importFilter,
                scopeBinder,
                disposeHandler)
        {
            VxArgs.NotNull(parentNode, nameof(parentNode));
            VxArgs.NotNull(instanceFactory, nameof(instanceFactory));

            // Adding self provided as default for children.
            VisibleNodes.Add(new IxIdentifier(Identifier.Type), new IxResolvePath(this, new IxProviderNode[] { }));
        }

        /// <inheritdoc/>
        public override async ValueTask<IIxInstanceLock> GetInstance(
            IIxInstance parentInstance,
            IxIdentifier identifier,
            IxHost.IxResolveContext context,
            [CanBeNull] IxResolveFrame frame)
        {
            if (parentInstance == null)
            {
                throw new ArgumentNullException(nameof(parentInstance));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            IIxInstanceLock creatorLock = null;
            IxPerResolveInstance instance;

            // Re-implement this with more advanced Half-Instantiation with loop detection.
            lock (Host.InstanceTreeSyncRoot)
            {
                instance = context.GetData(this) as IxPerResolveInstance;
                if (instance == null)
                {
                    // TODO: Detect cycles.
                    Debug.Assert(InstanceFactory != null, "InstanceFactory != null");

                    instance = new IxPerResolveInstance(this, parentInstance, context, frame, out creatorLock);

                    context.SetData(this, instance);
                }
            }

            try
            {
                await instance.ObjectCreationTask;
                return creatorLock ?? new IxInstancePinLock(instance);
            }
            catch
            {
                if (creatorLock != null)
                {
                    lock (Host.InstanceTreeSyncRoot)
                    {
                        context.SetData(this, null);
                    }

                    creatorLock.Dispose();
                }

                throw;
            }
        }
    }
}