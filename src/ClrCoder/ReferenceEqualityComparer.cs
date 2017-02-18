// <copyright file="ReferenceEqualityComparer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// Equality comparer that checks only references.
    /// </summary>
    /// <typeparam name="T">Type of <c>object</c>.</typeparam>
    [PublicAPI]
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private ReferenceEqualityComparer()
        {
        }

        /// <summary>
        /// Instance of the comparer.
        /// </summary>
        public static ReferenceEqualityComparer<T> Default => new ReferenceEqualityComparer<T>();

        /// <inheritdoc/>
        public bool Equals([CanBeNull] T x, [CanBeNull] T y)
        {
            return ReferenceEquals(x, y);
        }

        /// <inheritdoc/>
        public int GetHashCode([CanBeNull] T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}