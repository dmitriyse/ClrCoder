// <copyright file="MultiEventLoop.EventLoop.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
namespace ClrCoder.Threading
{
    using System;
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
            private const int InitialQueueCapacity = 0x10000;

            private readonly int _id;

            private readonly Thread _thread;

            private readonly CancellationTokenSource _shutdownCts;

            private readonly CancellationToken _shutdownCancellationToken;

            private MevelEvent[] _localEventQueue = new MevelEvent[InitialQueueCapacity];

            private int _queueCapacityMask = InitialQueueCapacity - 1;

            private int _readPointer = 0;

            private int _queuedCount = 0;

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

            /// <inheritdoc/>
            public void Dispose()
            {
                _shutdownCts.Cancel();
                _thread.Join();
            }

            /// <summary>
            /// Schedules action with state.
            /// </summary>
            /// <param name="action">The action to schedule.</param>
            /// <param name="state">The state object for the action.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Enqueue(Action<object> action, object state)
            {
                _localEventQueue[(_readPointer + _queuedCount++) & _queueCapacityMask].SetAction(action, state);
                if (_queuedCount == _localEventQueue.Length)
                {
                    IncreaseCapacity();
                }
            }

            /// <summary>
            /// Schedules the task to execute.
            /// </summary>
            /// <param name="task">The task to execute.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Enqueue(Task task)
            {
                _localEventQueue[(_readPointer + _queuedCount++) & _queueCapacityMask].Task = task;
                if (_queuedCount == _localEventQueue.Length)
                {
                    IncreaseCapacity();
                }
            }

            /// <summary>
            /// Schedules the simple action.
            /// </summary>
            /// <param name="simpleAction">The simple action to execute.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Enqueue(Action simpleAction)
            {
                _localEventQueue[(_readPointer + _queuedCount++) & _queueCapacityMask].SimpleAction = simpleAction;
                if (_queuedCount == _localEventQueue.Length)
                {
                    IncreaseCapacity();
                }
            }

            private void EventLoopThreadProc()
            {
                _currentEventLoopId = _id;
                _currentEventLoop = this;

                while (!_shutdownCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        while (_queuedCount != 0)
                        {
                            // ---- This is most critical execution path ----
                            ref MevelEvent ev = ref _localEventQueue[_readPointer];
                            _readPointer++;
                            _readPointer &= _queueCapacityMask;
                            _queuedCount--;

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

                            // Verifying global queue every 512 events.
                            if (((_readPointer & 511) == 0) && GlobalEventsQueue.TryTake(out var gev))
                            {
                                gev.ExecuteEvent();
                            }
                        }

                        GlobalEventsQueue.Take(_shutdownCancellationToken).ExecuteEvent();
                    }
                    catch
                    {
                        // Do nothing.
                    }
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private void IncreaseCapacity()
            {
                MevelEvent[] oldArray = _localEventQueue;
                _localEventQueue = new MevelEvent[oldArray.Length * 2];

                for (int i = 0; i < _queuedCount; i++)
                {
                    _localEventQueue[_readPointer + i] = oldArray[(_readPointer + i) & _queueCapacityMask];
                }

                _queueCapacityMask = _localEventQueue.Length - 1;
            }
        }
    }
}

#endif