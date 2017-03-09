// <copyright file="IReadOnlyDictionaryEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a read-only generic collection of key/value pairs. <br/>
    /// Polyfill of proposed new features. Assume that <see cref="IReadOnlyDictionaryEx{TKey, TValue}"/> will merge
    /// <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <remarks>
    /// When proposal will be adopted into BCL you need to replace all textual occurrences of IReadOnlyDictionaryEx to
    /// IReadOnlyDictionary.
    /// Polyfill proposal https://github.com/dotnet/corefx/issues/16818 .
    /// Also this feature is depends on C#/CLR proposal https://github.com/dotnet/csharplang/issues/52
    /// </remarks>
    /// <typeparam name="TKey">The type of keys in the read-only dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
    public interface IReadOnlyDictionaryEx<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        /// <summary>
        /// Equality comparer that is used for dictionary keys.
        /// </summary>
        IEqualityComparer<TKey> Comparer { get; }
    }
}