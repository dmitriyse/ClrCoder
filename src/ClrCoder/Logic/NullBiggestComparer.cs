// <copyright file="NullBiggestComparer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// <c>Comparer</c>, same as <see cref="Comparer{T}.Default"/> but the null is biggest value.
    /// </summary>
    /// <typeparam name="T">Type of comparing values.</typeparam>
    [PublicAPI]
    public class NullBiggestComparer<T>
    {
        /// <summary>
        /// Initializes class.
        /// </summary>
        static NullBiggestComparer()
        {
            if (typeof(T).GetTypeInfo().IsClass)
            {
                Default =
                    (IComparer<T>)
                    Activator.CreateInstance(typeof(NullBiggestClassComparer<>).MakeGenericType(typeof(T)));
            }
            else if (typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Default =
                    (IComparer<T>)
                    Activator.CreateInstance(
                        typeof(NullBiggestNullableComparer<>).MakeGenericType(typeof(T).GenericTypeArguments[0]));
            }
            else
            {
                Default = Comparer<T>.Default;
            }
        }

        /// <summary>
        /// The <c>comparer</c> instance.
        /// </summary>
        public static IComparer<T> Default { get; private set; }
    }
}