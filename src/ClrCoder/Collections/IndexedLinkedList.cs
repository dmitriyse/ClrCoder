// <copyright file="IndexedLinkedList.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// The linked list of the <typeparamref name="TValue"/> items with maintained index by <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the item.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class IndexedLinkedList<TKey, TValue> : IDictionaryEx<TKey, TValue>, IDictionary
    {
        private LinkedList<KeyValuePair<TKey, TValue>> _list = new LinkedList<KeyValuePair<TKey, TValue>>();

        private DictionaryEx<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _index;

        [CanBeNull]
        private ValueCollection _valueCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedLinkedList{TKey,TValue}"/> class.
        /// </summary>
        public IndexedLinkedList()
        {
            _index = new DictionaryEx<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedLinkedList{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="capacity">The initial collection capacity.</param>
        public IndexedLinkedList(int capacity)
        {
            _index = new DictionaryEx<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedLinkedList{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when
        /// comparing keys, or null to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1"/> for the type
        /// of the key.
        /// </param>
        public IndexedLinkedList([CanBeNull] IEqualityComparer<TKey> comparer)
        {
            _index = new DictionaryEx<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(comparer);
        }

        bool IDictionary.IsReadOnly => false;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _index.Keys;

        ICollection IDictionary.Values =>
            _valueCollection
            ?? (_valueCollection = new ValueCollection(this));

        ICollection IDictionary.Keys => _index.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _list.Select(kvp => kvp.Value);

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _index.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values =>
            _valueCollection
            ?? (_valueCollection = new ValueCollection(this));

        /// <inheritdoc cref="ICollection.Count"/>
        public int Count => _index.Count;

        /// <inheritdoc/>
        public IEqualityComparer<TKey> Comparer => _index.Comparer;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((ICollection)_index).SyncRoot;

        bool IDictionary.IsFixedSize => false;

        /// <summary>
        /// Gets the first item or null, if there are no any items in the list.
        /// </summary>
        public KeyValuePair<TKey, TValue>? First => _list.First?.Value;

        /// <summary>
        /// Gets the last item or null, if there are no any items in the list.
        /// </summary>
        public KeyValuePair<TKey, TValue>? Last => _list.Last?.Value;

        object IDictionary.this[[NotNull] object key]
        {
            get
            {
                KeyValuePair<TKey, TValue>? kvp =
                    (((IDictionary)_index)[key] as LinkedListNode<KeyValuePair<TKey, TValue>>)?.Value;
                if (kvp.HasValue)
                {
                    return kvp.Value.Value;
                }

                return null;
            }

            set
            {
                try
                {
                    var tempKey = (TKey)key;
                    try
                    {
                        this[tempKey] = (TValue)value;
                    }
                    catch (InvalidCastException)
                    {
                        throw new ArgumentException(
                            $"The value \"{key}\" is not of type \"{typeof(TKey)}\" and cannot be used in this generic collection",
                            nameof(key));
                    }
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(
                        $"The value \"{value}\" is not of type \"{typeof(TValue)}\" and cannot be used in this generic collection",
                        nameof(value));
                }
            }
        }

        /// <inheritdoc/>
        public TValue this[[NotNull] TKey key]
        {
            get => _index[key].Value.Value;

            set => _index[key].Value = new KeyValuePair<TKey, TValue>(key, value);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            if (_index.ContainsKey(item.Key))
            {
                throw new ArgumentException("An element with the same key already exists in the list.", nameof(item));
            }

            _index.Add(item.Key, _list.AddLast(item));
        }

        /// <inheritdoc/>
        void IDictionary<TKey, TValue>.Add([NotNull] TKey key, [CanBeNull] TValue value)
        {
            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("An element with the same key already exists in the list.", nameof(key));
            }

            _index.Add(key, _list.AddLast(new KeyValuePair<TKey, TValue>(key, value)));
        }

        void IDictionary.Add([NotNull] object key, [CanBeNull] object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                var tempKey = (TKey)key;

                try
                {
                    AddLast(tempKey, (TValue)value);
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(
                        $"The value \"{key}\" is not of type \"{typeof(TKey)}\" and cannot be used in this generic collection",
                        nameof(key));
                }
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(
                    $"The value \"{value}\" is not of type \"{typeof(TValue)}\" and cannot be used in this generic collection",
                    nameof(value));
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _index.Clear();
            _list.Clear();
        }

        bool IDictionary.Contains([NotNull] object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key is TKey)
            {
                return ContainsKey((TKey)key);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item) =>
            _index.TryGetValue(item.Key, out var listNode)
            && EqualityComparer<TValue>.Default.Equals(
                listNode.Value.Value,
                item.Value);

        /// <inheritdoc/>
        public bool ContainsKey([CanBeNull] TKey key)
        {
            return _index.ContainsKey(key);
        }

        /// <inheritdoc/>
        public void CopyTo([NotNull] KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            _list.CopyTo(
                array,
                arrayIndex);

        void ICollection.CopyTo([NotNull] Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(_list.GetEnumerator());
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            _index
                .Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value.Value.Value)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IDictionary.Remove([NotNull] object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key is TKey)
            {
                Remove((TKey)key);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_index.TryGetValue(item.Key, out var node)
                && EqualityComparer<TValue>.Default.Equals(item.Value, node.Value.Value))
            {
                _list.Remove(node);
                _index.Remove(item.Key);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Remove([NotNull] TKey key)
        {
            if (_index.TryGetValue(key, out var node))
            {
                _list.Remove(node);
                _index.Remove(key);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TryGetValue([NotNull] TKey key, [CanBeNull] out TValue value)
        {
            if (_index.TryGetValue(key, out var node))
            {
                value = node.Value.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Adds a new item after the specified item.
        /// </summary>
        /// <param name="searchKey">The key of the item to insert after.</param>
        /// <param name="key">The new item key.</param>
        /// <param name="value">The new item value.</param>
        public void AddAfter(TKey searchKey, TKey key, [CanBeNull] TValue value)
        {
            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("An element with the same key already exists in the list.", nameof(key));
            }

            _index.Add(key, _list.AddAfter(_index[searchKey], new KeyValuePair<TKey, TValue>(key, value)));
        }

        /// <summary>
        /// Adds a new item before the specified item.
        /// </summary>
        /// <param name="searchKey">The key of the item to insert before.</param>
        /// <param name="key">The new item key.</param>
        /// <param name="value">The new item value.</param>
        public void AddBefore(TKey searchKey, TKey key, [CanBeNull] TValue value)
        {
            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("An element with the same key already exists in the list.", nameof(key));
            }

            _index.Add(key, _list.AddAfter(_index[searchKey], new KeyValuePair<TKey, TValue>(key, value)));
        }

        /// <summary>
        /// Adds a new item to the start of the list.
        /// </summary>
        /// <param name="key">The new item key.</param>
        /// <param name="value">The new item value.</param>
        public void AddFirst(TKey key, [CanBeNull] TValue value)
        {
            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("An element with the same key already exists in the list.", nameof(key));
            }

            _index.Add(key, _list.AddFirst(new KeyValuePair<TKey, TValue>(key, value)));
        }

        /// <summary>
        /// Adds a new item to the end of the list.
        /// </summary>
        /// <param name="key">The new item key.</param>
        /// <param name="value">The new item value.</param>
        public void AddLast(TKey key, [CanBeNull] TValue value)
        {
            if (_index.ContainsKey(key))
            {
                throw new ArgumentException("An element with the same key already exists in the list.", nameof(key));
            }

            _index.Add(key, _list.AddLast(new KeyValuePair<TKey, TValue>(key, value)));
        }

        /// <summary>
        /// Determines whether the <see cref="IndexedLinkedList{TKey,TValue}"/> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="IndexedLinkedList{TKey,TValue}"/>. The value can
        /// be <see langword="null"/> for reference types.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="IndexedLinkedList{TKey,TValue}"/> contains an element with the
        /// specified value; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsValue([CanBeNull] TValue value)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in _list)
            {
                if (EqualityComparer<TValue>.Default.Equals(kvp.Value, value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the first node in the list.
        /// </summary>
        /// <returns>
        /// true, if the element is successfully found and removed; otherwise, false. This method returns false if key is
        /// not found in the <see cref="IndexedLinkedList{TKey,TValue}"/>.
        /// </returns>
        public bool RemoveFirst()
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> node = _list.First;
            if (node != null)
            {
                _index.Remove(node.Value.Key);
                _list.Remove(node);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the last node in the list.
        /// </summary>
        /// <returns>
        /// true, if the element is successfully found and removed; otherwise, false. This method returns false if key is
        /// not found in the <see cref="IndexedLinkedList{TKey,TValue}"/>.
        /// </returns>
        public bool RemoveLast()
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> node = _list.Last;
            if (node != null)
            {
                _index.Remove(node.Value.Key);
                _list.Remove(node);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to receive and delete a last item.
        /// </summary>
        /// <param name="item">The dequeued item.</param>
        /// <returns></returns>
        public bool TryDequeue(out KeyValuePair<TKey, TValue> item)
        {
            if (_index.Count > 0)
            {
                item = _list.Last.Value;
                RemoveLast();
                return true;
            }

            item = default;
            return false;
        }

        /// <summary>
        /// Tries to get next item in the list.
        /// </summary>
        /// <param name="key">The key of the item to get next item for.</param>
        /// <param name="item">The next item relative to the specified item key.</param>
        /// <returns>true, if the list contains elements after the specified item key, otherwise false.</returns>
        /// <exception cref="KeyNotFoundException">Key should exists in the list.</exception>
        public bool TryGetNext(TKey key, out KeyValuePair<TKey, TValue> item)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> nextNode = _index[key].Next;
            if (nextNode != null)
            {
                item = nextNode.Value;
                return true;
            }

            item = default(KeyValuePair<TKey, TValue>);
            return false;
        }

        /// <summary>
        /// Tries to get previous item in the list.
        /// </summary>
        /// <param name="key">The key of the item to get next item for.</param>
        /// <param name="item">The previous item relative to the specified item key.</param>
        /// <returns>true, if the list contains elements before the specified item key, otherwise false.</returns>
        /// <exception cref="KeyNotFoundException">Key should exists in the list.</exception>
        public bool TryGetPrevious(TKey key, out KeyValuePair<TKey, TValue> item)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> previousNode = _index[key].Previous;
            if (previousNode != null)
            {
                item = previousNode.Value;
                return true;
            }

            item = default(KeyValuePair<TKey, TValue>);
            return false;
        }

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
        [Serializable]
#endif
        private struct Enumerator : IDictionaryEnumerator, IDisposable
        {
            private LinkedList<KeyValuePair<TKey, TValue>>.Enumerator _enumerator;

            internal Enumerator(LinkedList<KeyValuePair<TKey, TValue>>.Enumerator enumerator)
            {
                _enumerator = enumerator;
            }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                _enumerator.Dispose();
            }

            object IEnumerator.Current => new DictionaryEntry(_enumerator.Current.Key, _enumerator.Current.Value);

            void IEnumerator.Reset()
            {
                ((IEnumerator)_enumerator).Reset();
            }

            DictionaryEntry IDictionaryEnumerator.Entry =>
                new DictionaryEntry(
                    _enumerator.Current.Key,
                    _enumerator.Current.Value);

            object IDictionaryEnumerator.Key => _enumerator.Current.Key;

            object IDictionaryEnumerator.Value => _enumerator.Current.Value;
        }

        private class ValueCollection : ICollection<TValue>, ICollection
        {
            private readonly IndexedLinkedList<TKey, TValue> _owner;

            public ValueCollection(IndexedLinkedList<TKey, TValue> owner)
            {
                _owner = owner;
            }

            public int Count => _owner.Count;

            public bool IsSynchronized => ((ICollection)_owner).IsSynchronized;

            public object SyncRoot => ((ICollection)_owner).SyncRoot;

            public bool IsReadOnly => true;

            public void Add([CanBeNull] TValue item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains([CanBeNull] TValue item)
            {
                return _owner.ContainsValue(item);
            }

            public void CopyTo([NotNull] Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Only an array with rank = 1 is supported.");
                }

                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException("Only an array with zero lower bound is supported");
                }

                if ((index < 0) || (index > array.Length))
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (array.Length - index < Count)
                {
                    throw new ArgumentException(
                        "Destination array is not long enough to copy all the items in the collection. Check array index and length.",
                        nameof(index));
                }

                foreach (KeyValuePair<TKey, TValue> kvp in _owner._list)
                {
                    array.SetValue(kvp.Value, index);
                }
            }

            public void CopyTo([NotNull] TValue[] array, int arrayIndex)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if ((arrayIndex < 0) || (arrayIndex > array.Length))
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                }

                if (array.Length - arrayIndex < Count)
                {
                    throw new ArgumentException(
                        "Destination array is not long enough to copy all the items in the collection. Check array index and length.",
                        nameof(arrayIndex));
                }

                foreach (KeyValuePair<TKey, TValue> kvp in _owner._list)
                {
                    array[arrayIndex++] = kvp.Value;
                }
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return _owner._list.Select(kvp => kvp.Value).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Remove([CanBeNull] TValue item)
            {
                throw new NotSupportedException();
            }
        }
    }
}