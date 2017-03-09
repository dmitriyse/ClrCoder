// <copyright file="SetExWrapper.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    /// <summary>
    /// Wrapper over classic <see cref="ISet{T}"/> contract, that provides implementation of additional contracts (
    /// <see cref="ISet{T}"/>, <see cref="IReadableSet{T}"/> and <see cref="IReadOnlySet{T}"/>).
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    public class SetExWrapper<T> : ISetEx<T>
    {
        private readonly IEqualityComparer<T> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SetExWrapper`1"/> class.
        /// </summary>
        /// <param name="inner">The set to wrap.</param>
        /// <param name="comparer">Equality comparer that is used by the inner set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="inner"/> is null.
        /// </exception>
        public SetExWrapper(ISet<T> inner, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            Inner = inner;

            _comparer = comparer;

            if (comparer == null)
            {
                _comparer = (inner as HashSet<T>)?.Comparer;
            }

            if (comparer == null)
            {
                _comparer = (inner as HashSetExWrapper<T>)?.Comparer;
            }
        }

        /// <inheritdoc/>
        public int Count => Inner.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => Inner.IsReadOnly;

        /// <inheritdoc/>
        public IEqualityComparer<T> Comparer
        {
            get
            {
                if (_comparer == null)
                {
                    throw new InvalidOperationException("Cannot detect comparer that is used by the wrapped set.");
                }

                return _comparer;
            }
        }

        /// <summary>
        /// Wrapped set.
        /// </summary>
        public ISet<T> Inner { get; }

        /// <inheritdoc/>
        public void Add([NotNull] T item)
        {
            Inner.Add(item);
        }

        /// <inheritdoc/>
        bool ISet<T>.Add([NotNull] T item)
        {
            return Inner.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Inner.Clear();
        }

        /// <inheritdoc/>
        public bool Contains([NotNull] T item)
        {
            return Inner.Contains(item);
        }

        /// <inheritdoc/>
        public bool Contains<TItem>(TItem item)
        {
            // TODO: Some optimization possible here. For valued types.
            if (item is T)
            {
                return Contains((T)(object)item);
            }

            return false;
        }

        /// <inheritdoc/>
        public void CopyTo([NotNull] T[] array, int arrayIndex)
        {
            Inner.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public void ExceptWith([NotNull] IEnumerable<T> other)
        {
            Inner.ExceptWith(other);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return Inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Inner).GetEnumerator();
        }

        /// <inheritdoc/>
        public void IntersectWith([NotNull] IEnumerable<T> other)
        {
            Inner.IntersectWith(other);
        }

        /// <inheritdoc/>
        public bool IsProperSubsetOf([NotNull] IEnumerable<T> other)
        {
            return Inner.IsProperSubsetOf(other);
        }

        /// <inheritdoc/>
        public bool IsProperSupersetOf([NotNull] IEnumerable<T> other)
        {
            return Inner.IsProperSupersetOf(other);
        }

        /// <inheritdoc/>
        public bool IsSubsetOf([NotNull] IEnumerable<T> other)
        {
            return Inner.IsSubsetOf(other);
        }

        /// <inheritdoc/>
        public bool IsSupersetOf([NotNull] IEnumerable<T> other)
        {
            return Inner.IsSupersetOf(other);
        }

        /// <inheritdoc/>
        public bool Overlaps([NotNull] IEnumerable<T> other)
        {
            return Inner.Overlaps(other);
        }

        /// <inheritdoc/>
        public bool Remove([NotNull] T item)
        {
            return Inner.Remove(item);
        }

        /// <inheritdoc/>
        public bool SetEquals([NotNull] IEnumerable<T> other)
        {
            return Inner.SetEquals(other);
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith([NotNull] IEnumerable<T> other)
        {
            Inner.SymmetricExceptWith(other);
        }

        /// <inheritdoc/>
        public void UnionWith([NotNull] IEnumerable<T> other)
        {
            Inner.UnionWith(other);
        }
    }
}