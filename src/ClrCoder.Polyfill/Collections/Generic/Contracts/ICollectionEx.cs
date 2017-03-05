// <copyright file="ICollectionEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    //// ReSharper disable once PossibleInterfaceMemberAmbiguity

    /// <summary>
    /// Defines methods to manipulate generic collections. <br/>
    /// Polyfill of comming new features.
    /// </summary>
    /// <remarks>
    /// Add link to 52
    /// Add link to 16660
    /// </remarks>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public interface ICollectionEx<T> : ICollection<T>, IReadOnlyCollectionEx<T>
    {
    }
}