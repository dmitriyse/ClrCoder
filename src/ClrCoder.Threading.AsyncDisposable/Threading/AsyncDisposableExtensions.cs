// <copyright file="AsyncDisposableExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Threading
{
    using System;

    using Tasks;

    /// <summary>
    /// <see cref="IAsyncDisposable"/> related extension methods.
    /// </summary>
    public static class AsyncDisposableExtensions
    {
        /// <summary>
        /// Syntaxis sugar for <see cref="IAsyncDisposable"/> interface. Starts dispose and returns dispose task.
        /// </summary>
        /// <param name="disposable">Object to dispose.</param>
        /// <returns>Task that completes after dispose finishes.</returns>
        public static Task AsyncDispose(this IAsyncDisposable disposable)
        {
            if (disposable == null)
            {
                throw new ArgumentNullException(nameof(disposable));
            }

            disposable.StartDispose();
            return disposable.DisposeTask;
        }

        /// <summary>
        /// Using-like operation for <see cref="IAsyncDisposable"/> <c>object</c>.
        /// </summary>
        /// <typeparam name="T">Type of disposable <c>object</c>.</typeparam>
        /// <typeparam name="TResult">Type of async using block result.</typeparam>
        /// <param name="obj">Task that provides disposable <c>object</c>.</param>
        /// <param name="action">Action that should be performed on <c>object</c>.</param>
        /// <returns>All operation completion task.</returns>
        public static async Task<TResult> AsyncUsing<T, TResult>(this T obj, Func<T, Task<TResult>> action)
            where T : IAsyncDisposable
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                return await action(obj);
            }
            catch (Exception ex)
            {
                var exceptional = obj as IAbortableAsyncDisposable;
                exceptional?.HandleException(ex);
                throw;
            }
            finally
            {
                await obj.AsyncDispose();
            }
        }

        /// <summary>
        /// Using-like operation for <see cref="IAsyncDisposable"/> <c>object</c>.
        /// </summary>
        /// <typeparam name="T">Type of disposable <c>object</c>.</typeparam>
        /// <param name="obj">Task that provides disposable <c>object</c>.</param>
        /// <param name="action">Action that should be performed on <c>object</c>.</param>
        /// <returns>All operation completion task.</returns>
        public static async Task AsyncUsing<T>(this T obj, Func<T, Task> action)
            where T : IAsyncDisposable
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                await action(obj);
            }
            catch (Exception ex)
            {
                var exceptional = obj as IAbortableAsyncDisposable;
                exceptional?.HandleException(ex);

                throw;
            }
            finally
            {
                await obj.AsyncDispose();
            }
        }

        /// <summary>
        /// Using-like operation for <see cref="IAsyncDisposable"/> <c>object</c>.
        /// </summary>
        /// <typeparam name="T">Type of disposable <c>object</c>.</typeparam>
        /// <param name="obj">Task that provides disposable <c>object</c>.</param>
        /// <param name="action">Action that should be performed on <c>object</c>.</param>
        /// <returns>All operation completion task.</returns>
        public static async Task AsyncUsing<T>(this T obj, Action<T> action)
            where T : IAsyncDisposable
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action(obj);
            }
            catch (Exception ex)
            {
                var exceptional = obj as IAbortableAsyncDisposable;
                exceptional?.HandleException(ex);

                throw;
            }
            finally
            {
                await obj.AsyncDispose();
            }
        }
    }
}