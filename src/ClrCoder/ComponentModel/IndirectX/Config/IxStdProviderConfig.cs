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
    /// <remarks>We needs to monitor https://github.com/dotnet/roslyn/issues/9482 to improve configuration pattern.</remarks>
    public class IxStdProviderConfig : IxProviderNodeConfig, IIxStdProviderConfig, IIxBasicIdentificationConfig
    {
        /// <inheritdoc/>
        [CanBeNull]
        public virtual IIxScopeBindingConfig ScopeBinding { get; set; }

        /// <inheritdoc/>
        [CanBeNull]
        public virtual IIxMultiplicityConfig Multiplicity { get; set; }

        /// <inheritdoc/>
        [CanBeNull]
        public virtual IIxInstanceBuilderConfig Factory { get; set; }

        /// <summary>
        /// Overrides dispose operation.
        /// </summary>
        [CanBeNull]
        public virtual IxDisposeHandlerDelegate DisposeHandler { get; set; }

        /// <inheritdoc/>
        public virtual Type ContractType { get; set; }

        /// <inheritdoc/>
        public virtual Type ImplementationType { get; set; }

        /// <inheritdoc/>
        public virtual bool AllowAccessByBaseTypes { get; set; }

        /// <inheritdoc/>
        public virtual string Name { get; set; }
    }
}