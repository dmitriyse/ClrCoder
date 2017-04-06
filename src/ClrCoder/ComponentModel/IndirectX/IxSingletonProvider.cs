// <copyright file="IxSingletonProvider.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Standard singleton provider.
    /// </summary>
    public class IxSingletonProvider : IxProviderNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxSingletonProvider"/> class.
        /// </summary>
        /// <param name="host">IndirectX host.</param>
        public IxSingletonProvider(
            IxHost host,
            IxProviderNode parentNode,
            IxStdProviderConfig config,
            IxInstanceFactory instanceFactory,
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
            if (parentNode == null)
            {
                throw new ArgumentNullException(nameof(parentNode));
            }

            if (instanceFactory == null)
            {
                throw new ArgumentNullException(nameof(instanceFactory));
            }

            // Adding self provided as default for children.
            VisibleNodes.Add(new IxIdentifier(Identifier.Type), new IxResolvePath(this, new IxProviderNode[] { }));
        }

        /// <inheritdoc/>
        public override async Task<IIxInstanceLock> GetInstance(
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
            IxSingletonInstance instance;

            // Re-implement this with more advanced Half-Instantiation with loop detection.
            lock (Host.InstanceTreeSyncRoot)
            {
                instance = parentInstance.GetData(this) as IxSingletonInstance;
                if (instance == null)
                {
                    // TODO: Detect cycles.
                    Debug.Assert(InstanceFactory != null, "InstanceFactory != null");

                    instance = new IxSingletonInstance(this, parentInstance, context, frame, out creatorLock);

                    parentInstance.SetData(this, instance);
                }
            }

            try
            {
                await instance.ObjectCreationTask;
                return creatorLock ?? new IxInstanceTempLock(instance);
            }
            catch
            {
                if (creatorLock != null)
                {
                    lock (Host.InstanceTreeSyncRoot)
                    {
                        parentInstance.SetData(this, null);
                    }

                    creatorLock.Dispose();
                }

                throw;
            }
        }
    }
}