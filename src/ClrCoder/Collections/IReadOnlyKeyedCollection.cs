// <copyright file="IReadOnlyKeyedCollection.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Collections
{
    using System.Collections.Generic;

    public interface IReadOnlyKeyedCollection<TKey, TItem> : IReadOnlyDictionaryEx<TKey, TItem>,
                                                             IReadOnlyCollection<TItem>
    {
    }
}