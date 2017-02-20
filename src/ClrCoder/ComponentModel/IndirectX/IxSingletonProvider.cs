// <copyright file="IxSingletonProvider.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

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
            IxHost.IxResolveContext context)
        {
            if (parentInstance == null)
            {
                throw new ArgumentNullException(nameof(parentInstance));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Task<IIxInstanceLock> creationTask;
            lock (Host.InstanceTreeSyncRoot)
            {
                lock (parentInstance.DataSyncRoot)
                {
                    object data = parentInstance.GetData(this);
                    if (data == null)
                    {
                        Debug.Assert(InstanceFactory != null, "InstanceFactory != null");
                        creationTask = CreateInstance(parentInstance, context);

                        parentInstance.SetData(this, creationTask);
                    }
                    else if (data is Task)
                    {
                        creationTask = (Task<IIxInstanceLock>)data;
                    }
                    else
                    {
                        // Object created.
                        return new IxInstanceTempLock((IIxInstance)data);
                    }
                }
            }

            IIxInstanceLock result = null;
            try
            {
                result = await creationTask;
            }
            finally
            {
                lock (parentInstance.DataSyncRoot)
                {
                    parentInstance.SetData(this, result?.Target);
                }
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            // This is resharper wrong nullability detection.
            return result;
        }

        private async Task<IIxInstanceLock> CreateInstance(IIxInstance parentInstance, IxHost.IxResolveContext context)
        {
            var halfInstantiatedInstance = new IxSingletonInstance(this, parentInstance);
            var result = new IxInstanceTempLock(halfInstantiatedInstance);

            Debug.Assert(InstanceFactory != null, "InstanceFactory != null");

            await InstanceFactory.Factory(
                halfInstantiatedInstance,
                parentInstance,
                context);

            Critical.Assert(halfInstantiatedInstance.Object != null, "Factory should initialize Object property of an instance.");

            // Just creating lock, child instance will dispose this lock inside it async-dispose procedure.
            // ReSharper disable once ObjectCreationAsStatement
            new IxInstanceMasterLock(parentInstance, halfInstantiatedInstance);

            return result;
        }
    }
}