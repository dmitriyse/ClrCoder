// <copyright file="IReadOnlyListEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    public interface IReadOnlyListEx<out T> : IReadOnlyList<T>, IReadOnlyCollectionEx<T>
    {
    }
}