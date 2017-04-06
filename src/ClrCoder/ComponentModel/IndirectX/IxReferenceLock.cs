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

        /// <summary>
        /// Initializes a new instance of the <see cref="IxReferenceLock"/> class.
        /// </summary>
        /// <param name="target">The instance to lock.</param>
        /// <param name="owner">The owner of this lock.</param>
        public IxReferenceLock(IIxInstance target, IIxInstance owner)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            Critical.Assert(!ReferenceEquals(target, owner), "Cannot create reference from self to self.");

            Target = target;
            Owner = owner;

            lock (Target.ProviderNode.Host.InstanceTreeSyncRoot)
            {
                Target.AddLock(this);
                Owner.AddOwnedLock(this);
            }
        }

        /// <inheritdoc/>
        public IIxInstance Target { get; }

        /// <inheritdoc/>
        public IIxInstance Owner { get; }

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
                Owner.RemoveOwnedLock(this);
            }
        }

        /// <inheritdoc/>
        public void PulseDispose()
        {
            // Do nothing.
        }
    }
}