// <copyright file="HashSetExWrapper.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    //// ReSharper disable ImplicitNotNullOverridesUnknownExternalMember

    /// <summary>
    /// Represents a set of values. Wraps existing <see cref="HashSet{T}"/> and provides implementation of an additional
    /// contracts (<see cref="IReadableSet{T}"/> and <see cref="IReadOnlySet{T}"/>).
    /// </summary>
    /// <typeparam name="T">The type of elements in the hash set.</typeparam>
    public class HashSetExWrapper<T> : ISet<T>, IReadableSet<T>
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

        /// <summary>Gets the number of elements that are contained in a set.</summary>
        /// <returns>The number of elements that are contained in the set.</returns>
        public int Count => Inner.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)Inner).IsReadOnly;

        /// <summary>
        /// Gets the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> object that is used to determine
        /// equality for the values in the set.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> object that is used to determine equality
        /// for the values in the set.
        /// </returns>
        public IEqualityComparer<T> Comparer => Inner.Comparer;

        /// <summary>
        /// Wrapped set.
        /// </summary>
        public HashSet<T> Inner { get; }

        /// <summary>
        /// Converts extended to classic hashset.
        /// </summary>
        /// <param name="setEx">Classic hash set.</param>
        public static implicit operator HashSet<T>(HashSetExWrapper<T> setEx)
        {
            // ReSharper disable once ConstantConditionalAccessQualifier
            return setEx?.Inner;
        }

        /// <summary>
        /// Converts classic <c>set</c> to extended.
        /// </summary>
        /// <param name="set">Extended wrapper over classic set.</param>
        public static implicit operator HashSetExWrapper<T>(HashSet<T> set)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return set == null ? null : new HashSetExWrapper<T>(set);
        }

        /// <summary>Adds the specified element to a set.</summary>
        /// <returns>
        /// true if the element is added to the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object; false if the
        /// element is already present.
        /// </returns>
        /// <param name="item">The element to add to the set.</param>
        public bool Add(T item)
        {
            return Inner.Add(item);
        }

        void ICollection<T>.Add(T item)
        {
        }

        /// <summary>Removes all elements from a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object.</summary>
        public void Clear()
        {
        }

        /// <summary>
        /// Determines whether a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object contains the specified
        /// element.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object contains the specified element;
        /// otherwise, false.
        /// </returns>
        /// <param name="item">The element to locate in the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object.</param>
        public bool Contains(T item)
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

        /// <summary>
        /// Copies the elements of a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object to an array, starting at
        /// the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the
        /// <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="arrayIndex"/> is greater than the length of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Inner.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes all elements in the specified collection from the current
        /// <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object.
        /// </summary>
        /// <param name="other">
        /// The collection of items to remove from the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        public void ExceptWith(IEnumerable<T> other)
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

        /// <summary>
        /// Modifies the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object to contain only elements that
        /// are present in that object and in the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        public void IntersectWith(IEnumerable<T> other)
        {
            Inner.IntersectWith(other);
        }

        /// <summary>
        /// Determines whether a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object is a proper subset of the
        /// specified collection.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object is a proper subset of
        /// <paramref name="other"/>; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// The collection to compare to the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return Inner.IsProperSubsetOf(other);
        }

        /// <summary>
        /// Determines whether a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object is a proper superset of the
        /// specified collection.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object is a proper superset of
        /// <paramref name="other"/>; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// The collection to compare to the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Inner.IsProperSupersetOf(other);
        }

        /// <summary>
        /// Determines whether a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object is a subset of the specified
        /// collection.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object is a subset of
        /// <paramref name="other"/>; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// The collection to compare to the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return Inner.IsSubsetOf(other);
        }

        /// <summary>
        /// Determines whether a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object is a superset of the
        /// specified collection.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object is a superset of
        /// <paramref name="other"/>; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// The collection to compare to the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return Inner.IsSupersetOf(other);
        }

        /// <summary>
        /// Determines whether the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object and a specified
        /// collection share common elements.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object and <paramref name="other"/> share at
        /// least one common element; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// The collection to compare to the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        public bool Overlaps(IEnumerable<T> other)
        {
            return Inner.Overlaps(other);
        }

        /// <summary>Removes the specified element from a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object.</summary>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false.  This method returns false if
        /// <paramref name="item"/> is not found in the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object.
        /// </returns>
        /// <param name="item">The element to remove.</param>
        public bool Remove(T item)
        {
            return Inner.Remove(item);
        }

        /// <summary>
        /// Determines whether a <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object and the specified collection
        /// contain the same elements.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object is equal to <paramref name="other"/>;
        /// otherwise, false.
        /// </returns>
        /// <param name="other">
        /// The collection to compare to the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        public bool SetEquals(IEnumerable<T> other)
        {
            return Inner.SetEquals(other);
        }

        /// <summary>
        /// Modifies the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object to contain only elements that
        /// are present either in that object or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            Inner.SymmetricExceptWith(other);
        }

        /// <summary>
        /// Modifies the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/> object to contain all elements that
        /// are present in itself, the specified collection, or both.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="T:System.Collections.Generic.HashSetExWrapper`1"/>
        /// object.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        public void UnionWith(IEnumerable<T> other)
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