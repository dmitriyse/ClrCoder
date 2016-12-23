// <copyright file="ContainerConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel
{
    using System;
    using System.Collections.Generic;

    using ObjectModel;

    /// <summary>
    /// <c>Container</c> configuration.
    /// </summary>
    public class ContainerConfig : Freezable, IContainerConfig
    {
        /// <inheritdoc/>
        IReadOnlyDictionary<ContainerNodeKey, IContainerNodeConfig> IContainerConfig.Nodes => Nodes;

        /// <summary>
        /// Frozable nodes dictionary. TODO: Reimplement me.
        /// </summary>
        public Dictionary<ContainerNodeKey, IContainerNodeConfig> Nodes
            => new Dictionary<ContainerNodeKey, IContainerNodeConfig>();

        /// <inheritdoc/>
        public IContainerNodeConfig Clone()
        {
            throw new NotImplementedException();
        }
    }
}