// <copyright file="SetExWrapper.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    //// ReSharper disable ImplicitNotNullOverridesUnknownExternalMember

    /// <summary>
    /// Wrapper over classic <see cref="ISet{T}"/> contract, that provides implementation of additional contracts (
    /// <see cref="IReadableSet{T}"/> and <see cref="IReadOnlySet{T}"/>).
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
        public SetExWrapper(ISet<T> inner, IEqualityComparer<T> comparer = null)
        {
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));

            // ReSharper disable once AssignNullToNotNullAttribute
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

        /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
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
        public void Add(T item)
        {
            Inner.Add(item);
        }

        /// <inheritdoc/>
        bool ISet<T>.Add(T item)
        {
            return Inner.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Inner.Clear();
        }

        /// <inheritdoc cref="ICollection{T}.Contains"/>
        public bool Contains(T item)
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
        public void CopyTo(T[] array, int arrayIndex)
        {
            Inner.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<T> other)
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
        public void IntersectWith(IEnumerable<T> other)
        {
            Inner.IntersectWith(other);
        }

        /// <inheritdoc cref="ISet{T}.IsProperSubsetOf"/>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return Inner.IsProperSubsetOf(other);
        }

        /// <inheritdoc cref="ISet{T}.IsProperSupersetOf"/>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Inner.IsProperSupersetOf(other);
        }

        /// <inheritdoc cref="ISet{T}.IsSubsetOf"/>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return Inner.IsSubsetOf(other);
        }

        /// <inheritdoc cref="ISet{T}.IsSupersetOf"/>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return Inner.IsSupersetOf(other);
        }

        /// <inheritdoc cref="ISet{T}.Overlaps"/>
        public bool Overlaps(IEnumerable<T> other)
        {
            return Inner.Overlaps(other);
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            return Inner.Remove(item);
        }

        /// <inheritdoc cref="ISet{T}.SetEquals"/>
        public bool SetEquals(IEnumerable<T> other)
        {
            return Inner.SetEquals(other);
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            Inner.SymmetricExceptWith(other);
        }

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<T> other)
        {
            Inner.UnionWith(other);
        }
    }
}