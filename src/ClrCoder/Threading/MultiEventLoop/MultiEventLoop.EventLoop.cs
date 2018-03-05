// <copyright file="MultiEventLoop.EventLoop.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <content>The <see cref="EventLoop"/> implementation.</content>
    public static partial class MultiEventLoop
    {
        /// <summary>
        /// The single event loop.
        /// </summary>
        public class EventLoop : IDisposable
        {
            private const int InitialQueueCapacity = 0x1000;

            /// <summary>
            /// Currently queued events count.
            /// </summary>
            internal int QueuedCount;

            private readonly int _id;

            private readonly Thread _thread;

            private readonly CancellationTokenSource _shutdownCts;

            private readonly CancellationToken _shutdownCancellationToken;

            private readonly BlockingCollection<MevelEvent> _remoteScheduledEvents =
                new BlockingCollection<MevelEvent>();

            private MevelEvent[] _localEventQueue = new MevelEvent[InitialQueueCapacity];

            private int _queueCapacityMask = InitialQueueCapacity - 1;

            private int _readPointer;

            // ReSharper disable once InconsistentNaming
            private bool _haveRemoteEvents_BadlyVolatile = false;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventLoop"/> class.
            /// </summary>
            /// <param name="id">The event loop id.</param>
            internal EventLoop(int id)
            {
                _shutdownCts = new CancellationTokenSource();
                _shutdownCancellationToken = _shutdownCts.Token;

                _id = id;
                _thread = new Thread(EventLoopThreadProc);
                _thread.Start();
            }

            /// <summary>
            /// Returns true, if locally and remote queues are empty.
            /// </summary>
            internal bool IsEmpty => QueuedCount + _remoteScheduledEvents.Count == 0;

            /// <inheritdoc/>
            public void Dispose()
            {
                _shutdownCts.Cancel();
                _thread.Join();
            }

            /// <summary>
            /// Performs nested event loop processing.
            /// </summary>
            /// <param name="includeNonLocal">Forces to try fetch event from global event loop.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DoEventsUnsafe(bool includeNonLocal)
            {
                if (includeNonLocal || (QueuedCount != 0))
                {
                    DoEventsCore(includeNonLocal);
                }
            }

            /// <summary>
            /// Schedules action with state.
            /// </summary>
            /// <param name="action">The action to schedule.</param>
            /// <param name="state">The state object for the action.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnqueueRemotely(Action<object> action, object state)
            {
                _remoteScheduledEvents.Add(
                    new MevelEvent
                        {
                            Action = action,
                            State = state
                        });

                _haveRemoteEvents_BadlyVolatile = true;
            }

            /// <summary>
            /// Schedules the task to execute.
            /// </summary>
            /// <param name="task">The task to execute.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnqueueRemotely(Task task)
            {
                _remoteScheduledEvents.Add(
                    new MevelEvent
                        {
                            Task = task
                        });

                _haveRemoteEvents_BadlyVolatile = true;
            }

            /// <summary>
            /// Schedules the simple action.
            /// </summary>
            /// <param name="simpleAction">The simple action to execute.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnqueueRemotely(Action simpleAction)
            {
                _remoteScheduledEvents.Add(
                    new MevelEvent
                        {
                            SimpleAction = simpleAction
                        });

                _haveRemoteEvents_BadlyVolatile = true;
            }

            /// <summary>
            /// Schedules action with state.
            /// </summary>
            /// <param name="action">The action to schedule.</param>
            /// <param name="state">The state object for the action.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnqueueUnsafe(Action<object> action, object state)
            {
                Debug.Assert(_currentEventLoop == this, "_currentEventLoop == this");
                _localEventQueue[(_readPointer + QueuedCount++) & _queueCapacityMask].SetAction(action, state);
                if (QueuedCount == _localEventQueue.Length)
                {
                    IncreaseCapacity();
                }
            }

            /// <summary>
            /// Schedules the task to execute.
            /// </summary>
            /// <param name="task">The task to execute.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnqueueUnsafe(Task task)
            {
                Debug.Assert(_currentEventLoop == this, "_currentEventLoop == this");

                _localEventQueue[(_readPointer + QueuedCount++) & _queueCapacityMask].Task = task;
                if (QueuedCount == _localEventQueue.Length)
                {
                    IncreaseCapacity();
                }
            }

            /// <summary>
            /// Schedules the simple action.
            /// </summary>
            /// <param name="simpleAction">The simple action to execute.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnqueueUnsafe(Action simpleAction)
            {
                Debug.Assert(_currentEventLoop == this, "_currentEventLoop == this");

                _localEventQueue[(_readPointer + QueuedCount++) & _queueCapacityMask].SimpleAction = simpleAction;
                if (QueuedCount == _localEventQueue.Length)
                {
                    IncreaseCapacity();
                }
            }

            private bool DoEventsCore(bool includeNonLocal)
            {
                try
                {
                    if (includeNonLocal)
                    {
                        while (_remoteScheduledEvents.TryTake(out var rev))
                        {
                            rev.ExecuteEvent();
                        }

                        _haveRemoteEvents_BadlyVolatile = false;
                    }

                    while (QueuedCount != 0)
                    {
                        // ---- This is most critical execution path ----
                        ref MevelEvent ev = ref _localEventQueue[_readPointer];
                        _readPointer++;
                        _readPointer &= _queueCapacityMask;
                        QueuedCount--;

                        if (ev.SimpleAction != null)
                        {
                            var simpleAction = ev.SimpleAction;
                            ev.SimpleAction = null;

                            simpleAction();
                        }
                        else if (ev.Action != null)
                        {
                            var action = ev.Action;
                            ev.Action = null;

                            var state = ev.State;
                            ev.State = null;

                            // ReSharper disable once PossibleNullReferenceException
                            action(state);
                        }
                        else
                        {
                            Debug.Assert(ev.Task != null, "Task != null");
                            var task = ev.Task;
                            ev.Task = null;

                            // ReSharper disable once PossibleNullReferenceException
                            // ReSharper disable once AssignNullToNotNullAttribute
                            _scheduler.TryExecuteTaskInternal(task);
                        }

                        if (((_readPointer & 0xFF) == 0) || _haveRemoteEvents_BadlyVolatile)
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch
                {
                    // Do nothing.
                }

                return true;
            }

            private void EventLoopThreadProc()
            {
                _currentEventLoopId = _id;
                _currentEventLoop = this;

                try
                {
                    while (!_shutdownCancellationToken.IsCancellationRequested)
                    {
                        while (DoEventsCore(true))
                        {
                        }

                        var ev = _remoteScheduledEvents.Take(_shutdownCancellationToken);

                        try
                        {
                            ev.ExecuteEvent();
                        }
                        catch
                        {
                            // Do nothing.
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Valid exit.
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private void IncreaseCapacity()
            {
                MevelEvent[] oldArray = _localEventQueue;
                _localEventQueue = new MevelEvent[oldArray.Length * 2];

                for (int i = 0; i < QueuedCount; i++)
                {
                    _localEventQueue[_readPointer + i] = oldArray[(_readPointer + i) & _queueCapacityMask];
                }

                _queueCapacityMask = _localEventQueue.Length - 1;
            }
        }
    }
}

#endif