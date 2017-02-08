// <copyright file="ProcessableSet.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Set that allows all items to be processed, <c>while</c> processing can change collection.
    /// </summary>
    /// <remarks>Collection is not thread-safe.</remarks>
    /// <typeparam name="T">Item type.</typeparam>
    [PublicAPI]
    public class ProcessableSet<T> : ISet<T>, IReadOnlyCollection<T>
    {
        [CanBeNull]
        private HashSet<T> _processed;

        private HashSet<T> _inner = new HashSet<T>();

        [CanBeNull]
        private Action<T> _processAction;

        /// <inheritdoc/>
        public int Count => _inner.Count + _processed?.Count ?? 0;

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => ((ISet<T>)_inner).IsReadOnly;

        /// <inheritdoc/>
        void ICollection<T>.Add([NotNull] T item)
        {
            // Reenterant method.
            // ----------------------------------------------
            if (_processAction != null)
            {
                Debug.Assert(_processed != null, "In processing state, processed collection should be not null.");

                if (_processed.Add(item))
                {
                    // Probably not processed collection already have this item.
                    _inner.Remove(item);

                    _processAction(item);
                }
            }
            else
            {
                _inner.Add(item);
            }
        }

        /// <inheritdoc/>
        public bool Add([NotNull] T item)
        {
            // Reenterant method.
            // ----------------------------------------------
            if (_processAction != null)
            {
                Debug.Assert(_processed != null, "In processing state, processed collection should be not null.");

                var result = true;
                if (_processed.Add(item))
                {
                    // Probably not processed collection already have this item.
                    result = !_inner.Remove(item);

                    // Can produce reentrancy.
                    _processAction(item);
                }

                return result;
            }

            return _inner.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            if (_processAction != null)
            {
                Debug.Assert(_processed != null, "In processing state, processed collection should be not null.");

                _processed.Clear();
                _inner.Clear();
            }

            _inner.Clear();
        }

        /// <inheritdoc/>
        public bool Contains([NotNull] T item)
        {
            if (_processAction != null)
            {
                Debug.Assert(_processed != null, "In processing state, processed collection should be not null.");

                return _processed.Contains(item) || _inner.Contains(item);
            }

            return _inner.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo([NotNull]T[] array, int arrayIndex)
        {
            if (_processAction != null)
            {
                Debug.Assert(_processed != null, "In processing state, processed collection should be not null.");

                _inner.CopyTo(array, arrayIndex);
                _processed.CopyTo(array, arrayIndex + _inner.Count);
            }

            _inner.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public void ExceptWith([NotNull] IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Count == 0)
            {
                return;
            }

            if (Equals(other, this))
            {
                Clear();
            }
            else
            {
                foreach (T obj in other)
                {
                    Remove(obj);
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            if (_processAction != null)
            {
                return _inner.Concat(_processed).GetEnumerator();
            }

            return ((IEnumerable<T>)_inner).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public void IntersectWith([NotNull] IEnumerable<T> other)
        {
            throw new NotSupportedException("Temporary not supported.");
        }

        /// <inheritdoc/>
        public bool IsProperSubsetOf([NotNull] IEnumerable<T> other)
        {
            throw new NotSupportedException("Temporary not supported.");
        }

        /// <inheritdoc/>
        public bool IsProperSupersetOf([NotNull] IEnumerable<T> other)
        {
            throw new NotSupportedException("Temporary not supported.");
        }

        /// <inheritdoc/>
        public bool IsSubsetOf([NotNull] IEnumerable<T> other)
        {
            throw new NotSupportedException("Temporary not supported.");
        }

        /// <inheritdoc/>
        public bool IsSupersetOf([NotNull] IEnumerable<T> other)
        {
            throw new NotSupportedException("Temporary not supported.");
        }

        /// <inheritdoc/>
        public bool Overlaps([NotNull] IEnumerable<T> other)
        {
            throw new NotSupportedException("Temporary not supported.");
        }

        /// <inheritdoc/>
        public bool Remove([NotNull] T item)
        {
            if (_processAction != null)
            {
                Debug.Assert(_processed != null, "In processing state, processed collection should be not null.");

                return _processed.Remove(item) || _inner.Remove(item);
            }

            return _inner.Remove(item);
        }

        /// <inheritdoc/>
        public bool SetEquals([NotNull] IEnumerable<T> other)
        {
            throw new NotSupportedException("Temporary not supported.");
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith([NotNull] IEnumerable<T> other)
        {
            throw new NotSupportedException("Temporary not supported.");
        }

        /// <inheritdoc/>
        public void UnionWith([NotNull] IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (T obj in other)
            {
                Add(obj);
            }
        }

        /// <summary>
        /// Performs operation on all items.
        /// </summary>
        /// <param name="action">Operation on item. This operation can remove some items or add another.</param>
        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (_processAction != null)
            {
                throw new InvalidOperationException("Cannot perform nested processing.");
            }

            _processAction = action;
            _processed = new HashSet<T>();

            try
            {
                while (_inner.Any())
                {
                    var itemToProcess = _inner.First();
                   
                    Debug.Assert(_processAction != null, "_processAction should be not null while ForEachMethod not exited.");
                    _processAction(itemToProcess);
                }
            }
            finally
            {
                _processAction = null;
                Debug.Assert(_processed != null, "_processed should be not null while ForEachMethod not exited.");
                _processed.UnionWith(_inner);
                _inner = _processed;
                _processed = null;
            }
        }
    }
}