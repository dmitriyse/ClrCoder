// <copyright file="IxInstanceChildLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    /// <summary>
    /// Locks from children to parent. Child is an owner of this lock.
    /// </summary>
    public class IxInstanceChildLock : IIxInstanceLock
    {
        private bool _disposed;

        public IxInstanceChildLock(IIxInstance parent, IIxInstance child)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            Target = parent;
            Owner = child;

            lock (Target.ProviderNode.Host.InstanceTreeSyncRoot)
            {
                Target.AddLock(this);
                Owner.AddOwnedLock(this);
            }
        }

        public IIxInstance Target { get; }

        public IIxInstance Owner { get; }

        public void Dispose()
        {
            if (_disposed)
            {
                throw new InvalidOperationException("Lock already disposed.");
            }

            lock (Target.ProviderNode.Host.InstanceTreeSyncRoot)
            {
                if (_disposed)
                {
                    throw new InvalidOperationException("Lock already disposed.");
                }

                _disposed = true;

                Target.RemoveLock(this);
                Owner.RemoveOwnedLock(this);
            }
        }

        /// <inheritdoc/>
        public void PulseDispose()
        {
            Owner.StartDispose();
        }
    }
}