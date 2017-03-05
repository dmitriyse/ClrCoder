// <copyright file="EnumerableImmutableEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Linq
{
    using Collections.Generic;

    /// <summary>
    /// Extensions to BCL linq that helps to work with new immutable api. See
    /// </summary>
    public static class EnumerableImmutableEx
    {
        public static IImmutableSetSlim<T> ToImmutable<T>(
            this IReadOnlySet<T> set,
            bool forceWrap = false)
        {
            throw new NotImplementedException();
        }

        public static IImmutableCollection<T> ToImmutable<T>(
            this IReadOnlyCollection<T> set,
            bool forceWrap = false)
        {
            throw new NotImplementedException();
        }
    }
}