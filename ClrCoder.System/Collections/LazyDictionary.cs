using System;
using System.Collections;
using System.Collections.Generic;

namespace ClrCoder.System.Collections
{
    /// <summary>
    /// <c>Dictionary</c>, which automatically creates values for the requested keys.
    /// </summary>
    /// <remarks>
    /// All methods and properties are not thread safe.
    /// </remarks>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class LazyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        ////[CanBeNull]
        private static readonly Func<TValue> DefaultValueFactory;

        ////[CanBeNull]
        // ReSharper disable once StaticFieldInGenericType
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
        /// Initializes a new instance of the class <see cref="LazyDictionary{K,V}"/>.
        /// </summary>
        /// <param name="valueFactory">Value factory method.</param>
        public LazyDictionary(Func<TValue> valueFactory)
        {
            _valueFactory = valueFactory;
            Keys = _dictionary.Keys;
            Values = _dictionary.Values;
        }

        /// <summary>
        /// Initializes a new instance of the class <see cref="LazyDictionary{K,V}"/>.
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
        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public ICollection<TKey> Keys { get; private set; }

        /// <inheritdoc/>
        public ICollection<TValue> Values { get; private set; }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                return _dictionary.Values;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                return _dictionary.Keys;
            }
        }

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

        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return _dictionary[key];
            }
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
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
        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
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

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}