// <copyright file="NullBiggestNullableComparer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logic
{
    using System.Collections.Generic;

    /// <summary>
    /// <see cref="NullBiggestComparer{T}"/> implementation for nullable types.
    /// </summary>
    /// <typeparam name="TStruct">Target struct type.</typeparam>
    internal class NullBiggestNullableComparer<TStruct> : IComparer<TStruct?>
        where TStruct : struct
    {
        /// <summary>
        /// <c>Comparer</c> instance.
        /// </summary>
        private static readonly IComparer<TStruct?> DefaultNullableComparer = Comparer<TStruct?>.Default;

        /// <inheritdoc/>
        public int Compare(TStruct? x, TStruct? y)
        {
            if (x == null && y != null)
            {
                return 1;
            }

            if (x != null && y == null)
            {
                return -1;
            }

            return DefaultNullableComparer.Compare(x, y);
        }
    }
}