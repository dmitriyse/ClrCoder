// <copyright file="IxReferenceLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    /// <summary>
    /// Lock when one object references another.
    /// </summary>
    public class IxReferenceLock : IIxInstanceLock
    {
        private bool _disposed;

        public IxReferenceLock(IIxInstance target, IIxInstance user)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Target = target;
            User = user;

            lock (Target.ProviderNode.Host.InstanceTreeSyncRoot)
            {
                Target.AddLock(this);
                User.AddOwnedLock(this);
            }
        }

        public IIxInstance Target { get; }

        public IIxInstance User { get; }

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
                User.RemoveOwnedLock(this);
            }
        }

        /// <inheritdoc/>
        public void PulseDispose()
        {
            // Do nothing.
        }
    }
}