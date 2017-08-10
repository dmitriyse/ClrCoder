// <copyright file="IxHost.IxSelfInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Threading.Tasks;

    /// <content><see cref="IIxSelf"/> implementation.</content>
    public partial class IxHost
    {
        private class IxSelfInstance : IIxInstance
        {
            public IxSelfInstance(IxProviderNode providerNode, IIxSelf self)
            {
                if (providerNode == null)
                {
                    throw new ArgumentNullException(nameof(providerNode));
                }

                if (self == null)
                {
                    throw new ArgumentNullException(nameof(self));
                }

                ProviderNode = providerNode;
                Object = self;
            }

            public Task DisposeTask => throw new NotSupportedException();

            /// <inheritdoc/>
            public IxProviderNode ProviderNode { get; }

            IIxInstance IIxInstance.ParentInstance { get; }

            IIxResolver IIxInstance.Resolver
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public Task ObjectCreationTask => throw new NotSupportedException();

            public object Object { get; }

            void IIxInstance.AddLock(IIxInstanceLock instanceLock)
            {
                // Do nothing.
            }

            void IIxInstance.AddOwnedLock(IIxInstanceLock instanceLock)
            {
                // Do nothing.
            }

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

            void IIxInstance.SetData(IxProviderNode providerNode, object data)
            {
                throw new NotSupportedException();
            }

            public Task DisposeAsync()
            {
                throw new NotSupportedException();
            }
        }
    }
}