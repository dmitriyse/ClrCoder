// <copyright file="ISetEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    //// ReSharper disable once PossibleInterfaceMemberAmbiguity

    /// <summary>Provides the base interface for the abstraction of sets.</summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    [PublicAPI]
    public interface ISetEx<T> : ISet<T>, ICollectionEx<T>, IReadableSet<T>
    {
    }
}