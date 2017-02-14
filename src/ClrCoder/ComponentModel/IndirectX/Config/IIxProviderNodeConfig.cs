// <copyright file="IIxProviderNodeConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// Base provider node configuration contract.
    /// </summary>
    [PublicAPI]
    public interface IIxProviderNodeConfig
    {
        /// <summary>
        /// Identifier in the parent scope.
        /// </summary>
        IxIdentifier Identifier { get; }

        /// <summary>
        /// Import filter. Controls which registrations of parent node are visible for current node.
        /// </summary>
        [CanBeNull]
        IIxVisibilityFilterConfig ImportFilter { get; }

        /// <summary>
        /// Export to parent filter. Controls which registrations of <c>this</c> node will be visible in parent node.
        /// </summary>
        [CanBeNull]
        IIxVisibilityFilterConfig ExportToParentFilter { get; }

        /// <summary>
        /// Export to children filter. Controls which registrations of <c>this</c> node.
        /// </summary>
        [CanBeNull]
        IIxVisibilityFilterConfig ExportFilter { get; }

        /// <summary>
        /// Nested provider node registrations.
        /// </summary>
        ICollection<IIxProviderNodeConfig> Nodes { get; }
    }
}