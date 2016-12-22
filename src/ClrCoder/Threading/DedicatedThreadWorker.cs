// <copyright file="DedicatedThreadWorker.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Worker that uses same thread for all tasks.
    /// </summary>
    [PublicAPI]
    public class DedicatedThreadWorker : IDisposable
    {
        [NotNull]
        private readonly Task _workerTask;

        private readonly BlockingCollection<Action> _workItems = new BlockingCollection<Action>();

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private CancellationToken _ct;

        /// <summary>
        /// Initializes a new instance of the <see cref="DedicatedThreadWorker"/> class.
        /// </summary>
        public DedicatedThreadWorker()
        {
            _workerTask = Task.Factory.StartNew(WorkerThreadProc, TaskCreationOptions.LongRunning);
            _ct = _cts.Token;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DedicatedThreadWorker"/> class.
        /// </summary>
        ~DedicatedThreadWorker()
        {
            Debug.Assert(false, "Worker should be disposed from code.");

            // ReSharper disable once HeuristicUnreachableCode
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            EnsureNotDisposing();
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Schedules task to be executed in the worker thread.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        public void ExecuteSync(Action action)
        {
            EnsureNotDisposing();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            _workItems.Add(
                () =>
                    {
                        try
                        {
                            action();
                            taskCompletionSource.SetResult(true);
                        }
                        catch (Exception ex)
                        {
                            taskCompletionSource.SetException(ex);
                        }
                    });

            taskCompletionSource.Task.Wait();
        }

        /// <summary>
        /// Schedules task to be executed in the worker thread.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="func">Function to execute.</param>
        /// <returns>Function result.</returns>
        public T ExecuteSync<T>(Func<T> func)
        {
            EnsureNotDisposing();
            var taskCompletionSource = new TaskCompletionSource<T>();
            _workItems.Add(
                () =>
                    {
                        try
                        {
                            taskCompletionSource.SetResult(func());
                        }
                        catch (Exception ex)
                        {
                            taskCompletionSource.SetException(ex);
                        }
                    });
            return taskCompletionSource.Task.Result;
        }

        /// <summary>
        /// Disposes <c>object</c>.
        /// </summary>
        /// <param name="disposing">Indicates that dispose was directly called.</param>
        protected virtual void Dispose(bool disposing)
        {
            _cts.Cancel();
            _workerTask.Wait();
        }

        private void EnsureNotDisposing()
        {
            if (_ct.IsCancellationRequested)
            {
                throw new InvalidOperationException("Worker disposed or disposing.");
            }
        }

        private void WorkerThreadProc()
        {
            for (;;)
            {
                Action action;
                if (!_workItems.TryTake(out action))
                {
                    try
                    {
                        action = _workItems.Take(_ct);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }

                action();
            }
        }
    }
}