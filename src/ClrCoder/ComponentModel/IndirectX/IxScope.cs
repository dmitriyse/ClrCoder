// <copyright file="IxScope.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using JetBrains.Annotations;

    public class IxScope : IxScopeBase
    {
        public IxScope(IxHost host, [CanBeNull] IxScopeBase parentNode, IxScopeConfig config)
            : base(host, parentNode, config)
        {
        }
    }
}