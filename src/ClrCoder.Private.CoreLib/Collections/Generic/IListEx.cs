// <copyright file="IListEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    //// ReSharper disable once PossibleInterfaceMemberAmbiguity

    /// <summary>
    /// Represents a collection of objects that can be individually accessed by index.<br/>
    /// Polyfill of proposed new features. Assume that <see cref="IList{T}"/> will merge <see cref="IListEx{T}"/>.
    /// </summary>
    /// <remarks>
    /// When proposal will be adopted into BCL you need to replace all textual occurrences of IListEx to
    /// IList.
    /// Polyfill proposal https://github.com/dotnet/corefx/issues/16626 . Collections contracts should inherits from readonly
    /// contracts.
    /// Also this feature is depends on C#/CLR proposal https://github.com/dotnet/csharplang/issues/52
    /// </remarks>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [PublicAPI]
    public interface IListEx<T> : IList<T>, ICollectionEx<T>, IReadOnlyList<T>
    {
    }
}