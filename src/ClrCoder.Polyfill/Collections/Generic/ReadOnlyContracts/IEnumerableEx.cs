// <copyright file="IEnumerableEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    public interface IEnumerableEx<out T> : IEnumerable<T>
    {
        bool IsImmutable { get; }
    }
}