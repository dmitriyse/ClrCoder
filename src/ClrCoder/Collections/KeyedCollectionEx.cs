// <copyright file="KeyedCollectionEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using ObjectModel;

    /// <summary>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyedCollectionEx<TKey, TValue> : IKeyedCollection<TKey, TValue>
        where TValue : IKeyed<TKey>
    {
        private Dictionary<TKey, TValue> _inner;

        public KeyedCollectionEx()
        {
            _inner = new DictionaryEx<TKey, TValue>();
        }

        public KeyedCollectionEx(IEqualityComparer<TKey> comparer)
        {
            _inner = new DictionaryEx<TKey, TValue>(comparer);
        }

        public IEnumerable<TKey> Keys => _inner.Keys;

        public IEnumerable<TValue> Values => _inner.Values;

        public int Count => _inner.Count;

        IEnumerator<TValue> IReadOnlyKeyedCollection<TKey, TValue>.GetEnumerator()
        {
            return _inner.Values.GetEnumerator();
        }

        public TValue this[TKey index]
        {
            get => _inner[index];
            set => _inner[index] = value;
        }

        public bool IsReadOnly { get; }

        public IEqualityComparer<TKey> Comparer { get; }

        public void Add(TValue item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _inner.Add(item.Key, item);
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(TValue item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            TValue existingValue;

            if (TryGetValue(item.Key, out existingValue))
            {
                return ReferenceEquals(item, existingValue);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool ContainsKey([NotNull] TKey key)
        {
            return _inner.ContainsKey(key);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _inner.Values.CopyTo(array, arrayIndex);
        }

        IEnumerator<TValue> IKeyedCollection<TKey, TValue>.GetEnumerator()
        {
            return _inner.Values.GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _inner.Values.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_inner.Values).GetEnumerator();
        }

        public bool Remove(TValue item)
        {
            return _inner.Remove(item.Key);
        }

        /// <inheritdoc/>
        public bool TryGetValue([NotNull] TKey key, out TValue value)
        {
            return _inner.TryGetValue(key, out value);
        }
    }
}