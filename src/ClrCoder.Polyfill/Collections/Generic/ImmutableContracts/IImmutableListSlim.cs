// <copyright file="IImmutableListSlim.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    /// <summary>
    /// Immutable list. By a convention items of collection also should be immutable.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    public interface IImmutableListSlim<out T> : IReadOnlyListEx<T>, IImmutableCollection<T>
    {
        /// <summary>Gets the element at the specified index in the read-only list.</summary>
        /// <returns>The element at the specified index in the read-only list.</returns>
        /// <param name="index">The zero-based index of the element to get. </param>
        /// <exception cref="InvalidOperationException">Collection is not in the immutable state.</exception>
        new T this[int index] { get; }
    }
}