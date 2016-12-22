// <copyright file="SafeList.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// List that allows to contain only non null items.
    /// </summary>
    /// <typeparam name="T">Type of item.</typeparam>
    [PublicAPI]
    public class SafeList<T> : IList<T>, IReadOnlyList<T>, IList
        where T : class
    {
        private readonly List<T> _list = new List<T>();

        /// <inheritdoc/>
        public int Count => _list.Count;

        /// <inheritdoc/>
        public bool IsFixedSize => ((IList)_list).IsFixedSize;

        /// <inheritdoc/>
        public bool IsReadOnly => ((IList)_list).IsReadOnly;

        /// <inheritdoc/>
        public bool IsSynchronized => ((IList)_list).IsReadOnly;

        /// <inheritdoc/>
        public object SyncRoot => ((IList)_list).SyncRoot;

        /// <inheritdoc/>
        [NotNull]
        public T this[int index]
        {
            get
            {
                return _list[index];
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _list[index] = value;
            }
        }

        /// <inheritdoc/>
        [NotNull]
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                ((IList)this)[index] = value;
            }
        }

        /// <inheritdoc/>
        public void Add([NotNull] T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _list.Add(item);
        }

        /// <inheritdoc/>
        public int Add([NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ((IList)_list).Add(value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _list.Clear();
        }

        /// <inheritdoc/>
        public bool Contains([NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ((IList)_list).Contains(value);
        }

        /// <inheritdoc/>
        public bool Contains([NotNull] T item)
        {
            return _list.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(Array array, int index)
        {
            ((IList)_list).CopyTo(array, index);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf([NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ((IList)_list).IndexOf(value);
        }

        /// <inheritdoc/>
        public int IndexOf([NotNull] T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return _list.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, [NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            ((IList)_list).Insert(index, value);
        }

        /// <inheritdoc/>
        public void Insert(int index, [NotNull] T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _list.Insert(index, item);
        }

        /// <inheritdoc/>
        public void Remove([NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            ((IList)_list).Remove(value);
        }

        /// <inheritdoc/>
        public bool Remove([NotNull] T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return _list.Remove(item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
    }
}