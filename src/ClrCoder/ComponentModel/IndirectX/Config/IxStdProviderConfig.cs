// <copyright file="IxStdProviderConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using JetBrains.Annotations;

    public class IxStdProviderConfig : IxScopeBaseConfig
    {
        [CanBeNull]
        public IxScopeBinding ScopeBinding { get; set; }

        [CanBeNull]
        public IIxMultiplicityConfig Multiplicity { get; set; }

        [CanBeNull]
        public IIxFactoryConfig Factory { get; set; }
    }
}