// <copyright file="IxSingletonProvider.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class IxSingletonProvider : IxProviderNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxSingletonProvider"/> class.
        /// </summary>
        public IxSingletonProvider(
            IxHost host,
            IxProviderNode parentNode,
            IxStdProviderConfig config,
            IxRawInstanceFactory rawInstanceFactory,
            IxVisibilityFilter exportFilter,
            IxVisibilityFilter exportToParentFilter,
            IxVisibilityFilter importFilter,
            IxScopeBinderDelegate scopeBinder,
            IxDisposeHandlerDelegate disposeHandler)
            : base(
                host,
                parentNode,
                config,
                rawInstanceFactory,
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

            if (rawInstanceFactory == null)
            {
                throw new ArgumentNullException(nameof(rawInstanceFactory));
            }

            // Adding self provided as default for children.
            VisibleNodes.Add(new IxIdentifier(Identifier.Type), new IxResolvePath(this, new IxProviderNode[] { }));
        }

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
                        Debug.Assert(RawInstanceFactory != null, "RawInstanceFactory != null");
                        creationTask = RawInstanceFactory.Factory(
                                     this,
                                     parentInstance,
                                     context,
                                     (provider, parent, ctx, @obj) => new IxInstanceTempLock(new IxSingletonInstance(provider, parent, @obj)));

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
    }
}