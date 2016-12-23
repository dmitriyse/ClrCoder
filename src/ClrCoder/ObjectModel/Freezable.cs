// <copyright file="Freezable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ObjectModel
{
    using System;

    /// <summary>
    /// Default implementation of <see cref="IFreezable"/> <c>object</c>.
    /// </summary>
    public abstract class Freezable : IFreezable
    {
        /// <inheritdoc/>
        public bool IsFrozen { get; private set; }

        /// <inheritdoc/>
        public virtual void Freeze()
        {
            if (IsFrozen)
            {
                throw new InvalidOperationException("Object already frozen.");
            }

            IsFrozen = true;
        }
    }
}