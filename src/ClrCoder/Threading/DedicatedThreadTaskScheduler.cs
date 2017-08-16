// <copyright file="DedicatedThreadTaskScheduler.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Scheduler that uses only one thread for all scheduled task.
    /// </summary>
    public class DedicatedThreadTaskScheduler : TaskScheduler, IAsyncDisposableEx
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly DelegatedAsyncDisposable _asyncDisposable;

        private readonly CancellationToken _ct;

        private readonly Task _workerTask;

        private readonly TaskCompletionSource<Guid> _workerThreadCompletionSource =
            new TaskCompletionSource<Guid>();

        private readonly ThreadLocal<Guid?> _currentThreadId = new ThreadLocal<Guid?>();

        private readonly AutoResetEvent _newTaskAvailableEvent = new AutoResetEvent(false);

        private ImmutableQueue<Task> _queuedTasks = ImmutableQueue<Task>.Empty;

        private Guid _workerThreadId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DedicatedThreadTaskScheduler"/> class.
        /// </summary>
        public DedicatedThreadTaskScheduler()
        {
            _asyncDisposable = new DelegatedAsyncDisposable(AsyncDisposeImpl);
            _workerTask = Task.Factory.StartNew(WorkerThreadProc, TaskCreationOptions.LongRunning);
            _workerThreadId = _workerThreadCompletionSource.Task.Result;
            _ct = _cts.Token;
        }

        /// <inheritdoc/>
        public Task Disposed => _asyncDisposable.Disposed;

        /// <inheritdoc/>
        public Task DisposeAsync() => _asyncDisposable.DisposeAsync();

        /// <inheritdoc/>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _queuedTasks;
        }

        /// <inheritdoc/>
        protected override void QueueTask([NotNull] Task task)
        {
            InterlockedEx.InterlockedUpdate(ref _queuedTasks, (q, t) => q.Enqueue(t), task);
            _newTaskAvailableEvent.Set();
        }

        /// <inheritdoc/>
        protected override bool TryExecuteTaskInline([NotNull] Task task, bool taskWasPreviouslyQueued)
        {
            if (_currentThreadId.Value == _workerThreadId)
            {
                return TryExecuteTask(task);
            }

            return false;
        }

        /// <summary>
        /// Disposes <c>object</c>.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        private async Task AsyncDisposeImpl()
        {
            _cts.Cancel();

            // ReSharper disable once MethodSupportsCancellation
            await _workerTask;
        }

        private void WorkerThreadProc()
        {
            Debug.Assert(_currentThreadId.Value == null);
            try
            {
                _currentThreadId.Value = Guid.NewGuid();
                _workerThreadCompletionSource.SetResult(_currentThreadId.Value.Value);

                try
                {
                    for (;;)
                    {
                        _ct.ThrowIfCancellationRequested();
                        Task dequeuedTask = InterlockedEx.InterlockedUpdate(
                            ref _queuedTasks,
                            q =>
                                {
                                    Task t = null;
                                    if (q.Any())
                                    {
                                        var nq = q.Dequeue(out t);
                                        return (nq, t);
                                    }
                                    else
                                    {
                                        return (q, null);
                                    }
                                });

                        if (dequeuedTask == null)
                        {
                            // TODO: Add spinwait
                            _newTaskAvailableEvent.WaitOne(TimeSpan.FromSeconds(1));
                        }
                        else
                        {
                            // This is synchronous execution.
                            bool taskExecuted = TryExecuteTask(dequeuedTask);
                            Debug.Assert(taskExecuted, "DedicatedThread task have some problem.");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // This is normal behavior on scheduler disposing.
                }
            }
            finally
            {
                _newTaskAvailableEvent.Dispose();
                _currentThreadId.Value = null;
            }
        }
    }
}