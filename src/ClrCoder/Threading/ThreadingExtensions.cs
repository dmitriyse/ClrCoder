﻿// <copyright file="ThreadingExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Validation;

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
        /// Ensures that task is started, call <see cref="Task.Start()"/> if the task in the <see cref="TaskStatus.WaitingToRun"/>
        /// state.
        /// </summary>
        /// <param name="task">The task to verify. null is allowed.</param>
        /// <returns>The same value as the input task.</returns>
        [CanBeNull]
        [ContractAnnotation("task:null=>null; task:notnull => notnull")]
        public static Task EnsureStarted([CanBeNull] this Task task)
        {
            if (task == null)
            {
                return null;
            }

            if (task.Status == TaskStatus.WaitingToRun)
            {
                task.Start();
            }

            return task;
        }

        /// <summary>
        /// Ensures that task is started, call <see cref="Task.Start()"/> if the task in the <see cref="TaskStatus.WaitingToRun"/>
        /// state.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the task.</typeparam>
        /// <param name="task">The task to verify. null is allowed.</param>
        /// <returns>The same value as the input task.</returns>
        [CanBeNull]
        [ContractAnnotation("task:null=>null; task:notnull => notnull")]
        public static Task<T> EnsureStarted<T>([CanBeNull] this Task<T> task)
        {
            if (task == null)
            {
                return null;
            }

            if (task.Status == TaskStatus.WaitingToRun)
            {
                task.Start();
            }

            return task;
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
        /// Gets value from the async results dictionary or call create methods.
        /// </summary>
        /// <typeparam name="TKey">The type of the resource key.</typeparam>
        /// <typeparam name="TValue">The type of the result value.</typeparam>
        /// <param name="asyncResultsDictionary">The dictionary with asynchronous results.</param>
        /// <param name="key">The key of result to get or create.</param>
        /// <param name="createFunc">The result creation async function.</param>
        /// <param name="allowRetry">Allows to retry call to createFunc when it throws an exception.</param>
        /// <returns>Async result task, corresponding to the specified key.</returns>
        public static ValueTask<TValue> GetOrCreateAsync<TKey, TValue>(
            this IDictionary<TKey, ValueTask<TValue>> asyncResultsDictionary,
            TKey key,
            Func<TKey, ValueTask<TValue>> createFunc,
            bool allowRetry = true)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            VxArgs.NotNull(asyncResultsDictionary, nameof(asyncResultsDictionary));
            VxArgs.NotNull(createFunc, nameof(createFunc));

            lock (asyncResultsDictionary)
            {
                if (asyncResultsDictionary.TryGetValue(key, out var asyncResult))
                {
                    return asyncResult;
                }

                asyncResult = new ValueTask<TValue>(
                    Task.Run(
                        async () =>
                            {
                                try
                                {
                                    return await createFunc(key);
                                }
                                catch (Exception)
                                {
                                    if (allowRetry)
                                    {
                                        lock (asyncResultsDictionary)
                                        {
                                            asyncResultsDictionary.Remove(key);
                                        }
                                    }

                                    throw;
                                }
                            }));

                asyncResultsDictionary.Add(key, asyncResult);
                return asyncResult;
            }
        }

        /// <summary>
        /// Gets value from the async results dictionary or call create methods.
        /// </summary>
        /// <typeparam name="TKey">The type of the resource key.</typeparam>
        /// <typeparam name="TValue">The type of the result value.</typeparam>
        /// <param name="asyncResultsDictionary">The dictionary with asynchronous results.</param>
        /// <param name="key">The key of result to get or create.</param>
        /// <param name="createFunc">The result creation async function.</param>
        /// <param name="allowRetry">Allows to retry call to createFunc when it throws an exception.</param>
        /// <returns>Async result task, corresponding to the specified key.</returns>
        public static Task<TValue> GetOrCreateAsync<TKey, TValue>(
            this IDictionary<TKey, Task<TValue>> asyncResultsDictionary,
            TKey key,
            Func<TKey, Task<TValue>> createFunc,
            bool allowRetry = true)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            VxArgs.NotNull(asyncResultsDictionary, nameof(asyncResultsDictionary));
            VxArgs.NotNull(createFunc, nameof(createFunc));

            lock (asyncResultsDictionary)
            {
                if (asyncResultsDictionary.TryGetValue(key, out var asyncResult))
                {
                    return asyncResult;
                }

                asyncResult =
                    Task.Run(
                        async () =>
                            {
                                try
                                {
                                    return await createFunc(key);
                                }
                                catch (Exception)
                                {
                                    if (allowRetry)
                                    {
                                        lock (asyncResultsDictionary)
                                        {
                                            asyncResultsDictionary.Remove(key);
                                        }
                                    }

                                    throw;
                                }
                            });

                asyncResultsDictionary.Add(key, asyncResult);
                return asyncResult;
            }
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
        [CanBeNull]
        public static object GetResult(this Task task)
        {
            return task.GetType().GetTypeInfo().GetDeclaredProperty("Result")?.GetValue(task);
        }

        /// <summary>
        /// Transforms task to task with timeout.
        /// </summary>
        /// <param name="task">The task to convert.</param>
        /// <param name="timeout">The timeout time.</param>
        /// <param name="cancellationToken">
        /// Cancellation token that cancels original task. If cancellation fires before timeout,
        /// result task will wait original task regardless of timeout.
        /// </param>
        /// <returns>Task with the timeout.</returns>
        public static async Task TimeoutAfter(
            this Task task,
            TimeSpan timeout,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                await task;
                return;
            }

            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout should be positive.");
            }

            if (task.IsCompleted)
            {
                await task;
                return;
            }

            Task timeoutTask = Task.Delay(timeout, cancellationToken);

            await Task.WhenAny(task, timeoutTask);
            if (task.IsCompleted || timeoutTask.IsCanceled)
            {
                await task;
                return;
            }

            throw new TimeoutException();
        }

        /// <summary>
        /// Transforms task to task with timeout.
        /// </summary>
        /// <typeparam name="TResult">The result of the original task.</typeparam>
        /// <param name="task">The task to convert.</param>
        /// <param name="timeout">The timeout time.</param>
        /// <param name="cancellationToken">
        /// Cancellation token that cancels original task. If cancellation fires before timeout,
        /// result task will wait original task regardless of timeout.
        /// </param>
        /// <returns>Task with the timeout.</returns>
        public static async Task<TResult> TimeoutAfter<TResult>(
            this Task<TResult> task,
            TimeSpan timeout,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                return await task;
            }

            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout should be positive.");
            }

            if (task.IsCompleted)
            {
                return await task;
            }

            Task timeoutTask = Task.Delay(timeout, cancellationToken);

            await Task.WhenAny(task, timeoutTask);
            if (task.IsCompleted || timeoutTask.IsCanceled)
            {
                return await task;
            }

            throw new TimeoutException();
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