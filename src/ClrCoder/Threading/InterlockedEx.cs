// <copyright file="InterlockedEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading;

    using JetBrains.Annotations;

    using Validation;

    /// <summary>
    /// Interlocked extended methods.
    /// </summary>
    [PublicAPI]
    public static class InterlockedEx
    {
        /// <summary>
        /// Lock-less, optimistically updates variable concurrently with possilbe retries.
        /// </summary>
        /// <typeparam name="T">The type of the variable to update.</typeparam>
        /// <typeparam name="TOut">The output type.</typeparam>
        /// <param name="value">The variable to update passed by reference.</param>
        /// <param name="updateFunc">The update function (possibly called multiple times).</param>
        /// <returns>The output value.</returns>
        [CanBeNull]
        public static TOut InterlockedUpdate<T, TOut>(ref T value, Func<T, (T, TOut)> updateFunc)
            where T : class
        {
            VxArgs.NotNull(updateFunc, nameof(updateFunc));

            T readedValue = Volatile.Read(ref value);
            T originalValue;
            TOut outValue;
            do
            {
                originalValue = readedValue;
                T updatedValue;
                (updatedValue, outValue) = updateFunc(originalValue);
                readedValue = Interlocked.CompareExchange(ref value, updatedValue, originalValue);
            }
            while (!ReferenceEquals(readedValue, originalValue));

            return outValue;
        }

        /// <summary>
        /// Lock-less, optimistically updates variable concurrently with possilbe retries.
        /// </summary>
        /// <typeparam name="T">The type of the variable to update.</typeparam>
        /// <typeparam name="TArg">The type of the parameter of the update function.</typeparam>
        /// <typeparam name="TOut">The output type.</typeparam>
        /// <param name="value">The variable to update passed by reference.</param>
        /// <param name="updateFunc">The update function (possibly called multiple times).</param>
        /// <param name="arg">The argument value for the update function.</param>
        /// <returns>The output value.</returns>
        [CanBeNull]
        public static TOut InterlockedUpdate<T, TArg, TOut>(ref T value, Func<T, TArg, (T, TOut)> updateFunc, [CanBeNull]TArg arg)
            where T : class
        {
            VxArgs.NotNull(updateFunc, nameof(updateFunc));

            T readedValue = Volatile.Read(ref value);
            T originalValue;
            TOut outValue;
            do
            {
                originalValue = readedValue;
                T updatedValue;
                (updatedValue, outValue) = updateFunc(originalValue, arg);
                readedValue = Interlocked.CompareExchange(ref value, updatedValue, originalValue);
            }
            while (!ReferenceEquals(readedValue, originalValue));

            return outValue;
        }

        /// <summary>
        /// Lock-less, optimistically updates variable concurrently with possilbe retries.
        /// </summary>
        /// <typeparam name="T">The type of the variable to update.</typeparam>
        /// <param name="value">The variable to update passed by reference.</param>
        /// <param name="updateFunc">The update function (possibly called multiple times).</param>
        public static void InterlockedUpdate<T>(ref T value, Func<T, T> updateFunc)
            where T : class
        {
            VxArgs.NotNull(updateFunc, nameof(updateFunc));

            T readedValue = Volatile.Read(ref value);
            T originalValue;
            do
            {
                originalValue = readedValue;
                T updatedValue = updateFunc(originalValue);
                readedValue = Interlocked.CompareExchange(ref value, updatedValue, originalValue);
            }
            while (!ReferenceEquals(readedValue, originalValue));
        }

        /// <summary>
        /// Lock-less, optimistically updates variable concurrently with possilbe retries.
        /// </summary>
        /// <typeparam name="T">The type of the variable to update.</typeparam>
        /// <typeparam name="TArg">The type of the parameter of the update function.</typeparam>
        /// <param name="value">The variable to update passed by reference.</param>
        /// <param name="updateFunc">The update function (possibly called multiple times).</param>
        /// <param name="arg">The argument value for the update function.</param>
        public static void InterlockedUpdate<T, TArg>(ref T value, Func<T, TArg, T> updateFunc, [CanBeNull]TArg arg)
            where T : class
        {
            VxArgs.NotNull(updateFunc, nameof(updateFunc));

            T readedValue = Volatile.Read(ref value);
            T originalValue;
            do
            {
                originalValue = readedValue;
                T updatedValue = updateFunc(originalValue, arg);
                readedValue = Interlocked.CompareExchange(ref value, updatedValue, originalValue);
            }
            while (!ReferenceEquals(readedValue, originalValue));
        }
    }
}