// <copyright file="IKeyedCollection.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Collections
{
    using System.Collections.Generic;

    using ObjectModel;

    /// <summary>
    /// The collection of keyed items.
    /// </summary>
    /// <typeparam name="TKey">The type of the items key.</typeparam>
    /// <typeparam name="TValue">The type of the collection item.</typeparam>
    public interface IKeyedCollection<TKey, TValue> : IReadOnlyKeyedCollection<TKey, TValue>, ICollectionEx<TValue>
        where TValue : IKeyed<TKey>
    {
        new int Count { get; }

        new TValue this[TKey index] { get; set; }

        new IEnumerator<TValue> GetEnumerator();
    }
}