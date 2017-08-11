// <copyright file="DedicatedThreadTaskScheduler.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Scheduler that uses only one thread for all scheduled task.
    /// </summary>
    public class DedicatedThreadTaskScheduler : TaskScheduler, IAsyncDisposableEx
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly BlockingCollection<Task> _queuedTasks = new BlockingCollection<Task>();

        private readonly DelegatedAsyncDisposable _asyncDisposable;

        private readonly CancellationToken _ct;

        private readonly Task _workerTask;

        private readonly TaskCompletionSource<Thread> _workerThreadCompletionSource =
            new TaskCompletionSource<Thread>();

        private readonly Thread _workerThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="DedicatedThreadTaskScheduler"/> class.
        /// </summary>
        public DedicatedThreadTaskScheduler()
        {
            _asyncDisposable = new DelegatedAsyncDisposable(AsyncDisposeImpl);
            _workerTask = Task.Factory.StartNew(WorkerThreadProc, TaskCreationOptions.LongRunning);
            _workerThread = _workerThreadCompletionSource.Task.Result;
            _ct = _cts.Token;
        }

        /// <inheritdoc/>
        public Task Disposed => _asyncDisposable.Disposed;

        /// <inheritdoc/>
        public Task DisposeAsync() => _asyncDisposable.DisposeAsync();

        /// <summary>
        /// Disposes <c>object</c>.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        protected virtual async Task AsyncDisposeImpl()
        {
            _cts.Cancel();

            // ReSharper disable once MethodSupportsCancellation
            await _workerTask;
        }

        /// <inheritdoc/>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _queuedTasks;
        }

        /// <inheritdoc/>
        protected override void QueueTask([NotNull] Task task)
        {
            _queuedTasks.Add(task, _ct);
        }

        /// <inheritdoc/>
        protected override bool TryExecuteTaskInline([NotNull] Task task, bool taskWasPreviouslyQueued)
        {
            if (Thread.CurrentThread == _workerThread)
            {
                return TryExecuteTask(task);
            }

            return false;
        }

        private void WorkerThreadProc()
        {
            _workerThreadCompletionSource.SetResult(Thread.CurrentThread);
            try
            {
                for (;;)
                {
                    Task task;
                    if (_queuedTasks.TryTake(out task, -1, _ct))
                    {
                        // This is synchronous execution.
                        bool taskExecuted = TryExecuteTask(task);
                        Debug.Assert(taskExecuted, "DedicatedThread task have some problem.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // This is normal behavior on scheduler disposing.
            }
        }
    }
}