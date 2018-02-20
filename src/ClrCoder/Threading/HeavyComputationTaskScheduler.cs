// <copyright file="HeavyComputationTaskScheduler.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Task scheduler that allows to fully utilize CPU resources without affecting latencies.
    /// </summary>
    public class HeavyComputationTaskScheduler : TaskScheduler, IAsyncDisposableEx
    {
        private static readonly Task[] EmptyTasksArray = new Task[0];

        private static readonly object InstanceSyncRoot = new object();

        [CanBeNull]
        private static HeavyComputationTaskScheduler _instance;

        private readonly CpuAlignedThreadPool _threadPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeavyComputationTaskScheduler"/> class.
        /// </summary>
        internal HeavyComputationTaskScheduler()
        {
            _threadPool = new CpuAlignedThreadPool();
        }

        /// <summary>
        /// The single class instance.
        /// </summary>
        [NotNull]
        public static HeavyComputationTaskScheduler Instance
        {
            get
            {
                if (_instance != null)
                {
                    // ReSharper disable once InconsistentlySynchronizedField
                    return _instance;
                }

                lock (InstanceSyncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new HeavyComputationTaskScheduler();
#if NETSTANDARD2_0
                        AppDomain.CurrentDomain.ProcessExit += CurrentDomainProcessExit;
#endif
                    }

                    return _instance;
                }
            }
        }

        /// <inheritdoc/>
        public Task Disposed => _threadPool.Disposed;

        /// <summary>Queues the specified work to run on the Heavy computation thread pool and returns a task handle for that work.</summary>
        /// <returns>A task that represents the work queued to execute in the ThreadPool.</returns>
        /// <param name="action">The work to execute asynchronously</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action"/> parameter was null.</exception>
        public static Task Run(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, Instance);
        }

        /// <summary>Queues the specified work to run on the Heavy computation thread pool and returns a task handle for that work.</summary>
        /// <returns>A task that represents the work queued to execute in the ThreadPool.</returns>
        /// <param name="action">The work to execute asynchronously</param>
        /// <param name="cancellationToken">A cancellation token that should be used to cancel the work</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action"/> parameter was null.</exception>
        /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.Threading.CancellationTokenSource"/>
        /// associated with <paramref name="cancellationToken"/> was disposed.
        /// </exception>
        public static Task Run(Action action, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, Instance);
        }

        /// <summary>
        /// Queues the specified work to run on the Heavy computation thread pool and returns a proxy for the  task
        /// returned by <paramref name="function"/>.
        /// </summary>
        /// <returns>A task that represents a proxy for the task returned by <paramref name="function"/>.</returns>
        /// <param name="function">The work to execute asynchronously</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> parameter was null.</exception>
        public static Task Run(Func<Task> function)
        {
            return Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.None, Instance).Unwrap();
        }

        /// <summary>
        /// Queues the specified work to run on the Heavy computation thread pool and returns a proxy for the  task
        /// returned by <paramref name="function"/>.
        /// </summary>
        /// <returns>A task that represents a proxy for the task returned by <paramref name="function"/>.</returns>
        /// <param name="function">The work to execute asynchronously</param>
        /// <param name="cancellationToken">A cancellation token that should be used to cancel the work</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> parameter was null.</exception>
        /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.Threading.CancellationTokenSource"/>
        /// associated with <paramref name="cancellationToken"/> was disposed.
        /// </exception>
        public static Task Run(Func<Task> function, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, Instance).Unwrap();
        }

        /// <summary>
        /// Queues the specified work to run on the Heavy computation thread pool and returns a Task(TResult) handle for
        /// that work.
        /// </summary>
        /// <returns>A Task(TResult) that represents the work queued to execute in the ThreadPool.</returns>
        /// <param name="function">The work to execute asynchronously</param>
        /// <typeparam name="TResult">The result type of the task.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> parameter was null.</exception>
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            return Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.None, Instance);
        }

        /// <summary>
        /// Queues the specified work to run on the Heavy computation thread pool and returns a Task(TResult) handle for
        /// that work.
        /// </summary>
        /// <returns>A Task(TResult) that represents the work queued to execute in the ThreadPool.</returns>
        /// <param name="function">The work to execute asynchronously</param>
        /// <param name="cancellationToken">A cancellation token that should be used to cancel the work</param>
        /// <typeparam name="TResult">The result type of the task.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> parameter was null.</exception>
        /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.Threading.CancellationTokenSource"/>
        /// associated with <paramref name="cancellationToken"/> was disposed.
        /// </exception>
        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, Instance);
        }

        /// <summary>
        /// Queues the specified work to run on the Heavy computation thread pool and returns a proxy for the
        /// Task(TResult) returned by <paramref name="function"/>.
        /// </summary>
        /// <returns>A Task(TResult) that represents a proxy for the Task(TResult) returned by <paramref name="function"/>.</returns>
        /// <param name="function">The work to execute asynchronously</param>
        /// <typeparam name="TResult">The type of the result returned by the proxy task.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> parameter was null.</exception>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
            return Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.None, Instance).Unwrap();
        }

        /// <summary>
        /// Queues the specified work to run on the Heavy computation thread pool and returns a proxy for the
        /// Task(TResult) returned by <paramref name="function"/>.
        /// </summary>
        /// <returns>A Task(TResult) that represents a proxy for the Task(TResult) returned by <paramref name="function"/>.</returns>
        /// <param name="function">The work to execute asynchronously</param>
        /// <param name="cancellationToken">A cancellation token that should be used to cancel the work</param>
        /// <typeparam name="TResult">The type of the result returned by the proxy task.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> parameter was null.</exception>
        /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.Threading.CancellationTokenSource"/>
        /// associated with <paramref name="cancellationToken"/> was disposed.
        /// </exception>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, Instance).Unwrap();
        }

#if NETSTANDARD2_0
        private static void CurrentDomainProcessExit(object sender, EventArgs e)
        {
            HeavyComputationTaskScheduler instanceToDispose;
            lock (InstanceSyncRoot)
            {
                instanceToDispose = _instance;
            }

            // Synchronously waiting for shutdown.
            instanceToDispose?.DisposeAsync().GetAwaiter().GetResult();
        }

#endif

        /// <inheritdoc/>
        public Task DisposeAsync() => _threadPool.DisposeAsync();

        /// <inheritdoc/>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return EmptyTasksArray;
        }

        /// <inheritdoc/>
        protected override void QueueTask([NotNull] Task task)
        {
            _threadPool.QueueWorkItem(ExecuteTaskAction, task);
        }

        /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
        /// <param name="task">The task to be removed.</param>
        /// <returns>Whether the task could be found and removed.</returns>
        protected override sealed bool TryDequeue(Task task)
        {
            return false;
        }

        /// <inheritdoc/>
        protected override bool TryExecuteTaskInline([NotNull] Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        private void ExecuteTaskAction(object state)
        {
            TryExecuteTask((Task)state);
        }

        private class CpuAlignedThreadPool : ThreadPoolBase
        {
            public CpuAlignedThreadPool()
            {
                ManualResetEventSlim threadStartedEvent = new ManualResetEventSlim();

#if NETSTANDARD2_0
                for (int i = 0; i < (int)(Environment.ProcessorCount * 1.25); i++)
                {
                    StartNewThread(threadStartedEvent, ThreadPriority.BelowNormal);
                }
#else
            throw new NotImplementedException("Use unmanaged API to slowdown thread priority. For NetStandard 1.0 and NetStandard 1.1 use long running task instead of Tthread.");
#endif
            }

#if NETSTANDARD2_0
            private void StartNewThread(ManualResetEventSlim threadStartedEvent, ThreadPriority priority)
            {
                var thread = new Thread(ThreadProc);
                thread.IsBackground = true;

                thread.Priority = priority;
                thread.Start(threadStartedEvent);
                threadStartedEvent.WaitAndReset();
            }

#endif

            private void ThreadProc(object state)
            {
                var threadStartedEvent = (ManualResetEventSlim)state;

                // Selling current thread into the slavery.
                AcceptNewWorker(
                    worker => { threadStartedEvent.Set(); });
            }
        }
    }
}