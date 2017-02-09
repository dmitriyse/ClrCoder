// <copyright file="SerializationExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Extension methods related to serialization.
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Dumps <c>object</c>.
        /// </summary>
        /// <typeparam name="TResult">Type of dump resut.</typeparam>
        /// <param name="dumper">Dumper to use.</param>
        /// <param name="target">Object to dump.</param>
        /// <returns>Object dump.</returns>
        public static TResult Dump<TResult>(this IObjectDumper dumper, object target)
        {
            if (dumper == null)
            {
                throw new ArgumentNullException(nameof(dumper));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return (TResult)dumper.Filter(target.GetType(), null, target);
        }
    }
}