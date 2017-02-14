// <copyright file="IIxScopeConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    /// <summary>
    /// Scope provider node configuration.
    /// </summary>
    public interface IIxScopeConfig : IIxProviderNodeConfig
    {
        /// <summary>
        /// Specifies where instance for a scope should be created.
        /// </summary>
        /// <remarks>
        /// TODO: Implement <c>this</c> feature.
        /// </remarks>
        bool IsInstanceless { get; }
    }
}