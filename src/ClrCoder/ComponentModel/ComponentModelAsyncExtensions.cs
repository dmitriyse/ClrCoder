// <copyright file="ComponentModelAsyncExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel
{
    using System;
    using System.Threading.Tasks;

    public static class ComponentModelAsyncExtensions
    {
        public static async Task AsyncUsing<T>(this Task<T> objTask, Func<T, Task> action)
            where T : IAsyncDisposable
        {
            T obj = await objTask;
            try
            {
                await action(obj);
            }
            finally
            {
                await obj.AsyncDispose();
            }
        }
        public static async Task<TResult> AsyncUsing<T, TResult>(this Task<T> objTask, Func<T, Task<TResult>> action)
            where T : IAsyncDisposable
        {
            T obj = await objTask;
            try
            {
                return await action(obj);
            }
            finally
            {
                await obj.AsyncDispose();
            }
        }
    }
}