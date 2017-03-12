// <copyright file="IxProviderNodeConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base configuration for any provider node.
    /// </summary>
    /// <remarks>
    /// TODO: Implement freezable behavior.
    /// </remarks>
    public class IxProviderNodeConfig : IIxProviderNodeConfig, IFreezable<IxProviderNodeConfig>, IFreezable
    {
        /// <inheritdoc/>
        public virtual IxIdentifier? Identifier { get; set; }

        /// <inheritdoc/>
        public virtual ICollection<IIxProviderNodeConfig> Nodes { get; }
            = new HashSet<IIxProviderNodeConfig>();

        /// <inheritdoc/>
        public virtual IIxVisibilityFilterConfig ImportFilter { get; set; }

        /// <inheritdoc/>
        public virtual IIxVisibilityFilterConfig ExportToParentFilter { get; set; }

        /// <inheritdoc/>
        public virtual IIxVisibilityFilterConfig ExportFilter { get; set; }

        /// <inheritdoc/>
        public virtual bool IsImmutable { get; protected set; }

        /// <inheritdoc/>
        public bool IsShallowImmutable => IsImmutable;

        /// <inheritdoc/>
        public void Freeze()
        {
            // TODO: Implement me.
        }

        /// <inheritdoc/>
        public void ShallowFreeze()
        {
            throw new NotSupportedException("User Freeze instead.");
        }
    }
}