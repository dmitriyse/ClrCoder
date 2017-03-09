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