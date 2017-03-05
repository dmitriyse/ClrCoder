// <copyright file="HashSetEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    /// <summary>
    /// Represents a set of values.<see cref="HashSet{T}"/>  <br/>
    /// Polyfill of comming new features.
    /// </summary>
    /// <remarks>
    /// This implementation follows proposal described here https://github.com/dotnet/corefx/issues/1973 .<br/>
    /// Adds IFreezable to standard BCL set.
    /// Implements new <see cref="IReadOnlySet{T}"/>, <see cref="IReadOnlySet{T}"/> interfaces.
    /// Implement other improvements in base contracts like https://github.com/dotnet/corefx/issues/16660 
    /// </remarks>
    /// <typeparam name="T">Item type.</typeparam>
    [PublicAPI]
    public class HashSetEx<T> : ISet<T>, IReadOnlySet<T>, IFreezable<HashSetEx<T>>
    {
        private readonly HashSet<T> _hashSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSetEx{T}"/> class.
        /// </summary>
        public HashSetEx()
        {
            _hashSet = new HashSet<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSetEx{T}"/> class.
        /// </summary>
        /// <param name="collection">Items to be added to set.</param>
        public HashSetEx(IEnumerable<T> collection)
        {
            _hashSet = new HashSet<T>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSetEx{T}"/> class.
        /// </summary>
        /// <param name="collection">Items to be added to set.</param>
        /// <param name="comparer">Items equality comparer.</param>
        public HashSetEx(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _hashSet = new HashSet<T>(collection, comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSetEx{T}"/> class.
        /// </summary>
        /// <param name="comparer">Items equality comparer.</param>
        public HashSetEx(IEqualityComparer<T> comparer)
        {
            _hashSet = new HashSet<T>(comparer);
        }

        private HashSetEx(HashSet<T> hashSet)
        {
            _hashSet = hashSet;
        }

        /// <inheritdoc/>
        public int Count => _hashSet.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => IsFrozen;

        /// <inheritdoc/>
        public bool IsFrozen { get; private set; }

        /// <inheritdoc/>
        public bool IsImmutable => IsFrozen;

        /// <inheritdoc/>
        public IEqualityComparer<T> Comparer => _hashSet.Comparer;

        /// <summary>
        /// Converts polyfill to classic hashset.
        /// </summary>
        /// <remarks>
        /// If frozen collection will be modified after <see langword="this"/> conversion,
        /// </remarks>
        /// <param name="setEx">Classic set.</param>
        [CanBeNull]
        public static implicit operator HashSet<T>([CanBeNull] HashSetEx<T> setEx)
        {
            return setEx?._hashSet;
        }

        /// <summary>
        /// Converts classic <c>set</c> to polyfill.
        /// </summary>
        /// <remarks>
        /// If frozen collection will be modified after <see langword="this"/> conversion,
        /// </remarks>
        /// <param name="set">Polyfill set.</param>
        [CanBeNull]
        public static implicit operator HashSetEx<T>([CanBeNull] HashSet<T> set)
        {
            if (set == null)
            {
                return null;
            }

            return new HashSetEx<T>(set);
        }

        /// <inheritdoc/>
        public void Add([NotNull] T item)
        {
            VerifyNotFrozen();
            _hashSet.Add(item);
        }

        bool ISet<T>.Add([NotNull] T item)
        {
            VerifyNotFrozen();
            return _hashSet.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            VerifyNotFrozen();
            _hashSet.Clear();
        }

        /// <inheritdoc/>
        public bool Contains([NotNull] T item)
        {
            return _hashSet.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo([NotNull] T[] array, int arrayIndex)
        {
            _hashSet.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public void ExceptWith([NotNull] IEnumerable<T> other)
        {
            _hashSet.ExceptWith(other);
        }

        /// <inheritdoc/>
        public void Freeze()
        {
            IsFrozen = true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _hashSet.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)_hashSet).GetEnumerator();
        }

        /// <inheritdoc/>
        public void IntersectWith([NotNull] IEnumerable<T> other)
        {
            VerifyNotFrozen();
            _hashSet.IntersectWith(other);
        }

        /// <inheritdoc/>
        public bool IsProperSubsetOf([NotNull] IEnumerable<T> other)
        {
            return _hashSet.IsProperSubsetOf(other);
        }

        /// <inheritdoc/>
        public bool IsProperSupersetOf([NotNull] IEnumerable<T> other)
        {
            return _hashSet.IsProperSupersetOf(other);
        }

        /// <inheritdoc/>
        public bool IsSubsetOf([NotNull] IEnumerable<T> other)
        {
            return _hashSet.IsSubsetOf(other);
        }

        /// <inheritdoc/>
        public bool IsSupersetOf([NotNull] IEnumerable<T> other)
        {
            return _hashSet.IsSupersetOf(other);
        }

        /// <inheritdoc/>
        public bool Overlaps([NotNull] IEnumerable<T> other)
        {
            return _hashSet.Overlaps(other);
        }

        /// <inheritdoc/>
        public bool Remove([NotNull] T item)
        {
            VerifyNotFrozen();
            return _hashSet.Remove(item);
        }

        /// <inheritdoc/>
        public bool SetEquals([NotNull] IEnumerable<T> other)
        {
            return _hashSet.SetEquals(other);
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith([NotNull] IEnumerable<T> other)
        {
            VerifyNotFrozen();
            _hashSet.SymmetricExceptWith(other);
        }

        /// <inheritdoc/>
        public void UnionWith([NotNull] IEnumerable<T> other)
        {
            VerifyNotFrozen();
        }

        /// <summary>
        /// Copies items to an <c>array</c>.
        /// </summary>
        /// <param name="array">Target <c>array</c>.</param>
        public void CopyTo(T[] array)
        {
            _hashSet.CopyTo(array);
        }

        /// <summary>
        /// Copies items to an <c>array</c>.
        /// </summary>
        /// <param name="array">Target <c>array</c>.</param>
        /// <param name="arrayIndex">Target starting index in the <c>array</c>.</param>
        /// <param name="count">Amount of items to copy.</param>
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            _hashSet.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets set enumerator.
        /// </summary>
        /// <remarks>This implementation allows <see langword="foreach"/> to be most performance efficient.</remarks>
        /// <returns>New enumerator for <see langword="this"/> set.</returns>
        public HashSet<T>.Enumerator GetEnumerator()
        {
            return _hashSet.GetEnumerator();
        }

        private void VerifyNotFrozen()
        {
            if (IsFrozen)
            {
                throw new InvalidOperationException("Cannot modify frozen collection.");
            }
        }
    }
}