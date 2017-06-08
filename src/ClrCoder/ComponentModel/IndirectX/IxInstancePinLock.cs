// <copyright file="IxInstancePinLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using Annotations;

    /// <summary>
    /// Pin lock, hold instance from disposing.
    /// </summary>
    [InvalidUsageIsCritical]
    public class IxInstancePinLock : IIxInstanceLock
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IxInstancePinLock"/> class.
        /// </summary>
        /// <param name="target">IndirectX instance.</param>
        public IxInstancePinLock(IIxInstance target)
        {
            Critical.Assert(target != null, "Pin lock target should not be null.");

            Target = target;

            lock (target.ProviderNode.Host.InstanceTreeSyncRoot)
            {
                Target.AddLock(this);
            }
        }

        /// <inheritdoc/>
        public IIxInstance Target { get; }

        /// <inheritdoc/>
        IIxInstance IIxInstanceLock.Owner { get; } = null;

        /// <inheritdoc/>
        public void Dispose()
        {
            try
            {
                Critical.Assert(!_disposed, "Lock already disposed.");

                lock (Target.ProviderNode.Host.InstanceTreeSyncRoot)
                {
                    Critical.Assert(!_disposed, "Lock already disposed.");

                    _disposed = true;

                    Target.RemoveLock(this);
                }
            }
            catch
            {
                // Dispose should decouple errors propagation.
                // Usually dispose errors relates only to disposing component, but not to it's caller.
            }
        }

        /// <inheritdoc/>
        public void PulseDispose()
        {
            // Do nothing.
        }
    }
}