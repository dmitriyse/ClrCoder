// <copyright file="IxInstanceTempLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    public class IxInstanceTempLock : IIxInstanceLock
    {
        private bool _disposed;

        public IxInstanceTempLock(IIxInstance target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Target = target;

            lock (target.ProviderNode.Host.InstanceTreeSyncRoot)
            {
                Target.AddLock(this);
            }
        }

        /// <inheritdoc/>
        public IIxInstance Target { get; }

        IIxInstance IIxInstanceLock.Owner { get; } = null;

        /// <inheritdoc/>
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
            }
        }

        /// <inheritdoc/>
        public void PulseDispose()
        {
            // Do nothing.
        }
    }
}