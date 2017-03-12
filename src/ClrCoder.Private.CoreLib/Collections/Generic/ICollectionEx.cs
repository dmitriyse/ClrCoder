// <copyright file="ICollectionEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    //// ReSharper disable once PossibleInterfaceMemberAmbiguity

    /// <summary>
    /// Defines methods to manipulate generic collections. <br/>
    /// Polyfill of proposed new features. Assume that <see cref="ICollection{T}"/> will merge <see cref="ICollectionEx{T}"/>.
    /// </summary>
    /// <remarks>
    /// When proposal will be adopted into BCL you need to replace all textual occurrences of ICollectionEx to
    /// ICollection.
    /// Polyfill proposal https://github.com/dotnet/corefx/issues/16626 . Collections contracts should inherits from readonly
    /// contracts.
    /// Also this feature is depends on C#/CLR proposal https://github.com/dotnet/csharplang/issues/52
    /// </remarks>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public interface ICollectionEx<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>The number of elements in the collection.</returns>
        /// <remarks>
        /// Both base contracts have this property, so we need to define another one to avoid "ambiguous error".
        /// </remarks>
        new int Count { get; }
    }
}