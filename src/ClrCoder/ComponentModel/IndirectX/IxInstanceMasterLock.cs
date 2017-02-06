// <copyright file="IxInstanceMasterLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    public class IxInstanceMasterLock : IIxInstanceLock
    {
        private bool _disposed;

        public IxInstanceMasterLock(IIxInstance target, IIxInstance master)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (master == null)
            {
                throw new ArgumentNullException(nameof(master));
            }

            Target = target;
            Master = master;

            lock (Target.Host.InstanceTreeSyncRoot)
            {
                Target.AddLock(this);
                Master.AddOwnedLock(this);
            }
        }

        public IIxInstance Target { get; }

        public IIxInstance Master { get; }

        public void Dispose()
        {
            if (_disposed)
            {
                throw new InvalidOperationException("Lock already disposed.");
            }

            lock (Target.Host.InstanceTreeSyncRoot)
            {
                if (_disposed)
                {
                    throw new InvalidOperationException("Lock already disposed.");
                }

                _disposed = true;

                Target.RemoveLock(this);
                Master.RemoveOwnedLock(this);
            }
        }

        /// <inheritdoc/>
        public void PulseDispose()
        {
            Master.StartDispose();
        }
    }
}