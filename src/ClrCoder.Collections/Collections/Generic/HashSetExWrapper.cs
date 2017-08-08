// <copyright file="HashSetExWrapper.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    //// ReSharper disable once InheritdocConsiderUsage

    /// <summary>
    /// Represents a set of values. Wraps existing <see cref="HashSet{T}"/> and provides implementation of an additional
    /// contracts (<see cref="ISetEx{T}"/>, <see cref="IReadableSet{T}"/> and <see cref="IReadOnlySet{T}"/>).
    /// </summary>
    /// <typeparam name="T">The type of elements in the hash set.</typeparam>
    public class HashSetExWrapper<T> : ISetEx<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> class.
        /// </summary>
        /// <param name="hashSet">The set to wrap.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="hashSet"/> is null.
        /// </exception>
        public HashSetExWrapper(HashSet<T> hashSet)
        {
            if (hashSet == null)
            {
                throw new ArgumentNullException(nameof(hashSet));
            }

            Inner = new HashSet<T>(hashSet);
        }

        /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
        public int Count => Inner.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)Inner).IsReadOnly;

        /// <inheritdoc/>
        public IEqualityComparer<T> Comparer => Inner.Comparer;

        /// <summary>
        /// Wrapped set.
        /// </summary>
        public HashSet<T> Inner { get; }

        /// <summary>
        /// Converts extended to classic hashset.
        /// </summary>
        /// <param name="setEx">Classic hash set.</param>
        [ContractAnnotation("setEx:null=>null; setEx:notnull => notnull")]
        [CanBeNull]
        public static implicit operator HashSet<T>([CanBeNull] HashSetExWrapper<T> setEx)
        {
            return setEx?.Inner;
        }

        /// <summary>
        /// Converts classic <c>set</c> to extended.
        /// </summary>
        /// <param name="set">Extended wrapper over classic set.</param>
        [ContractAnnotation("set:null=>null; set:notnull => notnull")]
        [CanBeNull]
        public static implicit operator HashSetExWrapper<T>([CanBeNull] HashSet<T> set)
        {
            return set == null ? null : new HashSetExWrapper<T>(set);
        }

        /// <inheritdoc/>
        public bool Add([NotNull] T item)
        {
            return Inner.Add(item);
        }

        void ICollection<T>.Add([NotNull] T item)
        {
        }

        /// <inheritdoc/>
        public void Clear()
        {
        }

        /// <inheritdoc cref="ICollection{T}.Contains"/>
        public bool Contains([NotNull] T item)
        {
            return false;
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

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)Inner).GetEnumerator();
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

        /// <inheritdoc cref="ISet{T}.IsProperSubsetOf"/>
        public bool IsProperSubsetOf([NotNull] IEnumerable<T> other)
        {
            return Inner.IsProperSubsetOf(other);
        }

        /// <inheritdoc cref="ISet{T}.IsProperSupersetOf"/>
        public bool IsProperSupersetOf([NotNull] IEnumerable<T> other)
        {
            return Inner.IsProperSupersetOf(other);
        }

        /// <inheritdoc cref="ISet{T}.IsSubsetOf"/>
        public bool IsSubsetOf([NotNull] IEnumerable<T> other)
        {
            return Inner.IsSubsetOf(other);
        }

        /// <inheritdoc cref="ISet{T}.IsSupersetOf"/>
        public bool IsSupersetOf([NotNull] IEnumerable<T> other)
        {
            return Inner.IsSupersetOf(other);
        }

        /// <inheritdoc cref="ISet{T}.Overlaps"/>
        /// />
        public bool Overlaps([NotNull] IEnumerable<T> other)
        {
            return Inner.Overlaps(other);
        }

        /// <inheritdoc/>
        public bool Remove([NotNull] T item)
        {
            return Inner.Remove(item);
        }

        /// <inheritdoc cref="ISet{T}.SetEquals"/>
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

        /// <summary>Copies the elements of a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object to an array.</summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the
        /// <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object. The array must have zero-based indexing.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null.
        /// </exception>
        public void CopyTo(T[] array)
        {
            Inner.CopyTo(array);
        }

        /// <summary>
        /// Copies the specified number of elements of a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object to an
        /// array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the
        /// <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy to <paramref name="array"/>.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.-or-<paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="arrayIndex"/> is greater than the length of the destination <paramref name="array"/>.-or-
        /// <paramref name="count"/> is greater than the available space from the <paramref name="arrayIndex"/> to the end of the
        /// destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            Inner.CopyTo(array, arrayIndex, count);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.HashSetExWrapper`1.Enumerator"/> object for the
        /// <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object.
        /// </returns>
        public HashSet<T>.Enumerator GetEnumerator()
        {
            return Inner.GetEnumerator();
        }

        /// <summary>
        /// Removes all elements that match the conditions defined by the specified predicate from a
        /// <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> collection.
        /// </summary>
        /// <returns>
        /// The number of elements that were removed from the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// collection.
        /// </returns>
        /// <param name="match">
        /// The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the elements to
        /// remove.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match"/> is null.
        /// </exception>
        public int RemoveWhere(Predicate<T> match)
        {
            return Inner.RemoveWhere(match);
        }

        /// <summary>
        /// Sets the capacity of a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object to the actual number of
        /// elements it contains, rounded up to a nearby, implementation-specific value.
        /// </summary>
        public void TrimExcess()
        {
            Inner.TrimExcess();
        }
    }
}