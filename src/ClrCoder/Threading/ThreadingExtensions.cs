// <copyright file="ThreadingExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Threading related extensions methods.
    /// </summary>
    [PublicAPI]
    public static class ThreadingExtensions
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
        /// <param name="objTask">Task that provides disposable <c>object</c>.</param>
        /// <param name="action">Action that should be performed on <c>object</c>.</param>
        /// <returns>All operation completion task.</returns>
        public static async Task AsyncUsing<T>(this Task<T> objTask, Func<T, Task> action)
            where T : IAsyncDisposable
        {
            if (objTask == null)
            {
                throw new ArgumentNullException(nameof(objTask));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

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

        /// <summary>
        /// Using-like operation for <see cref="IAsyncDisposable"/> <c>object</c> with return value.
        /// </summary>
        /// <typeparam name="T">Type of disposable <c>object</c>.</typeparam>
        /// <typeparam name="TResult">Type of return value.</typeparam>
        /// <param name="objTask">Task that provides disposable <c>object</c>.</param>
        /// <param name="action">Action that should be performed on <c>object</c>.</param>
        /// <returns>All operation completion task with result.</returns>
        public static async Task<TResult> AsyncUsing<T, TResult>(this Task<T> objTask, Func<T, Task<TResult>> action)
            where T : IAsyncDisposable
        {
            if (objTask == null)
            {
                throw new ArgumentNullException(nameof(objTask));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

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

        /// <summary>
        /// Turns CancellationToken into awaitable operation.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to wait on.</param>
        /// <returns>Cancellation awaiter.</returns>
        public static CancellationTokenAwaiter GetAwaiter(this CancellationToken cancellationToken)
        {
            return new CancellationTokenAwaiter(cancellationToken);
        }

        /// <summary>
        /// Helps to detect synchronous execution of await <see langword="operator"/>.
        /// </summary>
        /// <typeparam name="T">Type of task.</typeparam>
        /// <param name="task">Task that will be awaited.</param>
        /// <param name="isAsync">Output argument, that sets to false on synchronous execution.</param>
        /// <returns>Chained fluent syntax. Returned value provided in <see cref="task"/> argument.</returns>
        public static T WithSyncDetection<T>(this T task, out bool isAsync)
            where T : Task
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            isAsync = !task.IsCompleted;
            return task;
        }

        /// <summary>
        /// Allows awaits on cancellation token.
        /// </summary>
        public struct CancellationTokenAwaiter : INotifyCompletion
        {
            private readonly CancellationToken _cancellationToken;

            /// <summary>
            /// Initializes a new instance of the <see cref="CancellationTokenAwaiter"/> struct.
            /// </summary>
            /// <param name="cancellationToken">Cancellation token to await on.</param>
            internal CancellationTokenAwaiter(CancellationToken cancellationToken)
            {
                _cancellationToken = cancellationToken;
            }

            /// <summary>
            /// Gets result - none in our case.
            /// </summary>
            [UsedImplicitly]
            public void GetResult()
            {
            }

            /// <summary>
            /// Shows that awaitable operation finished.
            /// </summary>
            [UsedImplicitly]
            public bool IsCompleted => _cancellationToken.IsCancellationRequested;

            /// <summary>
            /// Registers continuation <c>action</c>..
            /// </summary>
            /// <param name="action">Operation continuation.</param>
            [UsedImplicitly]
            public void OnCompleted(Action action)
            {
                _cancellationToken.Register(action);
            }
        }
    }
}