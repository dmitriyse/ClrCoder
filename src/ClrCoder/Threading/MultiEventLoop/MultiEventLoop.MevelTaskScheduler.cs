// <copyright file="MultiEventLoop.MevelTaskScheduler.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
namespace ClrCoder.Threading
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <content>The <see cref="MevelTaskScheduler"/> implementation. </content>
    public static partial class MultiEventLoop
    {
        /// <summary>
        /// The TPL interface for the event loop.
        /// </summary>
        public sealed class MevelTaskScheduler : TaskScheduler
        {
            private static readonly Task[] EmptyTasksArray = new Task[0];

            /// <summary>
            /// Allows to execute task by different component of the MultiEventLoop.
            /// </summary>
            /// <param name="task">The task to execute.</param>
            /// <returns>
            /// <see langword="true"/>, if the <paramref name="task"/> executed successfully, <see langword="false"/>
            /// otherwise.
            /// </returns>
            internal bool TryExecuteTaskInternal(Task task)
            {
                return TryExecuteTask(task);
            }

            /// <inheritdoc/>
            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return EmptyTasksArray;
            }

            /// <inheritdoc/>
            protected override void QueueTask([NotNull] Task task)
            {
                var curEventLoop = _currentEventLoop;
                if ((curEventLoop != null) && ((task.CreationOptions & TaskCreationOptions.PreferFairness)
                                               == TaskCreationOptions.None))
                {
                    curEventLoop.EnqueueUnsafe(task);
                }
                else
                {
                    // ReSharper disable once PossibleNullReferenceException
                    _eventLoops[GetNextEventLoopToScheduleGlobalEvent()].EnqueueRemotely(task);
                }
            }

            /// <inheritdoc/>
            protected override bool TryExecuteTaskInline([NotNull] Task task, bool taskWasPreviouslyQueued)
            {
                if ((_currentEventLoopId != 0) && ((task.CreationOptions & TaskCreationOptions.PreferFairness)
                                                   == TaskCreationOptions.None))
                {
                    return TryExecuteTask(task);
                }

                // ReSharper disable once PossibleNullReferenceException
                _eventLoops[GetNextEventLoopToScheduleGlobalEvent()].EnqueueRemotely(task);

                return false;
            }
        }
    }
}

#endif