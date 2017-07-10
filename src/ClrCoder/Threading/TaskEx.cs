// <copyright file="TaskEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// TPL Task utility methods.
    /// </summary>
    [PublicAPI]
    public static class TaskEx
    {
        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task AsyncInvoke(this Func<Task> asyncMethod)
        {
            return Task.Factory.StartNew(() => asyncMethod().GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TArg1">The type of the first argument.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <param name="arg1">The value of the first argument.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task AsyncInvoke<TArg1>(this Func<TArg1, Task> asyncMethod, TArg1 arg1)
        {
            return Task.Factory.StartNew(() => asyncMethod(arg1).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TArg1">The type of the first argument.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <param name="arg1">The value of the first argument.</param>
        /// <param name="arg2">The value of the second argument.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task AsyncInvoke<TArg1, TArg2>(this Func<TArg1, TArg2, Task> asyncMethod, TArg1 arg1, TArg2 arg2)
        {
            return Task.Factory.StartNew(() => asyncMethod(arg1, arg2).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TArg1">The type of the first argument.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument.</typeparam>
        /// <typeparam name="TArg3">The type of the third argument.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <param name="arg1">The value of the first argument.</param>
        /// <param name="arg2">The value of the second argument.</param>
        /// <param name="arg3">The value of the third argument.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task AsyncInvoke<TArg1, TArg2, TArg3>(
            this Func<TArg1, TArg2, TArg3, Task> asyncMethod,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3)
        {
            return Task.Factory.StartNew(() => asyncMethod(arg1, arg2, arg3).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TArg1">The type of the first argument.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument.</typeparam>
        /// <typeparam name="TArg3">The type of the third argument.</typeparam>
        /// <typeparam name="TArg4">The type of the fourth argument.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <param name="arg1">The value of the first argument.</param>
        /// <param name="arg2">The value of the second argument.</param>
        /// <param name="arg3">The value of the third argument.</param>
        /// <param name="arg4">The value of the fourth argument.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task AsyncInvoke<TArg1, TArg2, TArg3, TArg4>(
            this Func<TArg1, TArg2, TArg3, TArg4, Task> asyncMethod,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            TArg4 arg4)
        {
            return Task.Factory.StartNew(() => asyncMethod(arg1, arg2, arg3, arg4).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TArg1">The type of the first argument.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument.</typeparam>
        /// <typeparam name="TArg3">The type of the third argument.</typeparam>
        /// <typeparam name="TArg4">The type of the fourth argument.</typeparam>
        /// <typeparam name="TArg5">The type of the fifth argument.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <param name="arg1">The value of the first argument.</param>
        /// <param name="arg2">The value of the second argument.</param>
        /// <param name="arg3">The value of the third argument.</param>
        /// <param name="arg4">The value of the fourth argument.</param>
        /// <param name="arg5">The value of the fifth argument.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task AsyncInvoke<TArg1, TArg2, TArg3, TArg4, TArg5>(
            this Func<TArg1, TArg2, TArg3, TArg4, TArg5, Task> asyncMethod,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            TArg4 arg4,
            TArg5 arg5)
        {
            return Task.Factory.StartNew(() => asyncMethod(arg1, arg2, arg3, arg4, arg5).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TResult">The result type of the async method.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task<TResult> AsyncInvoke<TResult>(this Func<Task<TResult>> asyncMethod)
        {
            return Task.Factory.StartNew(() => asyncMethod().GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TResult">The result type of the async method.</typeparam>
        /// <typeparam name="TArg1">The type of the first argument.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <param name="arg1">The value of the first argument.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task<TResult> AsyncInvoke<TResult, TArg1>(this Func<TArg1, Task<TResult>> asyncMethod, TArg1 arg1)
        {
            return Task.Factory.StartNew(() => asyncMethod(arg1).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TResult">The result type of the async method.</typeparam>
        /// <typeparam name="TArg1">The type of the first argument.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <param name="arg1">The value of the first argument.</param>
        /// <param name="arg2">The value of the second argument.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task<TResult> AsyncInvoke<TResult, TArg1, TArg2>(
            this Func<TArg1, TArg2, Task<TResult>> asyncMethod,
            TArg1 arg1,
            TArg2 arg2)
        {
            return Task.Factory.StartNew(() => asyncMethod(arg1, arg2).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TResult">The result type of the async method.</typeparam>
        /// <typeparam name="TArg1">The type of the first argument.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument.</typeparam>
        /// <typeparam name="TArg3">The type of the third argument.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <param name="arg1">The value of the first argument.</param>
        /// <param name="arg2">The value of the second argument.</param>
        /// <param name="arg3">The value of the third argument.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task<TResult> AsyncInvoke<TResult, TArg1, TArg2, TArg3>(
            this Func<TArg1, TArg2, TArg3, Task<TResult>> asyncMethod,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3)
        {
            return Task.Factory.StartNew(() => asyncMethod(arg1, arg2, arg3).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TResult">The result type of the async method.</typeparam>
        /// <typeparam name="TArg1">The type of the first argument.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument.</typeparam>
        /// <typeparam name="TArg3">The type of the third argument.</typeparam>
        /// <typeparam name="TArg4">The type of the fourth argument.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <param name="arg1">The value of the first argument.</param>
        /// <param name="arg2">The value of the second argument.</param>
        /// <param name="arg3">The value of the third argument.</param>
        /// <param name="arg4">The value of the fourth argument.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task<TResult> AsyncInvoke<TResult, TArg1, TArg2, TArg3, TArg4>(
            this Func<TArg1, TArg2, TArg3, TArg4, Task<TResult>> asyncMethod,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            TArg4 arg4)
        {
            return Task.Factory.StartNew(() => asyncMethod(arg1, arg2, arg3, arg4).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Invokes async methods in a thread pool, return task as fast as possible.
        /// </summary>
        /// <typeparam name="TResult">The result type of the async method.</typeparam>
        /// <typeparam name="TArg1">The type of the first argument.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument.</typeparam>
        /// <typeparam name="TArg3">The type of the third argument.</typeparam>
        /// <typeparam name="TArg4">The type of the fourth argument.</typeparam>
        /// <typeparam name="TArg5">The type of the fifth argument.</typeparam>
        /// <param name="asyncMethod">The async method to execute.</param>
        /// <param name="arg1">The value of the first argument.</param>
        /// <param name="arg2">The value of the second argument.</param>
        /// <param name="arg3">The value of the third argument.</param>
        /// <param name="arg4">The value of the fourth argument.</param>
        /// <param name="arg5">The value of the fifth argument.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task<TResult> AsyncInvoke<TResult, TArg1, TArg2, TArg3, TArg4, TArg5>(
            this Func<TArg1, TArg2, TArg3, TArg4, TArg5, Task<TResult>> asyncMethod,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            TArg4 arg4,
            TArg5 arg5)
        {
            return Task.Factory.StartNew(() => asyncMethod(arg1, arg2, arg3, arg4, arg5).GetAwaiter().GetResult());
        }
    }
}