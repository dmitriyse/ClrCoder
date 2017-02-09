// <copyright file="LazyDictionary.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Reflection;

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
            this._valueFactory = valueFactory;
            this.Keys = this._dictionary.Keys;
            this.Values = this._dictionary.Values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyDictionary{TKey, TValue}"/> class.
        /// </summary>
        public LazyDictionary()
        {
            this._valueFactory = DefaultValueFactory;
            if (this._valueFactory == null)
            {
                // ReSharper disable once PossibleNullReferenceException
                throw DefaultValueFactoryCreateException;
            }

            this.Keys = this._dictionary.Keys;
            this.Values = this._dictionary.Values;
        }

        /// <inheritdoc/>
        public int Count => this._dictionary.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public ICollection<TKey> Keys { get; }

        /// <inheritdoc/>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => this._dictionary.Keys;

        /// <inheritdoc/>
        public ICollection<TValue> Values { get; }

        /// <inheritdoc/>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => this._dictionary.Values;

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                TValue curValue;
                if (!this._dictionary.TryGetValue(key, out curValue))
                {
                    curValue = this._valueFactory.Invoke();
                    this._dictionary.Add(key, curValue);
                }

                return curValue;
            }

            set
            {
                this._dictionary[key] = value;
            }
        }

        /// <inheritdoc/>
        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] => this._dictionary[key];

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            this._dictionary.Add(key, value);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)this._dictionary).Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this._dictionary.Clear();
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)this._dictionary).Contains(item);
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return this._dictionary.ContainsKey(key);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)this._dictionary).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this._dictionary.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            return this._dictionary.Remove(key);
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)this._dictionary).Remove(item);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return this._dictionary.TryGetValue(key, out value);
        }
    }
}