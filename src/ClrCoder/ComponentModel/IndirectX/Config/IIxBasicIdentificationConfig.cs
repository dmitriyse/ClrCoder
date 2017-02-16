// <copyright file="IIxBasicIdentificationConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Basic configuration for registration identifiers.
    /// </summary>
    public interface IIxBasicIdentificationConfig
    {
        /// <summary>
        /// Allows registration to be accessed by specified contract type.
        /// </summary>
        [CanBeNull]
        Type ContractType { get; }

        /// <summary>
        /// Allows access to contract implementation by
        /// </summary>
        [CanBeNull]
        Type ImplementationType { get; }

        /// <summary>
        /// Allows registration to be accessed by contract base types. If <see cref="ImplementationType"/> is specified, it will
        /// not be accessable by base types regardless of this setting.
        /// </summary>
        bool AllowAccessByBaseTypes { get; }

        /// <summary>
        /// Registration name. It will be used to produce identifiers.
        /// </summary>
        [CanBeNull]
        string Name { get; }
    }
}