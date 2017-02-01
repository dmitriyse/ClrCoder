// <copyright file="IxComponentConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

    public class IxComponentConfig : IxScopeConfig
    {
        [CanBeNull]
        public Delegate Factory { get; set; }

        public Type FactoryType { get; set; }

        public ScopeBinding ScopeBinding { get; set; }
    }
}