﻿// <copyright file="MultiEventLoop.cs" company="ClrCoder project">
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
    using System.Security;

    using JetBrains.Annotations;

    /// <summary>
    /// Task scheduler for max throughput processing applications (Mevel is acronym for the Multi Event Loop).
    /// </summary>
    /// <remarks>
    /// This component have Single-ton design due to the performance tricks limitations.
    /// </remarks>
    public static partial class MultiEventLoop
    {
        /// <summary>
        /// The global events queue.
        /// </summary>
        private static readonly BlockingCollection<MevelEvent> GlobalEventsQueue = new BlockingCollection<MevelEvent>();

        [ThreadStatic]
        [CanBeNull]
        private static EventLoop _currentEventLoop;

        /// <summary>
        /// The 1-based event loop id. If this value is zero
        /// </summary>
        [ThreadStatic]
        private static int _currentEventLoopId;

        [CanBeNull]
        private static EventLoop[] _eventLoops;

        [CanBeNull]
        private static MevelTaskScheduler _scheduler;

        /// <summary>
        /// The TPL scheduler interface to the component.
        /// </summary>
        [NotNull]
        public static MevelTaskScheduler Scheduler
        {
            get
            {
                VerifyInitialized();

                // ReSharper disable once AssignNullToNotNullAttribute
                return _scheduler;
            }
        }

        /// <summary>
        /// Forces async method continuation execution on any event loop.
        /// </summary>
        /// <returns>The awaitable struct.</returns>
        public static FairnessYieldAwaitable FairnessYield() => default;

        /// <summary>
        /// Initializes the component.
        /// </summary>
        /// <param name="eventLoopsCount">The number of event loops. By default it's equals to the number of logical processors.</param>
        public static void Initialize(int? eventLoopsCount = null)
        {
            if (_scheduler != null)
            {
                throw new InvalidOperationException("The component has already been initialized.");
            }

            if (eventLoopsCount == null)
            {
                eventLoopsCount = Environment.ProcessorCount;
            }

            _eventLoops = new EventLoop[(int)eventLoopsCount + 1];
            for (int evlId = 1; evlId < _eventLoops.Length; evlId++)
            {
                _eventLoops[evlId] = new EventLoop(evlId);
            }

            _scheduler = new MevelTaskScheduler();
        }

        /// <summary>
        /// Shuts down the component.
        /// </summary>
        public static void Shutdown()
        {
            if (_scheduler == null)
            {
                throw new InvalidOperationException("The component is not initialized.");
            }

            // ReSharper disable once PossibleNullReferenceException
            for (int i = 1; i < _eventLoops.Length; i++)
            {
                _eventLoops[i].Dispose();
            }

            // Clearing all not processed events.
            while (GlobalEventsQueue.TryTake(out var _))
            {
            }

            _scheduler = null;
        }

        /// <summary>
        /// Forces async method continuation on the same event loop (or on any event loop, if async method currently runs on a non
        /// <see cref="MultiEventLoop"/> thread).
        /// </summary>
        /// <returns>The awaitable.</returns>
        public static FastYieldAwaitable Yield() => default;

        [Conditional("DEBUG")]
        private static void VerifyInitialized()
        {
            Debug.Assert(_scheduler != null, "_scheduler!= null");
        }

        /// <summary>
        /// The awaitable for the fairness yield (<see cref="FairnessYield"/>).
        /// </summary>
        public struct FairnessYieldAwaitable
        {
            /// <summary>
            /// Gets the awaiter.
            /// </summary>
            /// <returns>The awaiter.</returns>
            public FairnessYieldAwaiter GetAwaiter() => default;
        }

        /// <summary>
        /// The awaiter for the <see cref="FairnessYield"/>.
        /// </summary>
        public struct FairnessYieldAwaiter : INotifyCompletion
        {
            /// <summary>
            /// Always incomplete to force continuation scheduling.
            /// </summary>
            public bool IsCompleted => false;

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted([NotNull] Action continuation)
            {
                GlobalEventsQueue.Add(
                    new MevelEvent
                        {
                            SimpleAction = continuation
                        });
            }

            /// <summary>
            /// It's not required only by the compiler.
            /// </summary>
            public void GetResult()
            {
            }
        }

        /// <summary>
        /// The awaitable for the fast yield (see <see cref="Yield"/>).
        /// </summary>
        public struct FastYieldAwaitable
        {
            private EventLoop _eventLoop;

            /// <summary>
            /// Gets the awaiter.
            /// </summary>
            /// <returns>The awaiter.</returns>
            public FastYieldAwaiter GetAwaiter()
            {
                if (_eventLoop == null)
                {
                    _eventLoop = _currentEventLoop;
                }

                return new FastYieldAwaiter(_eventLoop);
            }
        }

        /// <summary>
        /// The awaiter for the <see cref="Yield"/>.
        /// </summary>
        public struct FastYieldAwaiter : ICriticalNotifyCompletion
        {
            [CanBeNull]
            private readonly EventLoop _eventLoop;

            /// <summary>
            /// Initializes the struct.
            /// </summary>
            /// <param name="eventLoop">The even loop object of the current thread.</param>
            internal FastYieldAwaiter([CanBeNull] EventLoop eventLoop)
            {
                _eventLoop = eventLoop;
            }

            /// <summary>
            /// Always incomplete to force continuation scheduling.
            /// </summary>
            public bool IsCompleted => false;

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted([NotNull] Action continuation)
            {
                Debug.Assert(
                    ReferenceEquals(_eventLoop, _currentEventLoop),
                    "ReferenceEquals(_eventLoop, _currentEventLoop)");

                if (_eventLoop != null)
                {
                    _eventLoop.Enqueue(continuation);
                }
                else
                {
                    GlobalEventsQueue.Add(
                        new MevelEvent
                            {
                                SimpleAction = continuation
                            });
                }
            }

            /// <inheritdoc/>
            [SecurityCritical]
            public void UnsafeOnCompleted([NotNull] Action continuation)
            {
                Debug.Assert(
                    ReferenceEquals(_eventLoop, _currentEventLoop),
                    "ReferenceEquals(_eventLoop, _currentEventLoop)");

                if (_eventLoop != null)
                {
                    _eventLoop.Enqueue(continuation);
                }
                else
                {
                    GlobalEventsQueue.Add(
                        new MevelEvent
                            {
                                SimpleAction = continuation
                            });
                }
            }

            /// <summary>
            /// It's not required only by the compiler.
            /// </summary>
            public void GetResult()
            {
            }
        }
    }
}
#endif