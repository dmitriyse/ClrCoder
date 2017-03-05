// <copyright file="IReadOnlyCollectionEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    public interface IReadOnlyCollectionEx<out T> : IReadOnlyCollection<T>, IEnumerableEx<T>
    {
    }
}