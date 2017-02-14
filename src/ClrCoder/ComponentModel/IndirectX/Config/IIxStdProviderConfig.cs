// <copyright file="IIxStdProviderConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using JetBrains.Annotations;

    /// <summary>
    /// Standard provider node config.
    /// </summary>
    public interface IIxStdProviderConfig : IIxProviderNodeConfig
    {
        /// <summary>
        /// Scope binding strategy config (registration, transient, etc.).
        /// </summary>
        [CanBeNull]
        IIxScopeBindingConfig ScopeBinding { get; }

        /// <summary>
        /// Multiplicity config. (Singleton, pool, factory etc.).
        /// </summary>
        [CanBeNull]
        IIxMultiplicityConfig Multiplicity { get; }

        /// <summary>
        /// Instance builder config. (Class constructor, existing instance, etc.).
        /// </summary>
        [CanBeNull]
        IIxInstanceBuilderConfig Factory { get; }

        /// <summary>
        /// Overrides dispose operation.
        /// </summary>
        [CanBeNull]
        IxDisposeHandlerDelegate DisposeHandler { get; }
    }
}