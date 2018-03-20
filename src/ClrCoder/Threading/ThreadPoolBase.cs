// <copyright file="ThreadPoolBase.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
#if !NETSTANDARD1_0
    using System.Collections.Concurrent;
#endif
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Validation;

    /// <summary>
    /// Base implementation for custom thread pools.
    /// TODO: Implement IAsyncHandler.
    /// </summary>
    public abstract class ThreadPoolBase : DelegatedAsyncDisposable
    {
        private readonly HashSet<WorkerThread> _workerThreads = new HashSet<WorkerThread>();

#if !NETSTANDARD1_0
        private readonly BlockingCollection<ThreadPoolTask> _queuedTasks = new BlockingCollection<ThreadPoolTask>();
#endif

        [CanBeNull]
        private Task _allWorkersDisposedTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolBase"/> class.
        /// </summary>
        protected ThreadPoolBase([CanBeNull] object lifetimeSyncRoot = null)
            : base(lifetimeSyncRoot)
        {
        }

        /// <summary>
        /// Queues new work item to thread pool.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="state"></param>
        public void QueueWorkItem(Action<object> action, [CanBeNull] object state)
        {
            VxArgs.NotNull(action, nameof(action));

#if !NETSTANDARD1_0
            _queuedTasks.Add(new ThreadPoolTask(action, state));
#else
            throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// Call this proc to allow thread pool to use current thread to schedule tasks.
        /// </summary>
        /// <param name="workerObjectReceiver">The callback proc </param>
        protected void AcceptNewWorker([CanBeNull] Action<WorkerThread> workerObjectReceiver = null)
        {
            // It can be called from unmanaged. code.
            // Here we can only rely on the OS.
            // Thread.CurrentThread can work unpredictable, so we should use as less .Net API as possible inside this proc.
            WorkerThread workerThread;
            lock (DisposeSyncRoot)
            {
                if (IsDisposeStarted)
                {
                    throw new InvalidOperationException(
                        "Cannot accept new worker threads while shutdown already in progress.");
                }

                workerThread = new WorkerThread(this);
                _workerThreads.Add(workerThread);
            }

            try
            {
                workerObjectReceiver?.Invoke(workerThread);
            }
            catch
            {
                // Do nothing.
            }

            try
            {
                while (!workerThread.IsDisposeStarted)
                {
                    double queueWaitTimeInSeconds = 0;
#if NETSTANDARD2_0

                    if (!_queuedTasks.TryTake(out ThreadPoolTask taskToExecute))
                    {
                        var sw = Stopwatch.StartNew();
                        try
                        {
                            taskToExecute = _queuedTasks.Take(workerThread.ShutdownCancellationToken);
                            queueWaitTimeInSeconds = sw.Elapsed.TotalSeconds;
                        }
                        catch (OperationCanceledException)
                        {
                            // This is normal exit.
                            break;
                        }
                    }

#else
                    ThreadPoolTask taskToExecute;
                    throw new NotImplementedException();
#endif
                    workerThread.IsRunningTask = true;
                    workerThread.QueueWaitTimeInSeconds = queueWaitTimeInSeconds;
                    try
                    {
                        taskToExecute.Run();
                    }
                    finally
                    {
                        workerThread.QueueWaitTimeInSeconds = null;
                        workerThread.IsRunningTask = false;
                    }
                }
            }
            finally
            {
                workerThread.WorkerThreadFreed();
            }
        }

        /// <summary>
        /// Disposes <c>object</c>.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        protected override async Task DisposeAsyncCore()
        {
            Debug.Assert(_allWorkersDisposedTask != null, "_allWorkersDisposedTask != null");

            // ReSharper disable once PossibleNullReferenceException
            await _allWorkersDisposedTask;
        }

        /// <inheritdoc/>
        protected override void OnDisposeStarted()
        {
            // We are under lifetime lock here.
            // ------------------------------------
            var workerThreadDisposeTasks = new Task[_workerThreads.Count];
            WorkerThread[] threads = _workerThreads.ToArray();
            for (int i = 0; i < threads.Length; i++)
            {
                var wt = threads[i];
                workerThreadDisposeTasks[i] = wt.DisposeAsync().EnsureStarted();
            }

            _allWorkersDisposedTask = Task.WhenAll(workerThreadDisposeTasks);

            base.OnDisposeStarted();
        }

        private struct ThreadPoolTask
        {
            private readonly Action<object> _action;

            [CanBeNull]
            private readonly object _state;

            public ThreadPoolTask(Action<object> action, [CanBeNull] object state)
            {
                _action = action;
                _state = state;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Run()
            {
                _action(_state);
            }
        }

        /// <summary>
        /// Encapsulates worker thread.
        /// </summary>
        public class WorkerThread : AsyncDisposableBase
        {
            [ThreadStatic]
            private static WorkerThread _currentWorker;

            private readonly ThreadPoolBase _threadPool;

            private readonly CancellationTokenSource _shutdownCts;

            private readonly TaskCompletionSource<VoidResult> _shutdownCompletedCompletionSource;

            /// <summary>
            /// Initializes a new instance of the <see cref="WorkerThread"/> class.
            /// </summary>
            /// <param name="threadPool">The owner thread pool.</param>
            internal WorkerThread(ThreadPoolBase threadPool)
                : base(threadPool.DisposeSyncRoot)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                Debug.Assert(threadPool != null, "threadPool != null");

                _threadPool = threadPool;
                _shutdownCts = new CancellationTokenSource();
                ShutdownCancellationToken = _shutdownCts.Token;

                _shutdownCompletedCompletionSource = new TaskCompletionSource<VoidResult>();
                _currentWorker = this;
            }

            /// <summary>
            /// The <see cref="WorkerThread"/> corresponding to the current thread (or null).
            /// </summary>
            [CanBeNull]
            public static WorkerThread CurrentWorker => _currentWorker;

            /// <summary>
            /// Shows if the current thread is currently executes task.
            /// Expected that this property will be inspected in this thread.
            /// </summary>
            public bool IsRunningTask { get; internal set; }

            /// <summary>
            /// The cancellation token that is set when thread enters shutdown state.
            /// </summary>
            public CancellationToken ShutdownCancellationToken { get; }

            /// <summary>
            /// Time that elapsed while gathering current work item from the queue. It's defined only when work item is being
            /// processed.
            /// </summary>
            public double? QueueWaitTimeInSeconds { get; internal set; }

            /// <summary>
            /// The last call in a worker thread before it goes away.
            /// </summary>
            internal void WorkerThreadFreed()
            {
                Debug.Assert(ReferenceEquals(_currentWorker, this), "_current == this");

                _currentWorker = null;
                _shutdownCompletedCompletionSource.SetResult(default);
            }

            /// <inheritdoc/>
            protected override async Task DisposeAsyncCore()
            {
                // Waiting worker thread proc exit.
                try
                {
                    await _shutdownCompletedCompletionSource.Task;
                }
                finally
                {
                    lock (DisposeSyncRoot)
                    {
                        // Fully disposed state here.
                        _threadPool._workerThreads.Remove(this);
                    }
                }
            }

            /// <inheritdoc/>
            protected override void OnDisposeStarted()
            {
                // Here we already under lifetime lock.
                _shutdownCts.Cancel();
                base.OnDisposeStarted();
            }
        }
    }
}