// <copyright file="IxStdVisibilityFilterConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    public class IxStdVisibilityFilterConfig : IIxVisibilityFilterConfig
    {
        [CanBeNull]
        public HashSet<IxIdentifier> WhiteList { get; set; }

        [CanBeNull]
        public HashSet<IxIdentifier> BlackList { get; set; }
    }
}