// <copyright file="IImmutableCollection.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    /// <summary>
    /// Immutable collection. By a convention items of collection also should be immutable.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements.This type parameter is covariant. That is, you can use either the type you
    /// specified or any type that is more derived. For more information about covariance and contravariance, see Covariance
    /// and Contravariance in Generics.
    /// </typeparam>
    public interface IImmutableCollection<out T> : IReadOnlyCollectionEx<T>, IImmutableEnumerable<T>
    {
    }
}