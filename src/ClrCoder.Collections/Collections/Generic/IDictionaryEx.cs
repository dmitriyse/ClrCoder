// <copyright file="IDictionaryEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    //// ReSharper disable once PossibleInterfaceMemberAmbiguity

    /// <summary>
    /// Represents a generic collection of key/value pairs. <br/>
    /// Polyfill of proposed new features. Assume that <see cref="IDictionaryEx{TKey, TValue}"/> will merge
    /// <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <remarks>
    /// When proposal will be adopted into BCL you need to replace all textual occurrences of IDictionaryEx to
    /// IDictionary.
    /// Polyfill proposal https://github.com/dotnet/corefx/issues/16626 . Collections contracts should inherits from readonly
    /// contracts.
    /// Also this feature is depends on C#/CLR proposal https://github.com/dotnet/csharplang/issues/52
    /// </remarks>
    /// <typeparam name="TKey">The type of keys in the read-only dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
    [PublicAPI]
    public interface IDictionaryEx<TKey, TValue> : IDictionary<TKey, TValue>,
                                                   ICollectionEx<KeyValuePair<TKey, TValue>>,
                                                   IReadOnlyDictionaryEx<TKey, TValue>
    {
    }
}