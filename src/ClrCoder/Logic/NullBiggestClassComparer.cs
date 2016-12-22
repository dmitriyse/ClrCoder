// <copyright file="NullBiggestClassComparer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Logic
{
    using System.Collections.Generic;

    /// <summary>
    /// <see cref="NullBiggestComparer{T}"/> implementation for classes.
    /// </summary>
    /// <typeparam name="TClass">Target class type.</typeparam>
    internal class NullBiggestClassComparer<TClass> : IComparer<TClass>
        where TClass : class
    {
        /// <summary>
        /// <c>Comparer</c> instance.
        /// </summary>
        public static readonly IComparer<TClass> DefaultComparer = Comparer<TClass>.Default;

        /// <inheritdoc/>
        public int Compare(TClass x, TClass y)
        {
            if (x == null && y != null)
            {
                return 1;
            }

            if (x != null && y == null)
            {
                return -1;
            }

            return DefaultComparer.Compare(x, y);
        }
    }
}