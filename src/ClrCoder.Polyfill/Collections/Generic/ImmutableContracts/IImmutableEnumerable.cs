// <copyright file="IImmutableEnumerable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    /// <summary>
    /// Immutable variant of a collection that is materialized, but provides only IEnumerable{T} like stack, queue.
    /// </summary>
    /// <remarks>
    /// This contract implements proposal https://github.com/dotnet/corefx/issues/16661.
    /// </remarks>
    /// <typeparam name="T">Item type.</typeparam>
    public interface IImmutableEnumerable<out T> : IEnumerableEx<T>
    {
    }
}