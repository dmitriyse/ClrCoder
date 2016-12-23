// <copyright file="IContainerConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel
{
    using System.Collections.Generic;

    /// <summary>
    /// <c>Container</c> configuration node.
    /// </summary>
    public interface IContainerConfig : IContainerNodeConfig
    {
        /// <summary>
        /// <c>Container</c> nodes configurations.
        /// </summary>
        IReadOnlyDictionary<ContainerNodeKey, IContainerNodeConfig> Nodes { get; }
    }
}