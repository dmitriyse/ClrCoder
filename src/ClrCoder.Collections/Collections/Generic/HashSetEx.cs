// <copyright file="HashSetEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    /// <inheritdoc cref="HashSet{T}"/>
    public class HashSetEx<T> : HashSet<T>, ISetEx<T>
    {
        /// <inheritdoc/>
        public HashSetEx()
        {
        }

        /// <inheritdoc/>
        public HashSetEx(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <inheritdoc/>
        public HashSetEx(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : base(collection, comparer)
        {
        }

        /// <inheritdoc/>
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