// <copyright file="IxProviderNodeConfigComparer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Collections.Generic;

    /// <summary>
    /// Compares provider node configs by identifier.
    /// </summary>
    public class IxProviderNodeConfigComparer : IEqualityComparer<IIxProviderNodeConfig>
    {
        /// <inheritdoc/>
        public bool Equals(IIxProviderNodeConfig x, IIxProviderNodeConfig y)
        {
            return x.Identifier.Equals(y.Identifier);
        }

        /// <inheritdoc/>
        public int GetHashCode(IIxProviderNodeConfig obj)
        {
            return obj.Identifier.GetHashCode();
        }
    }
}