// <copyright file="IReadOnlyDictionaryEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    /// <summary>
    /// Polyfilled readonly dictionary. Do not use it directly in variables/parameters/fields/properties. <br/>
    /// </summary>
    /// <remarks>TODO: fill me.</remarks>
    /// <typeparam name="TKey">The type of keys in the read-only dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
    public interface IReadOnlyDictionaryEx<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollectionEx<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Equality comparer that is used for dictionary keys.
        /// </summary>
        IEqualityComparer<TKey> Comparer { get; }
    }
}