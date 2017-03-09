// <copyright file="ReadOnlySetExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    //// ReSharper disable UnusedMember.Global

    /// <summary>
    /// Extensions for the <see cref="IReadOnlySet{T}"/>.
    /// </summary>
    public static class ReadOnlySetExtensions
    {
        /// <summary>
        /// Wraps the provided hashset to the wrapper collection with additional contracts implementation.
        /// (<see cref="IReadableSet{T}"/>, <see cref="IReadOnlySet{T}"/>)
        /// </summary>
        /// <typeparam name="T">The type of elements in the set.</typeparam>
        /// <param name="hashSet">The hash set to extend. Works as null in, null out.</param>
        /// <returns>The extended wrapper over the provided set.</returns>
        public static HashSetExWrapper<T> Extend<T>(this HashSet<T> hashSet)
        {
            // Implicit conversion here.
            return hashSet;
        }

        /// <summary>
        /// Wraps the provided set to the wrapper collection with additional contracts implementation.
        /// (<see cref="IReadableSet{T}"/>, <see cref="IReadOnlySet{T}"/>)
        /// </summary>
        /// <typeparam name="T">The type of elements in the set.</typeparam>
        /// <param name="set">The hash set to extend. Works as null in, null out.</param>
        /// <param name="comparer">Equality comparer that is used by the inner set.</param>
        /// <returns>The extended wrapper over the provided set.</returns>
        public static SetExWrapper<T> Extend<T>(this ISet<T> set, IEqualityComparer<T> comparer)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return set == null ? null : new SetExWrapper<T>(set, comparer);
        }

        /// <summary>
        /// Creates extended hash set from the specified enumeration and the specified comparer.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumeration.</typeparam>
        /// <param name="enumeration">The enumeration to create hash set from.</param>
        /// <param name="comparer">Comparer for the new hash set.</param>
        /// <returns>New hash set with items from the specifie enumeration.</returns>
        public static HashSetEx<T> ToHashSetEx<T>(this IEnumerable<T> enumeration, IEqualityComparer<T> comparer = null)
        {
            if (enumeration == null)
            {
                throw new ArgumentNullException(nameof(enumeration));
            }

            if (comparer == null)
            {
                return new HashSetEx<T>(enumeration);
            }

            return new HashSetEx<T>(enumeration, comparer: comparer);
        }
    }
}