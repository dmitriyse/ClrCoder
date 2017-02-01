// <copyright file="IxScopeBaseConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Collections.Generic;

    public class IxScopeBaseConfig
    {
        public IxIdentifier Identifier { get; set; }

        public List<IxScopeBaseConfig> Nodes { get; } = new List<IxScopeBaseConfig>();

        public bool ImportFromParent { get; set; }
    }
}