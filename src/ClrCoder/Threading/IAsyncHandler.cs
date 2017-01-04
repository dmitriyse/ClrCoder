// <copyright file="IAsyncHandler.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Asynchronous handler. Allow to process work items asynchronously.
    /// </summary>
    [PublicAPI]
    public interface IAsyncHandler
    {
        /// <summary>
        /// Executes <c>action</c> asynchronously.
        /// </summary>
        /// <param name="action">Action to be executed.</param>
        void RunAsync([NotNull] Action action);

        /// <summary>
        /// Executes <c>action</c> with one argument asynchronously.
        /// </summary>
        /// <typeparam name="T">First argument type.</typeparam>
        /// <param name="action">Action to be executed.</param>
        /// <param name="arg">Action argument 1.</param>
        void RunAsync<T>([NotNull] Action<T> action, T arg);

        /// <summary>
        /// Executes <c>action</c> with one argument asynchronously.
        /// </summary>
        /// <typeparam name="T1">First argument type.</typeparam>
        /// <typeparam name="T2">Second argument type.</typeparam>
        /// <param name="action">Action to be executed.</param>
        /// <param name="arg1">Action argument 1.</param>
        /// <param name="arg2">Action argument 2.</param>
        void RunAsync<T1, T2>([NotNull] Action<T1, T2> action, T1 arg1, T2 arg2);

        /// <summary>
        /// Executes <c>action</c> with one argument asynchronously.
        /// </summary>
        /// <typeparam name="T1">First argument type.</typeparam>
        /// <typeparam name="T2">Second argument type.</typeparam>
        /// <typeparam name="T3">Third argument type.</typeparam>
        /// <param name="action">Action to be executed.</param>
        /// <param name="arg1">Action argument 1.</param>
        /// <param name="arg2">Action argument 2.</param>
        /// <param name="arg3">Action argument 3.</param>
        void RunAsync<T1, T2, T3>([NotNull] Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3);

        /// <summary>
        /// Executes <c>action</c> with one argument asynchronously.
        /// </summary>
        /// <typeparam name="T1">First argument type.</typeparam>
        /// <typeparam name="T2">Second argument type.</typeparam>
        /// <typeparam name="T3">Third argument type.</typeparam>
        /// <typeparam name="T4">4th argument type.</typeparam>
        /// <param name="action">Action to be executed.</param>
        /// <param name="arg1">Action argument 1.</param>
        /// <param name="arg2">Action argument 2.</param>
        /// <param name="arg3">Action argument 3.</param>
        /// <param name="arg4">Action argument 4.</param>
        void RunAsync<T1, T2, T3, T4>([NotNull] Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        /// <summary>
        /// Executes <c>action</c> with one argument asynchronously.
        /// </summary>
        /// <typeparam name="T1">First argument type.</typeparam>
        /// <typeparam name="T2">Second argument type.</typeparam>
        /// <typeparam name="T3">Third argument type.</typeparam>
        /// <typeparam name="T4">4th argument type.</typeparam>
        /// <typeparam name="T5">5th argument type.</typeparam>
        /// <param name="action">Action to be executed.</param>
        /// <param name="arg1">Action argument 1.</param>
        /// <param name="arg2">Action argument 2.</param>
        /// <param name="arg3">Action argument 3.</param>
        /// <param name="arg4">Action argument 4.</param>
        /// <param name="arg5">Action argument 5.</param>
        void RunAsync<T1, T2, T3, T4, T5>(
            [NotNull] Action<T1, T2, T3, T4, T5> action,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5);
    }
}