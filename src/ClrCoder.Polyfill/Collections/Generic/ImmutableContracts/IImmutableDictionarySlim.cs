// <copyright file="IImmutableDictionarySlim.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    /// <summary>
    /// Immutable dictionary. By a convention items of collection also should be immutable.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the immutable dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the immutable dictionary.</typeparam>
    public interface IImmutableDictionarySlim<TKey, TValue> : IReadOnlyDictionaryEx<TKey, TValue>,
                                                              IImmutableCollection<KeyValuePair<TKey, TValue>>
    {
        /// <summary>Gets an enumerable collection that contains the keys in the read-only dictionary. </summary>
        /// <returns>An enumerable collection that contains the keys in the read-only dictionary.</returns>
        /// <exception cref="InvalidOperationException">Collection is not in the immutable state.</exception>
        new IImmutableSetSlim<TKey> Keys { get; }

        /// <summary>Gets an enumerable collection that contains the values in the read-only dictionary.</summary>
        /// <returns>An enumerable collection that contains the values in the read-only dictionary.</returns>
        /// <exception cref="InvalidOperationException">Collection is not in the immutable state.</exception>
        new IImmutableCollection<TValue> Values { get; }

    }
}