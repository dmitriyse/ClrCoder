// <copyright file="TaskDynamicListAwaitable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Awaits on dynamic list of tasks.
    /// </summary>
    [PublicAPI]
    public class TaskDynamicListAwaitable
    {
        private readonly HashSet<Task> _tasksToWait = new HashSet<Task>();

        /// <summary>
        /// Adds task to wait on.
        /// </summary>
        /// <param name="taskToWait">Task to wait.</param>
        public void AddTask(Task taskToWait)
        {
            if (taskToWait == null)
            {
                throw new ArgumentNullException(nameof(taskToWait));
            }

            lock (_tasksToWait)
            {
                _tasksToWait.Add(taskToWait);
            }
        }

        /// <summary>
        /// Gets awaiter.
        /// </summary>
        /// <returns>Allows <see cref="TaskDynamicListAwaitable"/> to be used in await expression.</returns>
        public TaskDynamicListAwaiter GetAwaiter()
        {
            // TODO: Optimize me.
            // Synchronous call to async method can return task in WaitingToRun state.
            Task waitAllTask = WaitAllDynamically().EnsureStarted();

            return new TaskDynamicListAwaiter(waitAllTask.GetAwaiter());
        }

        /// <summary>
        /// Gets task that completes when all added task completed. Prefer to use await directly on
        /// <see cref="TaskDynamicListAwaitable"/> instead of this method.
        /// </summary>
        /// <returns>TPL task.</returns>
        public Task Wait()
        {
            Task result = WaitAllDynamically().EnsureStarted();

            return result;
        }

        private async Task WaitAllDynamically()
        {
            while (true)
            {
                Task[] tasksToWaitCopy;
                lock (_tasksToWait)
                {
                    if (!_tasksToWait.Any())
                    {
                        break;
                    }

                    tasksToWaitCopy = _tasksToWait.ToArray();
                }

                Task task = await Task.WhenAny(tasksToWaitCopy);
                lock (_tasksToWait)
                {
                    _tasksToWait.Remove(task);
                }

                await task;
            }
        }

        /// <summary>
        /// <c>Awaiter</c> for <see cref="TaskDynamicListAwaitable"/> completion.
        /// </summary>
        public struct TaskDynamicListAwaiter : INotifyCompletion
        {
            private readonly TaskAwaiter _awaiter;

            /// <summary>
            /// Initializes a new instance of the <see cref="TaskDynamicListAwaiter"/> struct.
            /// </summary>
            /// <param name="awaiter">Awaiter for <see cref="TaskDynamicListAwaitable.Wait"/> method result.</param>
            internal TaskDynamicListAwaiter(TaskAwaiter awaiter)
            {
                _awaiter = awaiter;
            }

            /// <summary>
            /// Checks if awaitable operation is already completed.
            /// </summary>
            public bool IsCompleted => _awaiter.IsCompleted;

            /// <inheritdoc/>
            public void OnCompleted([NotNull] Action continuation)
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                _awaiter.OnCompleted(continuation);
            }

            /// <summary>
            /// Gets await result.
            /// </summary>
            public void GetResult()
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                _awaiter.GetResult();
            }
        }
    }
}