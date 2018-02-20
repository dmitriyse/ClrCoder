// <copyright file="ThreadingExtensions.cs" company="ClrCoder project">
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
        private static readonly int ProcessorsCount = Environment.ProcessorCount;

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

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0

        /// <summary>
        /// Overrides null task to completed task. This is analogue for the <see cref="GetOrDefault{T}"/> method.
        /// </summary>
        /// <param name="task">Task that can be null.</param>
        /// <returns>Always not null awaitable task.</returns>
        public static Task GetOrDefault([CanBeNull] this Task task)
        {
            return task ?? Task.CompletedTask;
        }

#else

/// <summary>
/// Overrides null task to completed task. This is analogue for the <see cref="GetOrDefault{T}"/> method.
/// </summary>
/// <param name="task">Task that can be null.</param>
/// <returns>Always not null awaitable task.</returns>
        public static async Task GetOrDefault([CanBeNull] this Task task)
        {
            if (task != null)
            {
                await task;
            }
        }

#endif

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

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0

        /// <summary>
        /// Helps to detect synchronous execution of await <see langword="operator"/>.
        /// </summary>
        /// <param name="task">Task that will be awaited.</param>
        /// <returns>Returns synchronous run flags (true for sync, false for async).</returns>
        public static async ValueTask<bool> WithSyncDetection(this Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            int threadId = Thread.CurrentThread.ManagedThreadId;
            await task;

            return threadId == Thread.CurrentThread.ManagedThreadId;
        }

#endif

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
            /// Registers continuation <c>body</c>.
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

        /// <summary>
        /// Verifies that provided task is in the completed state, but not in canceled or in faulted state.
        /// This is polyfill method.
        /// See this discussion https://github.com/dotnet/corefx/issues/16745.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>The value of expression "IsCompleted &amp;&amp; !IsFaulted &amp;&amp; !IsCanceled."</returns>
        public static bool IsCompletedSuccessfully(this Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return task.IsCompleted && !task.IsFaulted && !task.IsCanceled;
        }

        /// <summary>
        /// Allows to correctly use <see cref="ManualResetEventSlim"/> as an auto reset event.
        /// See <see cref="ManualResetEventSlim.Wait()"/>.
        /// </summary>
        /// <param name="mre">The event to wait.</param>
        public static void WaitAndReset(this ManualResetEventSlim mre)
        {
            mre.Wait();
            mre.Reset();
        }

        /// <summary>
        /// Allows to correctly use <see cref="ManualResetEventSlim"/> as an auto reset event.
        /// See <see cref="ManualResetEventSlim.Wait(CancellationToken)"/>.
        /// </summary>
        /// <param name="mre">The event to wait.</param>
        /// <param name="ct">The cancellation token.</param>
        public static void WaitAndReset(this ManualResetEventSlim mre, CancellationToken ct)
        {
            mre.Wait(ct);
            mre.Reset();
        }

        /// <summary>
        /// Allows to correctly use <see cref="ManualResetEventSlim"/> as an auto reset event.
        /// See <see cref="ManualResetEventSlim.Wait(TimeSpan, CancellationToken)"/>.
        /// </summary>
        /// <param name="mre">The event to wait.</param>
        /// <param name="timeout">The wait timeout.</param>
        /// <param name="ct">The cancellation token.</param>
        public static void WaitAndReset(this ManualResetEventSlim mre, TimeSpan timeout, CancellationToken ct)
        {
            if (mre.Wait(timeout, ct))
            {
                mre.Reset();
            }
        }

        /// <summary>
        /// Allows to correctly use <see cref="ManualResetEventSlim"/> as an auto reset event.
        /// See <see cref="ManualResetEventSlim.Wait(int)"/>.
        /// </summary>
        /// <param name="mre">The event to wait.</param>
        /// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
        public static void WaitAndReset(this ManualResetEventSlim mre, int millisecondsTimeout)
        {
            if (mre.Wait(millisecondsTimeout))
            {
                mre.Reset();
            }
        }

        /// <summary>
        /// Allows to correctly use <see cref="ManualResetEventSlim"/> as an auto reset event.
        /// See <see cref="ManualResetEventSlim.Wait(CancellationToken)"/>.
        /// </summary>
        /// <param name="mre">The event to wait.</param>
        /// <param name="timeout">The wait timeout.</param>
        public static void WaitAndReset(this ManualResetEventSlim mre, TimeSpan timeout)
        {
            if (mre.Wait(timeout))
            {
                mre.Reset();
            }
        }

        /// <summary>
        /// Correctly invokes multicast delegate.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="handler">THe handler multicast delegate.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data. </param>
        /// <returns>Async execution TPL task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task InvokeAsync<TEventArgs>(
            [CanBeNull] this AsyncEventHandler<TEventArgs> handler,
            object sender,
            TEventArgs e)
            where TEventArgs : EventArgs
        {
            // In most cases events are not subscribed and fast return should be inlined by JIT.
            if (handler == null)
            {
                return TaskEx.CompletedTaskValue;
            }

            return InvokeAsyncSlow(handler, sender, e);
        }

        private static Task InvokeAsyncSlow<TEventArgs>(
            this AsyncEventHandler<TEventArgs> handler,
            object sender,
            TEventArgs e)
            where TEventArgs : EventArgs
        {
            Delegate[] invocationList = handler.GetInvocationList();

            var handlerTasks = new Task[invocationList.Length];

            // Firing all handlers.
            bool successFastPossible = true;
            for (int i = 0; i < invocationList.Length; i++)
            {
                var h = (AsyncEventHandler<TEventArgs>)invocationList[i];
                try
                {
                    var t = h(sender, e);
                    handlerTasks[i] = t;
                    if (t.IsCompletedSuccessfully())
                    {
                    }
                }
                catch (Exception ex)
                {
                    successFastPossible = false;
                    handlerTasks[i] = TaskEx.FromException(ex);
                }
            }

            if (!successFastPossible)
            {
                return Task.WhenAll(handlerTasks);
            }

            return TaskEx.CompletedTaskValue;
        }

        /// <summary>
        /// Correctly invokes multicast delegate.
        /// </summary>
        /// <param name="handler">THe handler multicast delegate.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data. </param>
        /// <returns>Async execution TPL task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task InvokeAsync(
            [CanBeNull] this AsyncEventHandler handler,
            object sender,
            EventArgs e)
        {
            // In most cases events are not subscribed and fast return should be inlined by JIT.
            if (handler == null)
            {
                return TaskEx.CompletedTaskValue;
            }

            return InvokeAsyncSlow(handler, sender, e);
        }

        private static Task InvokeAsyncSlow(
            this AsyncEventHandler handler,
            object sender,
            EventArgs e)
        {
            Delegate[] invocationList = handler.GetInvocationList();

            var handlerTasks = new Task[invocationList.Length];

            // Firing all handlers.
            bool successFastPossible = true;
            for (int i = 0; i < invocationList.Length; i++)
            {
                var h = (AsyncEventHandler)invocationList[i];
                try
                {
                    var t = h(sender, e);
                    handlerTasks[i] = t;
                    if (t.IsCompletedSuccessfully())
                    {
                    }
                }
                catch (Exception ex)
                {
                    successFastPossible = false;
                    handlerTasks[i] = TaskEx.FromException(ex);
                }
            }

            if (!successFastPossible)
            {
                return Task.WhenAll(handlerTasks);
            }

            return TaskEx.CompletedTaskValue;
        }

        /// <summary>
        /// The parallel loop with async body.
        /// </summary>
        /// <typeparam name="T">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="body">The async delegate that is invoked once per iteration.</param>
        /// <param name="maxDegreeOfParallelism">The  maximum number of concurrent tasks enabled in this operation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="scheduler">The tasks scheduler.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task ParallelForEachAsync<T>(
            this ICollection<T> source,
            Func<T, Task> body,
            int? maxDegreeOfParallelism = null,
            CancellationToken cancellationToken = default,
            [CanBeNull] TaskScheduler scheduler = null)
        {
            VxArgs.NotNull(body, nameof(body));

            if (maxDegreeOfParallelism == null)
            {
                maxDegreeOfParallelism = ProcessorsCount;
            }

            int parallelTasksCount = maxDegreeOfParallelism.Value;

            if (source is IReadOnlyCollection<T> rc)
            {
                parallelTasksCount = Math.Min(parallelTasksCount, rc.Count);
            }
            else if (source is ICollection<T> c)
            {
                parallelTasksCount = Math.Min(parallelTasksCount, c.Count);
            }

            return ParallelForEachAsync(
                new ConcurrentEnumerator<T, IEnumerator<T>>(source.GetEnumerator()),
                body,
                parallelTasksCount,
                cancellationToken,
                scheduler);
        }

        /// <summary>
        /// The parallel loop with async body.
        /// </summary>
        /// <typeparam name="T">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TList">The type of the list.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="body">The async delegate that is invoked once per iteration.</param>
        /// <param name="maxDegreeOfParallelism">The  maximum number of concurrent tasks enabled in this operation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="scheduler">The tasks scheduler.</param>
        /// <returns>Async execution TPL task.</returns>
        public static Task ParallelForEachAsync<T, TList>(
            this TList source,
            Func<T, Task> body,
            int? maxDegreeOfParallelism = null,
            CancellationToken cancellationToken = default,
            [CanBeNull] TaskScheduler scheduler = null)
            where TList : IReadOnlyList<T>
        {
            VxArgs.NotNull(body, nameof(body));

            int parallelTasksCount = maxDegreeOfParallelism ?? ProcessorsCount;

            parallelTasksCount = Math.Min(parallelTasksCount, source.Count);

            return ParallelForEachAsync(
                new ListConcurrentEnumerator<T, TList>(source),
                body,
                parallelTasksCount,
                cancellationToken,
                scheduler);
        }

        private static Task ParallelForEachAsync<T, TEnumerator>(
            TEnumerator enumerator,
            Func<T, Task> body,
            int tasksCount,
            CancellationToken cancellationToken = default,
            [CanBeNull] TaskScheduler scheduler = null)
            where TEnumerator : IConcurrentEnumerator<T>
        {
            var wrappedEnumerator = new[] { enumerator };

            async Task WorkSequenceProc()
            {
                while (wrappedEnumerator[0].TryGetNext(out var item))
                {
                    bool isYieldRequired = false;
                    try
                    {
                        Task t = null;
                        try
                        {
                            t = body(item);
                        }
                        catch
                        {
                            // Do nothing.
                        }

                        isYieldRequired = (t == null) || t.IsCompleted;

                        if (t != null)
                        {
                            await t;
                        }
                    }
                    catch
                    {
                        // Do nothing.
                    }

                    if (isYieldRequired)
                    {
                        await Task.Yield();
                    }
                }
            }

            var tasks = new Task[tasksCount];

            using (enumerator)
            {
                for (int i = 0; i < tasksCount; i++)
                {
                    tasks[i] = scheduler == null
                                   ? Task.Factory.StartNew(WorkSequenceProc, cancellationToken).Unwrap()
                                   : Task.Factory.StartNew(
                                       WorkSequenceProc,
                                       cancellationToken,
                                       TaskCreationOptions.None,
                                       scheduler).Unwrap();
                }

                return Task.WhenAll(tasks);
            }
        }
    }
}