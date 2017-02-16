// <copyright file="IxScopeConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using JetBrains.Annotations;

    /// <summary>
    /// Configuration for a scope.
    /// </summary>
    [PublicAPI]
    public class IxScopeConfig : IxProviderNodeConfig, IIxScopeConfig
    {
        /// <inheritdoc/>
        public bool IsInstanceless { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }
    }
}