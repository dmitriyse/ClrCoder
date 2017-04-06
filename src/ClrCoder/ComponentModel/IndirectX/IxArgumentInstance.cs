// <copyright file="IxArgumentInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
#pragma warning disable 1998
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Threading;

    public class IxArgumentInstance : AsyncDisposableBase, IIxInstance
    {
        public IxArgumentInstance(IxArgumentProvider providerNode, object instanceObj)
        {
            if (providerNode == null)
            {
                throw new ArgumentNullException(nameof(providerNode));
            }

            if (instanceObj == null)
            {
                throw new ArgumentNullException(nameof(instanceObj));
            }

            Object = instanceObj;
            ProviderNode = providerNode;
        }

        /// <inheritdoc/>
        public IxProviderNode ProviderNode { get; }

        IIxInstance IIxInstance.ParentInstance { get; }

        IIxResolver IIxInstance.Resolver
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        Task IIxInstance.ObjectCreationTask => Task.CompletedTask;

        /// <inheritdoc/>
        public object Object { get; }

        /// <inheritdoc/>
        void IIxInstance.AddLock(IIxInstanceLock instanceLock)
        {
            // Do nothing.
        }

        /// <inheritdoc/>
        void IIxInstance.AddOwnedLock(IIxInstanceLock instanceLock)
        {
            // Do nothing.
        }

        /// <inheritdoc/>
        [CanBeNull]
        object IIxInstance.GetData(IxProviderNode providerNode)
        {
            throw new NotSupportedException();
        }

        void IIxInstance.RemoveLock(IIxInstanceLock instanceLock)
        {
            // Do nothing.
        }

        void IIxInstance.RemoveOwnedLock(IIxInstanceLock instanceLock)
        {
            // Do nothing.
        }

        void IIxInstance.SetData(IxProviderNode providerNode, [CanBeNull] object data)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        protected override async Task AsyncDispose()
        {
            // Do nothing.
        }
    }
}