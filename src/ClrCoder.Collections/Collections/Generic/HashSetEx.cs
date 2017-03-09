// <copyright file="HashSetEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    /// <summary>
    /// Extended <see cref="HashSet{T}"/> implementation, that implements <see cref="ISetEx{T}"/> <see cref="IReadableSet{T}"/>
    /// , <see cref="IReadOnlySet{T}"/> contracts.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    public class HashSetEx<T> : HashSet<T>, ISetEx<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSetEx`1"/> class that is empty and
        /// uses the default equality comparer for the set type.
        /// </summary>
        public HashSetEx()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSetEx`1"/> class that uses the
        /// default equality comparer for the set type, contains elements copied from the specified collection, and has sufficient
        /// capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="collection"/> is null.
        /// </exception>
        public HashSetEx(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSetEx`1"/> class that uses the
        /// specified equality comparer for the set type, contains elements copied from the specified collection, and has
        /// sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <param name="comparer">
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when
        /// comparing values in the set, or null to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1"/>
        /// implementation for the set type.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="collection"/> is null.
        /// </exception>
        public HashSetEx(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : base(collection, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSetEx`1"/> class that is empty and
        /// uses the specified equality comparer for the set type.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when
        /// comparing values in the set, or null to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1"/>
        /// implementation for the set type.
        /// </param>
        public HashSetEx(IEqualityComparer<T> comparer)
            : base(comparer)
        {
        }

        /// <inheritdoc/>
        bool IReadOnlySet<T>.Contains<TItem>(TItem item)
        {
            // TODO: Some optimization possible here. For valued types.
            if (item is T)
            {
                return Contains((T)(object)item);
            }

            return false;
        }
    }
}