// <copyright file="ObjectModelExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Object models related extensions.
    /// </summary>
    [PublicAPI]
    public static class ObjectModelExtensions
    {
        /// <summary>
        /// Builds interceptors chain from sequence of interceptor builders.
        /// </summary>
        /// <typeparam name="TDelegate">Type of method that goes through interceptors list.</typeparam>
        /// <param name="interceptors">
        /// Interceptor builder takes some <c>delegate</c> and returns intercepted
        /// <c>delegate</c>.
        /// </param>
        /// <param name="interceptTarget">Original non intercepted <c>delegate</c>.</param>
        /// <returns>Intercepted <c>delegate</c>.</returns>
        public static TDelegate BuildInterceptorsChain<TDelegate>(
            this IEnumerable<Func<TDelegate, TDelegate>> interceptors,
            TDelegate interceptTarget)
            where TDelegate : class
        {
            if (interceptors == null)
            {
                throw new ArgumentNullException(nameof(interceptors));
            }

            if (interceptTarget == null)
            {
                throw new ArgumentNullException(nameof(interceptTarget));
            }

            return interceptors.Reverse().Aggregate(interceptTarget, (current, interceptor) => interceptor(current));
        }
    }
}