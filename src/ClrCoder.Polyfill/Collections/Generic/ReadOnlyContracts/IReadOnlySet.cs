// <copyright file="IReadOnlySet.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    /// <summary>
    /// Readable set abstracton. Also ensures that elements are unique.
    /// </summary>
    /// <remarks>
    /// WHY IT DOES NOT contains method bool Contains(T) ???. Don't warry, just use
    /// <see cref="CollectionsExtensions.Contains()"/> method it will try by cast or by reflection access to
    /// implementation of Contains() method.
    /// Proposal for this abstraction is discussed here https://github.com/dotnet/corefx/issues/1973.
    /// There is two opened question:
    /// 1) Comparer property
    /// 2) IsSubsetOf, ISupersetOf, IsProperSupersetOf, IsProperSubsetOf, Overlaps, SetEquals methods.
    /// Current implementation is follows to <c>this</c> comment
    /// https://github.com/dotnet/corefx/issues/1973#issuecomment-283912409
    /// </remarks>
    /// <typeparam name="T">Item type.</typeparam>
    public interface IReadOnlySet<out T> : IReadOnlyCollectionEx<T>
    {
    }
}