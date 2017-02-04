// <copyright file="IxInstance.cs" company="ClrCoder project">
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

    public abstract class IxInstance : IIxInstance
    {
        private readonly IIxInstance _parentInstance;

        [CanBeNull]
        private Dictionary<IxProviderNode, object> _childrenData;

        public IxInstance(
            IxHost host,
            IxProviderNode providerNode,
            [CanBeNull] IIxInstance parentInstance,
            object @object)
        {
            Host = host;
            ProviderNode = providerNode;
            _parentInstance = parentInstance;
            Object = @object;
        }

        public IxHost Host { get; }

        public IxProviderNode ProviderNode { get; }

        public object Object { get; }

        [CanBeNull]
        public IIxInstance ParentInstance
        {
            get
            {
                if (!Monitor.IsEntered(Host.InstanceTreeSyncRoot))
                {
                    throw new InvalidOperationException(
                        "Inspecting instance parent should be performed under InstanceTreeLock.");
                }

                return _parentInstance;
            }
        }

        public IIxResolver Resolver { get; set; }

        public object DataSyncRoot => ChildrenData;

        private Dictionary<IxProviderNode, object> ChildrenData
        {
            get
            {
                if (_childrenData == null)
                {
                    Interlocked.CompareExchange(ref _childrenData, new Dictionary<IxProviderNode, object>(), null);
                }

                return _childrenData;
            }
        }

        public abstract Task AsyncDispose();

        [CanBeNull]
        public object GetData(IxProviderNode providerNode)
        {
            if (providerNode == null)
            {
                throw new ArgumentNullException(nameof(providerNode));
            }

            if (!Monitor.IsEntered(Host.InstanceTreeSyncRoot))
            {
                throw new InvalidOperationException(
                    "Inspecting instance parent should be performed under InstanceTreeLock.");
            }

            if (!Monitor.IsEntered(DataSyncRoot))
            {
                throw new InvalidOperationException("Data manipulations should be performed under lock.");
            }

            object result;

            ChildrenData.TryGetValue(providerNode, out result);

            return result;
        }

        public void SetData(IxProviderNode providerNode, [CanBeNull] object data)
        {
            if (providerNode == null)
            {
                throw new ArgumentNullException(nameof(providerNode));
            }

            if (!Monitor.IsEntered(Host.InstanceTreeSyncRoot))
            {
                throw new InvalidOperationException(
                    "Inspecting instance parent should be performed under InstanceTreeLock.");
            }

            if (!Monitor.IsEntered(DataSyncRoot))
            {
                throw new InvalidOperationException("Data manipulations should be performed under lock.");
            }

            if (data == null)
            {
                ChildrenData.Remove(providerNode);
            }
            else
            {
                ChildrenData[providerNode] = data;
            }
        }
    }
}