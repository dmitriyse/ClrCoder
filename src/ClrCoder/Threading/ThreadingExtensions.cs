// <copyright file="ThreadingExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Reflection;
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
        /// Overrides null task to awaitable task with default value.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="task">Task that can be null.</param>
        /// <returns>Always not null awaitable task.</returns>
        public static Task<T> GetOrDefault<T>([CanBeNull] this Task<T> task)
        {
            return task ?? Task.FromResult(default(T));
        }

        /// <summary>
        /// Overrides null task to completed task. This is analogue for the <see cref="GetOrDefault{T}"/> method.
        /// </summary>
        /// <param name="task">Task that can be null.</param>
        /// <returns>Always not null awaitable task.</returns>
        public static Task GetOrDefault([CanBeNull] this Task task)
        {
            return task ?? Task.CompletedTask;
        }

        /// <summary>
        /// Gets result of task, from unknown final type task. CoreFX Proposal: https://github.com/dotnet/corefx/issues/17094.
        /// </summary>
        /// <param name="task">Task to get result from.</param>
        /// <returns>Result of the task.</returns>
        public static object GetResult(this Task task)
        {
            return task.GetType().GetTypeInfo().GetDeclaredProperty("Result")?.GetValue(task);
        }

        /// <summary>
        /// Helps to detect synchronous execution of await <see langword="operator"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of task.</typeparam>
        /// <param name="task">Task that will be awaited.</param>
        /// <param name="handleDetectionResult">
        /// Handles detection result. Receives true if await was in synchronous form, false
        /// otherwise.
        /// </param>
        /// <returns>Awaitable object.</returns>
        public static WithSyncDetectionFromTaskAwaitable<TResult> WithSyncDetection<TResult>(
            this Task<TResult> task,
            Action<bool> handleDetectionResult)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (handleDetectionResult == null)
            {
                throw new ArgumentNullException(nameof(handleDetectionResult));
            }

            return new WithSyncDetectionFromTaskAwaitable<TResult>(task.GetAwaiter(), handleDetectionResult);
        }

        /// <summary>
        /// Helps to detect synchronous execution of await <see langword="operator"/>.
        /// </summary>
        /// <param name="task">Task that will be awaited.</param>
        /// <param name="handleDetectionResult">
        /// Handles detection result. Receives true if await was in synchronous form, false
        /// otherwise.
        /// </param>
        /// <returns>Awaitable object.</returns>
        public static WithSyncDetectionFromTaskAwaitable WithSyncDetection(
            this Task task,
            Action<bool> handleDetectionResult)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (handleDetectionResult == null)
            {
                throw new ArgumentNullException(nameof(handleDetectionResult));
            }

            return new WithSyncDetectionFromTaskAwaitable(task.GetAwaiter(), handleDetectionResult);
        }

        /// <summary>
        /// Helps to detect synchronous execution of await <see langword="operator"/>.
        /// </summary>
        /// <param name="task">Task that will be awaited.</param>
        /// <returns>Returns synchronous run flags (true for sync, false for async).</returns>
        public static async Task<bool> WithSyncDetection(this Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            int threadId = Thread.CurrentThread.ManagedThreadId;
            await task;

            return threadId == Thread.CurrentThread.ManagedThreadId;
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
                if (!_cancellationToken.IsCancellationRequested)
                {
                    // GetResult can be called directly by GetAwaiter().GetResult(). 
                    // In this case we should use synchronous style wait.
                    _cancellationToken.WaitHandle.WaitOne();
                }
            }

            /// <summary>
            /// Shows that awaitable operation finished.
            /// </summary>
            [UsedImplicitly]
            public bool IsCompleted => _cancellationToken.IsCancellationRequested;

            /// <summary>
            /// Registers continuation <c>action</c>.
            /// </summary>
            /// <param name="action">Operation continuation.</param>
            [UsedImplicitly]
            public void OnCompleted([NotNull] Action action)
            {
                if (action == null)
                {
                    throw new ArgumentNullException(nameof(action));
                }

                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                _cancellationToken.Register(action);
            }
        }
    }
}