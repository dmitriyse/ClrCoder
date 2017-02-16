// <copyright file="IxStdProviderConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Standard provider node config.
    /// </summary>
    public class IxStdProviderConfig : IxProviderNodeConfig, IIxStdProviderConfig, IIxBasicIdentificationConfig
    {
        /// <inheritdoc/>
        [CanBeNull]
        public IIxScopeBindingConfig ScopeBinding { get; set; }

        /// <inheritdoc/>
        [CanBeNull]
        public IIxMultiplicityConfig Multiplicity { get; set; }

        /// <inheritdoc/>
        [CanBeNull]
        public IIxInstanceBuilderConfig Factory { get; set; }

        /// <summary>
        /// Overrides dispose operation.
        /// </summary>
        [CanBeNull]
        public IxDisposeHandlerDelegate DisposeHandler { get; set; }

        /// <inheritdoc/>
        public Type ContractType { get; set; }

        /// <inheritdoc/>
        public Type ImplementationType { get; set; }

        /// <inheritdoc/>
        public bool AllowAccessByBaseTypes { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }
    }
}