// <copyright file="IxInstanceNoLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    public class IxInstanceNoLock : IIxInstanceLock
    {
        public IxInstanceNoLock(IIxInstance target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Target = target;
        }

        public IIxInstance Target { get; }

        public IIxInstance Owner { get; } = null;

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do nothing.
        }

        /// <inheritdoc/>
        public void PulseDispose()
        {
            // Do nothing.
        }
    }
}