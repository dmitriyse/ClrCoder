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
    public class DedicatedThreadWorker : IAsyncHandler, IDisposable
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
            Dispose(true);
        }

        /// <inheritdoc/>
        public void RunAsync(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                _ct.ThrowIfCancellationRequested();

                _workItems.Add(action, _ct);
            }
            catch (OperationCanceledException ex)
            {
                throw ReThrowOperationCanceled(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T>(Action<T> action, [CanBeNull] T arg)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                _ct.ThrowIfCancellationRequested();

                // This is non optimal implementation.
                _workItems.Add(
                    () => { action(arg); },
                    _ct);
            }
            catch (OperationCanceledException ex)
            {
                throw ReThrowOperationCanceled(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2>(Action<T1, T2> action, [CanBeNull] T1 arg1, [CanBeNull] T2 arg2)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                _ct.ThrowIfCancellationRequested();

                // This is non optimal implementation.
                _workItems.Add(
                    () => { action(arg1, arg2); },
                    _ct);
            }
            catch (OperationCanceledException ex)
            {
                throw ReThrowOperationCanceled(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3>(
            Action<T1, T2, T3> action,
            [CanBeNull] T1 arg1,
            [CanBeNull] T2 arg2,
            [CanBeNull] T3 arg3)
        {
            try
            {
                _ct.ThrowIfCancellationRequested();

                // This is non optimal implementation.
                _workItems.Add(
                    () => { action(arg1, arg2, arg3); },
                    _ct);
            }
            catch (OperationCanceledException ex)
            {
                throw ReThrowOperationCanceled(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3, T4>(
            Action<T1, T2, T3, T4> action,
            [CanBeNull] T1 arg1,
            [CanBeNull] T2 arg2,
            [CanBeNull] T3 arg3,
            [CanBeNull] T4 arg4)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                _ct.ThrowIfCancellationRequested();

                // This is non optimal implementation.
                _workItems.Add(
                    () => { action(arg1, arg2, arg3, arg4); },
                    _ct);
            }
            catch (OperationCanceledException ex)
            {
                throw ReThrowOperationCanceled(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3, T4, T5>(
            Action<T1, T2, T3, T4, T5> action,
            [CanBeNull] T1 arg1,
            [CanBeNull] T2 arg2,
            [CanBeNull] T3 arg3,
            [CanBeNull] T4 arg4,
            [CanBeNull] T5 arg5)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                _ct.ThrowIfCancellationRequested();

                // This is non optimal implementation.
                _workItems.Add(
                    () => { action(arg1, arg2, arg3, arg4, arg5); },
                    _ct);
            }
            catch (OperationCanceledException ex)
            {
                throw ReThrowOperationCanceled(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3, T4, T5, T6>(
            Action<T1, T2, T3, T4, T5, T6> action,
            [CanBeNull] T1 arg1,
            [CanBeNull] T2 arg2,
            [CanBeNull] T3 arg3,
            [CanBeNull] T4 arg4,
            [CanBeNull] T5 arg5,
            [CanBeNull] T6 arg6)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                _ct.ThrowIfCancellationRequested();

                // This is non optimal implementation.
                _workItems.Add(
                    () => { action(arg1, arg2, arg3, arg4, arg5, arg6); },
                    _ct);
            }
            catch (OperationCanceledException ex)
            {
                throw ReThrowOperationCanceled(ex);
            }
        }

        /// <summary>
        /// Schedules task to be executed in the worker thread.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        public void RunSync(Action action)
        {
            try
            {
                _ct.ThrowIfCancellationRequested();

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
                        },
                    _ct);

                taskCompletionSource.Task.Wait(_ct);
            }
            catch (OperationCanceledException ex)
            {
                throw ReThrowOperationCanceled(ex);
            }
        }

        /// <summary>
        /// Schedules task to be executed in the worker thread.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="func">Function to execute.</param>
        /// <returns>Function result.</returns>
        public T RunSync<T>(Func<T> func)
        {
            try
            {
                _ct.ThrowIfCancellationRequested();
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
                        },
                    _ct);

                taskCompletionSource.Task.Wait(_ct);
                return taskCompletionSource.Task.Result;
            }
            catch (OperationCanceledException ex)
            {
                throw ReThrowOperationCanceled(ex);
            }
        }

        /// <summary>
        /// Disposes <c>object</c>.
        /// </summary>
        /// <param name="disposing">Indicates that dispose was directly called.</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (_cts)
            {
                try
                {
                    _ct.ThrowIfCancellationRequested();

                    if (disposing)
                    {
                        GC.SuppressFinalize(this);
                    }

                    _cts.Cancel();

                    // ReSharper disable once MethodSupportsCancellation
                    _workerTask.Wait();
                }
                catch (OperationCanceledException ex)
                {
                    throw ReThrowOperationCanceled(ex);
                }
            }
        }

        private void EnsureNotDisposing()
        {
            if (_ct.IsCancellationRequested)
            {
                throw new InvalidOperationException("Worker disposed or disposing.");
            }
        }

        private Exception ReThrowOperationCanceled(OperationCanceledException ex)
        {
            return new InvalidOperationException("Worker disposed or disposing.");
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

                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    ThreadPool.QueueUserWorkItem(h => { throw ex; });
                }
            }
        }
    }
}