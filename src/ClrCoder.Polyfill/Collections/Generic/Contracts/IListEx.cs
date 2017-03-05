// <copyright file="IListEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    //// ReSharper disable once PossibleInterfaceMemberAmbiguity

    /// <summary>
    /// Represents a collection of objects that can be individually accessed by index.<br/>
    /// Polyfill of comming new features.
    /// </summary>
    /// <remarks>
    /// Implementation of good inheritance proposal https://github.com/dotnet/corefx/issues/16626 .
    /// This is also requires https://github.com/dotnet/csharplang/issues/52 .
    /// </remarks>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public interface IListEx<T> : IList<T>, ICollectionEx<T>, IReadOnlyListEx<T>
    {
    }
}