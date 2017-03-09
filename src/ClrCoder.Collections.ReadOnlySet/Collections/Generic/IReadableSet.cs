// <copyright file="IReadableSet.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    /// <summary>
    /// Provides the base interface for the abstraction of sets. <br/>
    /// This is full-featured readonly interface but without contravariance. See contravariant version
    /// <see cref="IReadOnlySet{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    public interface IReadableSet<T> : IReadOnlySet<T>
    {
        /// <summary>
        /// Gets the <see cref="Generic.IEqualityComparer{T}"/> object that is used to determine equality for the values
        /// in the set.
        /// </summary>
        /// <returns>
        /// The <see cref="Generic.IEqualityComparer{T}"/> object that is used to determine equality for the values in the
        /// set.
        /// </returns>
        IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// Determines whether a <see cref="T:System.Collections.Generic.HashSet`1"/> object contains the specified
        /// element.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.HashSet`1"/> object contains the specified element;
        /// otherwise, false.
        /// </returns>
        /// <param name="item">The element to locate in the <see cref="T:System.Collections.Generic.HashSet`1"/> object.</param>
        bool Contains(T item);

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <returns><see langword="true"/> if the current set is a proper subset of <paramref name="other"/>; otherwise, false.</returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        bool IsProperSubsetOf(IEnumerable<T> other);

        /// <summary>Determines whether the current set is a proper (strict) superset of a specified collection.</summary>
        /// <returns>true if the current set is a proper superset of <paramref name="other"/>; otherwise, false.</returns>
        /// <param name="other">The collection to compare to the current set. </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        bool IsProperSupersetOf(IEnumerable<T> other);

        /// <summary>Determines whether a set is a subset of a specified collection.</summary>
        /// <returns>true if the current set is a subset of <paramref name="other"/>; otherwise, false.</returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        bool IsSubsetOf(IEnumerable<T> other);

        /// <summary>Determines whether the current set is a superset of a specified collection.</summary>
        /// <returns>true if the current set is a superset of <paramref name="other"/>; otherwise, false.</returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        bool IsSupersetOf(IEnumerable<T> other);

        /// <summary>Determines whether the current set overlaps with the specified collection.</summary>
        /// <returns>true if the current set and <paramref name="other"/> share at least one common element; otherwise, false.</returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        bool Overlaps(IEnumerable<T> other);

        /// <summary>Determines whether the current set and the specified collection contain the same elements.</summary>
        /// <returns>true if the current set is equal to <paramref name="other"/>; otherwise, false.</returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="other"/> is null.
        /// </exception>
        bool SetEquals(IEnumerable<T> other);
    }
}