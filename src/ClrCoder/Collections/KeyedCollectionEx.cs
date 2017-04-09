// <copyright file="KeyedCollectionEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Collections
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using JetBrains.Annotations;

    using ObjectModel;

    /// <summary>
    /// TODO: Reimplement me without inheritance from KeyedCollection.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyedCollectionEx<TKey, TValue> : KeyedCollection<TKey, TValue>, IReadOnlyKeyedCollection<TKey, TValue>
        where TValue : IKeyed<TKey>
    {
        public KeyedCollectionEx()
        {
        }

        public KeyedCollectionEx(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public IEnumerable<TKey> Keys => Dictionary.Keys;

        public IEnumerable<TValue> Values => Dictionary.Values;

        /// <inheritdoc/>
        public bool ContainsKey([NotNull] TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        /// <inheritdoc/>
        public bool TryGetValue([NotNull] TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        protected override TKey GetKeyForItem(TValue item)
        {
            return item.Key;
        }
    }
}