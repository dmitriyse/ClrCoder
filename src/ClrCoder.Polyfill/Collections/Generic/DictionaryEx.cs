// <copyright file="DictionaryEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    /// <summary>
    /// Represents a collection of keys and values. <br/>
    /// Polyfill of comming new features.
    /// TODO: Keys is not implements IReadOnlySet, just implements IReadOnlySetThin.
    /// </summary>
    /// <remarks>
    /// Implements https://github.com/dotnet/corefx/issues/16661 proposal. Adds IFreezable behavior to standard BCL
    /// dictionary and implements collection contracts improvements like https://github.com/dotnet/corefx/issues/16660.
    /// </remarks>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [PublicAPI]
    public class DictionaryEx<TKey, TValue> : IDictionaryEx<TKey, TValue>,
                                              IDictionary
    {
        private readonly Dictionary<TKey, TValue> _dict;

        private KeyCollection _keys;

        private ValueCollection _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEx{TKey,TValue}"/> class.
        /// </summary>
        public DictionaryEx()
        {
            _dict = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEx{TKey,TValue}"/> class that contains elements copied from the
        /// specified <see cref="DictionaryEx{TKey,TValue}"/> and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">Dictionary with initial content.</param>
        public DictionaryEx(IDictionary<TKey, TValue> dictionary)
        {
            _dict = new Dictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEx{TKey,TValue}"/> class that contains elements copied from the
        /// specified <see cref="DictionaryEx{TKey,TValue}"/> and uses the specified comparer for the key type.
        /// </summary>
        /// <param name="dictionary">Dictionary with initial content.</param>
        /// <param name="comparer">Comparer for keys.</param>
        public DictionaryEx(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEx{TKey,TValue}"/> class with the specified comparer for the key
        /// type.
        /// </summary>
        /// <param name="comparer">Comparer for keys.</param>
        public DictionaryEx(IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEx{TKey,TValue}"/> class that is empty, has the specified
        /// initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="capacity">Initial capacity.</param>
        public DictionaryEx(int capacity)
        {
            _dict = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEx{TKey,TValue}"/> class that is empty, has the specified
        /// initial capacity, and uses the specified comparer for the key type.
        /// </summary>
        /// <param name="capacity">Initial capacity.</param>
        /// <param name="comparer">Comparer for keys.</param>
        public DictionaryEx(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        private DictionaryEx(Dictionary<TKey, TValue> dictionary)
        {
            _dict = dictionary;
        }

        /// <inheritdoc/>
        public int Count => _dict.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => IsFrozen;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keys ?? (_keys = new KeyCollection(this));

        ICollection<TValue> IDictionary<TKey, TValue>.Values => _values ?? (_values = new ValueCollection(this));

        bool ICollection.IsSynchronized => ((ICollection)_dict).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)_dict).SyncRoot;

        bool IDictionary.IsFixedSize => ((IDictionary)_dict).IsFixedSize;

        bool IDictionary.IsReadOnly => IsFrozen;

        ICollection IDictionary.Keys => _keys ?? (_keys = new KeyCollection(this));

        ICollection IDictionary.Values => _values ?? (_values = new ValueCollection(this));

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _keys ?? (_keys = new KeyCollection(this));

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values =>
            _values ?? (_values = new ValueCollection(this));

        /// <inheritdoc/>
        public bool IsImmutable => IsFrozen;

        /// <inheritdoc/>
        public bool IsFrozen { get; private set; }

        /// <inheritdoc/>
        public IEqualityComparer<TKey> Comparer => _dict.Comparer;

        /// <summary>
        /// Keys collection.
        /// </summary>
        public KeyCollection Keys => _keys ?? (_keys = new KeyCollection(this));

        /// <summary>
        /// Values collection.
        /// </summary>
        public ValueCollection Values => _values ?? (_values = new ValueCollection(this));

        /// <inheritdoc/>
        [CanBeNull]
        public TValue this[[NotNull] TKey key]
        {
            get
            {
                return _dict[key];
            }

            set
            {
                VerifyNotFrozen();
                _dict[key] = value;
            }
        }

        /// <inheritdoc/>
        [CanBeNull]
        object IDictionary.this[[NotNull] object key]
        {
            get
            {
                return ((IDictionary)_dict)[key];
            }

            set
            {
                VerifyNotFrozen();
                ((IDictionary)_dict)[key] = value;
            }
        }

        /// <summary>
        /// Implicitly converts polyfill dictionary to normal BCL dictionary. No any items copy performed.
        /// </summary>
        /// <param name="dictionaryEx">Polyfill dictionary.</param>
        [CanBeNull]
        public static implicit operator Dictionary<TKey, TValue>([CanBeNull] DictionaryEx<TKey, TValue> dictionaryEx)
        {
            return dictionaryEx?._dict;
        }

        /// <summary>
        /// Implicitly converts normal BCL dictionary to polyfill dictionary. No any items copy performed.
        /// </summary>
        /// <param name="dictionary">Normal BCL dictionary.</param>
        [CanBeNull]
        public static implicit operator DictionaryEx<TKey, TValue>([CanBeNull] Dictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }

            return new DictionaryEx<TKey, TValue>(dictionary);
        }

        /// <inheritdoc/>
        public void Add([NotNull] TKey key, [CanBeNull] TValue value)
        {
            VerifyNotFrozen();
            _dict.Add(key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            VerifyNotFrozen();
            ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Add(keyValuePair);
        }

        void IDictionary.Add([NotNull] object key, [CanBeNull] object value)
        {
            VerifyNotFrozen();
            ((IDictionary)_dict).Add(key, value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            VerifyNotFrozen();
            _dict.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Contains(keyValuePair);
        }

        bool IDictionary.Contains([NotNull] object key)
        {
            return ((IDictionary)_dict).Contains(key);
        }

        /// <inheritdoc/>
        public bool ContainsKey([NotNull] TKey key)
        {
            return _dict.ContainsKey(key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo([NotNull] KeyValuePair<TKey, TValue>[] array, int index)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dict).CopyTo(array, index);
        }

        void ICollection.CopyTo([NotNull] Array array, int index)
        {
            ((ICollection)_dict).CopyTo(array, index);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dict).GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_dict).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dict).GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove([NotNull] TKey key)
        {
            VerifyNotFrozen();
            return _dict.Remove(key);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            VerifyNotFrozen();
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Remove(keyValuePair);
        }

        void IDictionary.Remove([NotNull] object key)
        {
            VerifyNotFrozen();
            ((IDictionary)_dict).Remove(key);
        }

        /// <inheritdoc/>
        public bool TryGetValue([NotNull] TKey key, [CanBeNull] out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }

        /// <summary>
        /// Verifies that the dictionary contains the specified value.
        /// </summary>
        /// <param name="value">Value to verify.</param>
        /// <returns><see langword="true"/>, if value was found, <see langword="false"/> otherwise.</returns>
        public bool ContainsValue([CanBeNull] TValue value)
        {
            return _dict.ContainsValue(value);
        }

        /// <inheritdoc/>
        public void Freeze()
        {
            IsFrozen = true;
        }

        /// <summary>
        /// Gets performance optimized dictionary enumerator.
        /// </summary>
        /// <returns>Dictionary enumerator.</returns>
        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        private void VerifyNotFrozen()
        {
            if (IsFrozen)
            {
                throw new InvalidOperationException("Cannot modify frozen collection.");
            }
        }

        /// <summary>
        /// Collection of dictionary keys.
        /// </summary>
        public sealed class KeyCollection : ICollection<TKey>,
                                            ICollection
        {
            private readonly DictionaryEx<TKey, TValue> _ownerDict;

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyCollection"/> class.
            /// </summary>
            /// <param name="ownerDict">Owner dictionary.</param>
            internal KeyCollection(DictionaryEx<TKey, TValue> ownerDict)
            {
                _ownerDict = ownerDict;
            }

            /// <inheritdoc/>
            public int Count => _ownerDict.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            bool ICollection.IsSynchronized => ((ICollection)_ownerDict).IsSynchronized;

            object ICollection.SyncRoot => ((ICollection)_ownerDict).SyncRoot;

            /// <inheritdoc/>
            public bool IsImmutable => _ownerDict.IsFrozen;

            /// <inheritdoc/>
            public IEqualityComparer<TKey> Comparer => _ownerDict.Comparer;

            void ICollection<TKey>.Add([NotNull] TKey item)
            {
                throw new NotSupportedException("Keys collection is ReadOnly.");
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException("Keys collection is ReadOnly.");
            }

            bool ICollection<TKey>.Contains([NotNull] TKey item)
            {
                return _ownerDict.ContainsKey(item);
            }

            /// <inheritdoc/>
            public void CopyTo([NotNull] TKey[] array, int index)
            {
                _ownerDict.Keys.CopyTo(array, index);
            }

            void ICollection.CopyTo([NotNull] Array array, int index)
            {
                ((ICollection)_ownerDict.Keys).CopyTo(array, index);
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return ((IEnumerable<TKey>)_ownerDict.Keys).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_ownerDict.Keys).GetEnumerator();
            }

            /// <inheritdoc/>
            public bool Remove([NotNull] TKey item)
            {
                throw new NotSupportedException("Keys collection is ReadOnly.");
            }

            /// <inheritdoc/>
            public bool Contains(TKey item)
            {
                return _ownerDict.ContainsKey(item);
            }

            /// <summary>
            /// Gets performance optimized enumerator.
            /// </summary>
            /// <returns>Dictionary keys enumerator.</returns>
            public Dictionary<TKey, TValue>.KeyCollection.Enumerator GetEnumerator()
            {
                return _ownerDict.Keys.GetEnumerator();
            }
        }

        /// <summary>
        /// Dictionary values collection.
        /// </summary>
        [PublicAPI]
        public sealed class ValueCollection : ICollectionEx<TValue>,
                                              ICollection
        {
            private readonly DictionaryEx<TKey, TValue> _ownerDict;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueCollection"/> class.
            /// </summary>
            /// <param name="ownerDict">Owner dictionary.</param>
            internal ValueCollection(DictionaryEx<TKey, TValue> ownerDict)
            {
                _ownerDict = ownerDict;
            }

            /// <inheritdoc/>
            public int Count => _ownerDict.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            bool ICollection.IsSynchronized => ((ICollection)_ownerDict).IsSynchronized;

            object ICollection.SyncRoot => ((ICollection)_ownerDict).SyncRoot;

            /// <inheritdoc/>
            public bool IsImmutable => _ownerDict.IsImmutable;

            void ICollection<TValue>.Add([CanBeNull] TValue item)
            {
                throw new NotSupportedException("Values collection is ReadOnly.");
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException("Values collection is ReadOnly.");
            }

            bool ICollection<TValue>.Contains([CanBeNull] TValue item)
            {
                return _ownerDict.ContainsValue(item);
            }

            /// <inheritdoc/>
            public void CopyTo([NotNull] TValue[] array, int index)
            {
                _ownerDict.Values.CopyTo(array, index);
            }

            void ICollection.CopyTo([NotNull] Array array, int index)
            {
                ((ICollection)_ownerDict.Values).CopyTo(array, index);
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return ((IEnumerable<TValue>)_ownerDict.Values).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_ownerDict.Values).GetEnumerator();
            }

            bool ICollection<TValue>.Remove([CanBeNull] TValue item)
            {
                throw new NotSupportedException("Values collection is ReadOnly.");
            }

            /// <summary>
            /// Gets performance optimized values enumerator.
            /// </summary>
            /// <returns>Dictionary values enumerator.</returns>
            public Dictionary<TKey, TValue>.ValueCollection.Enumerator GetEnumerator()
            {
                return new Dictionary<TKey, TValue>.ValueCollection.Enumerator();
            }
        }
    }
}