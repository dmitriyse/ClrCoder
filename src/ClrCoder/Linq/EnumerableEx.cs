// <copyright file="EnumerableEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Linq
{
    using System.Collections.Generic;

    /// <summary>
    /// The linq like extension methods.
    /// </summary>
    public static class EnumerableEx
    {
        /// <summary>
        /// Simple join of the input items with the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the items to join.</typeparam>
        /// <typeparam name="TValue">The type of join result.</typeparam>
        /// <param name="items">The source items.</param>
        /// <param name="index">The indexed collection to join with.</param>
        /// <returns>The join result in a form of key value pairs.</returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> Join<TKey, TValue>(
            this IEnumerable<TKey> items,
            IReadOnlyDictionary<TKey, TValue> index)
        {
            foreach (TKey key in items)
            {
                yield return new KeyValuePair<TKey, TValue>(key, index[key]);
            }
        }
    }
}