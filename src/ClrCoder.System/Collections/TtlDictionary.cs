// <copyright file="TtlDictionary.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.System.Collections
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.CompilerServices;
    using global::System.Threading.Tasks;

    using JetBrains.Annotations;

    using Threading;

    /// <summary>
    /// <c>Dictionary</c> with items storage timeout.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    [PublicAPI]
    public class TtlDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, Entry> _entries;

        private readonly KeyedMonitor<TKey> _keyedMonitor;

        private TimeSpan? _defaultTtl;

        private long _nextVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="TtlDictionary{TKey, TValue}"/> class.
        /// </summary>
        public TtlDictionary()
        {
            _entries = new Dictionary<TKey, Entry>();
            _keyedMonitor = new KeyedMonitor<TKey>();
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                lock (_entries)
                {
                    return _entries.Count;
                }
            }
        }

        /// <summary>
        /// Default time to live(TTL) for entries. <see langword="null"/> means infinite time ti live.
        /// </summary>
        public TimeSpan? DefaultTtl
        {
            get
            {
                lock (_entries)
                {
                    return _defaultTtl;
                }
            }

            set
            {
                VerifyTtlIsValid(value);

                lock (_entries)
                {
                    _defaultTtl = value;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public ICollection<TKey> Keys
        {
            get
            {
                lock (_entries)
                {
                    return _entries.Keys.ToList();
                }
            }
        }

        /// <inheritdoc/>
        public ICollection<TValue> Values
        {
            get
            {
                lock (_entries)
                {
                    return _entries.Values.Select(x => x.Value).ToList();
                }
            }
        }

        /// <inheritdoc/>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                lock (_entries)
                {
                    return _entries.Keys.ToList();
                }
            }
        }

        /// <inheritdoc/>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                lock (_entries)
                {
                    return _entries.Values.Select(x => x.Value).ToList();
                }
            }
        }

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                // ReSharper disable once CompareNonConstrainedGenericWithNull
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                using (_keyedMonitor.Lock(key))
                {
                    lock (_entries)
                    {
                        return _entries[key].Value;
                    }
                }
            }

            set
            {
                Set(key, value, DefaultTtl);
            }
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            Add(key, value, DefaultTtl);
        }

        /// <summary>
        /// Adds an element with the provided <c>key</c> and <c>value</c> to the
        /// <see cref="TtlDictionary{TKey,TValue}"/> with the provided TTL.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <param name="ttl">The interval of time after which entry will be removed.</param>
        public void Add(TKey key, TValue value, TimeSpan? ttl)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            VerifyTtlIsValid(ttl);

            using (_keyedMonitor.Lock(key))
            {
                lock (_entries)
                {
                    if (!_entries.ContainsKey(key))
                    {
                        var newEntry = new Entry { Value = value, Version = ++_nextVersion };
                        if (ttl != null)
                        {
                            ScheduleEntryRemove(key, ref newEntry, ttl.Value);
                        }

                        _entries.Add(key, newEntry);
                    }
                    else
                    {
                        throw new ArgumentException("Dictionary already contains specified key.");
                    }
                }
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (item.Key == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using (_keyedMonitor.Lock(item.Key))
            {
                lock (_entries)
                {
                    Entry entry;
                    return _entries.TryGetValue(item.Key, out entry)
                           && Comparer<TValue>.Default.Compare(entry.Value, item.Value) == 0;
                }
            }
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (_keyedMonitor.Lock(key))
            {
                lock (_entries)
                {
                    return _entries.ContainsKey(key);
                }
            }
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index should be non negative.");
            }

            lock (_entries)
            {
                if (array.Length - arrayIndex < Count)
                {
                    throw new ArgumentException("Target array size is not enough to copy collection.");
                }

                var index = arrayIndex;
                foreach (var entry in _entries)
                {
                    array[index] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value.Value);
                    index++;
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (_entries)
            {
                return
                    _entries.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value.Value)).ToList().GetEnumerator();
            }
        }

        /// <summary>
        /// Locks entry with specified <c>key</c>. It does not matter if entry exist or not.
        /// </summary>
        /// <param name="key"><c>Entry</c> <c>key</c>.</param>
        /// <returns><c>Token</c> of the <c>lock</c>.</returns>
        public ILockToken Lock(TKey key)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _keyedMonitor.Lock(key);
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (item.Key == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using (_keyedMonitor.Lock(item.Key))
            {
                lock (_entries)
                {
                    Entry entry;
                    if (_entries.TryGetValue(item.Key, out entry)
                        && Comparer<TValue>.Default.Compare(entry.Value, item.Value) == 0)
                    {
                        return _entries.Remove(item.Key);
                    }

                    return false;
                }
            }
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (_keyedMonitor.Lock(key))
            {
                lock (_entries)
                {
                    return _entries.Remove(key);
                }
            }
        }

        /// <summary>
        /// Sets the <c>value</c> associated with the specified <c>key</c> with the specified <c>ttl</c>.
        /// </summary>
        /// <param name="key">The <c>key</c> of the <c>value</c> to set.</param>
        /// <param name="value">The <c>value</c> to set.</param>
        /// <param name="ttl">Time to live of the entry.</param>
        public void Set(TKey key, TValue value, TimeSpan? ttl)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            VerifyTtlIsValid(ttl);

            using (_keyedMonitor.Lock(key))
            {
                lock (_entries)
                {
                    var newEntry = new Entry { Value = value, Version = ++_nextVersion };

                    if (ttl != null)
                    {
                        ScheduleEntryRemove(key, ref newEntry, ttl.Value);
                    }

                    _entries[key] = newEntry;
                }
            }
        }

        /// <summary>
        /// Tries to get <c>value</c> by the specified <c>key</c> and remove <c>this</c> <c>value</c>.
        /// </summary>
        /// <param name="key">Search <c>key</c>.</param>
        /// <param name="value">Found <c>value</c>.</param>
        /// <returns><see langword="true"/> if value was found, otherwise <see langword="false"/>.</returns>
        public bool TryGetAndRemoveValue(TKey key, out TValue value)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (_keyedMonitor.Lock(key))
            {
                lock (_entries)
                {
                    Entry entry;
                    var result = _entries.TryGetValue(key, out entry);
                    value = result ? entry.Value : default(TValue);

                    return result;
                }
            }
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (_keyedMonitor.Lock(key))
            {
                lock (_entries)
                {
                    Entry entry;
                    var result = _entries.TryGetValue(key, out entry);
                    value = result ? entry.Value : default(TValue);
                    _entries.Remove(key);
                    return result;
                }
            }
        }

        /// <inheritdoc/>
        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private void ScheduleEntryRemove(TKey key, ref Entry entry, TimeSpan ttl)
        {
#pragma warning disable 4014
            ScheduleEntryRemove(key, entry.Version, ttl);
#pragma warning restore 4014
        }

        private async Task ScheduleEntryRemove(TKey key, long version, TimeSpan ttl)
        {
            await Task.Delay(ttl).ConfigureAwait(false);

            using (_keyedMonitor.Lock(key))
            {
                lock (_entries)
                {
                    Entry entry;
                    if (_entries.TryGetValue(key, out entry) && entry.Version == version)
                    {
                        _entries.Remove(key);
                    }
                }
            }
        }

        private void VerifyTtlIsValid(TimeSpan? ttl)
        {
            if (ttl != null && ttl.Value < TimeSpan.Zero)
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentOutOfRangeException("value", "Time to live should be grater than zero.");
            }
        }

        private struct Entry
        {
            public TValue Value;

            public long Version;
        }
    }
}