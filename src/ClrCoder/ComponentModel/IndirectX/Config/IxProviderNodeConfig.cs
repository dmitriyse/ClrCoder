// <copyright file="IxProviderNodeConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Collections.Generic;

    /// <summary>
    /// Base configuration for any provider node.
    /// </summary>
    public class IxProviderNodeConfig : IIxProviderNodeConfig
    {
        /// <inheritdoc/>
        public IxIdentifier Identifier { get; set; }

        /// <inheritdoc/>
        public ICollection<IIxProviderNodeConfig> Nodes { get; }
            = new HashSet<IIxProviderNodeConfig>(new IxProviderNodeConfigComparer());

        /// <inheritdoc/>
        public IIxVisibilityFilterConfig ImportFilter { get; set; }

        /// <inheritdoc/>
        public IIxVisibilityFilterConfig ExportToParentFilter { get; set; }

        /// <inheritdoc/>
        public IIxVisibilityFilterConfig ExportFilter { get; set; }
    }
}