// <copyright file="LazyDictionary.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.System.Collections
{
    using global::System;
    using global::System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// <c>Dictionary</c>, which automatically creates values for the requested keys.
    /// </summary>
    /// <remarks>
    /// All methods and properties are not thread safe.
    /// </remarks>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [PublicAPI]
    public class LazyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        [CanBeNull]
        private static readonly Func<TValue> DefaultValueFactory;

        // ReSharper disable once StaticFieldInGenericType
        [CanBeNull]
        private static readonly Exception DefaultValueFactoryCreateException;

        private readonly Dictionary<TKey, TValue> _dictionary = new DictionaryEx<TKey, TValue>();

        private readonly Func<TValue> _valueFactory;

        static LazyDictionary()
        {
            try
            {
                DefaultValueFactory = TypeEx<TValue>.CreateConstructorDelegate();
            }
            catch (Exception ex)
            {
                DefaultValueFactoryCreateException = ex;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="valueFactory">Value factory method.</param>
        public LazyDictionary(Func<TValue> valueFactory)
        {
            _valueFactory = valueFactory;
            Keys = _dictionary.Keys;
            Values = _dictionary.Values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyDictionary{TKey, TValue}"/> class.
        /// </summary>
        public LazyDictionary()
        {
            _valueFactory = DefaultValueFactory;
            if (_valueFactory == null)
            {
                // ReSharper disable once PossibleNullReferenceException
                throw DefaultValueFactoryCreateException;
            }

            Keys = _dictionary.Keys;
            Values = _dictionary.Values;
        }

        /// <inheritdoc/>
        public int Count => _dictionary.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public ICollection<TKey> Keys { get; }

        /// <inheritdoc/>
        public ICollection<TValue> Values { get; }

        /// <inheritdoc/>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;

        /// <inheritdoc/>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                TValue curValue;
                if (!_dictionary.TryGetValue(key, out curValue))
                {
                    curValue = _valueFactory.Invoke();
                    _dictionary.Add(key, curValue);
                }

                return curValue;
            }

            set
            {
                _dictionary[key] = value;
            }
        }

        /// <inheritdoc/>
        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] => _dictionary[key];

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);
        }
    }
}