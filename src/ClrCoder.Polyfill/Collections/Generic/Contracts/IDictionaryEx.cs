// <copyright file="IDictionaryEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a generic collection of key/value pairs.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IDictionaryEx<TKey, TValue> : IDictionary<TKey, TValue>,
                                                   ICollectionEx<KeyValuePair<TKey, TValue>>,
                                                   IReadOnlyDictionaryEx<TKey, TValue>
    {
    }
}