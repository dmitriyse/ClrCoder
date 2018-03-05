// <copyright file="ResearchBenchmarkTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
#if NETCOREAPP2_0
namespace ClrCoder.Tests.Dsp
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using NUnit.Framework;

    /// <summary>
    /// TPL performance research.
    /// </summary>
    [TestFixture]
    public class ResearchBenchmarkTests
    {
        /// <summary>
        /// Fastest possible CLR event loop benchmark. Result: 157 millions/sec.
        /// </summary>
        [Test]
        [Category("Manual")]
        public void FastestEventLoopBenchmark()
        {
            // ReSharper disable once UnusedVariable
            var obj = new object();

            const int TestSize = 100_000_000;

            var eventLoop = new FastestEventLoop();
            eventLoop.ProcessEvents(1);
            Stopwatch stopwatch = Stopwatch.StartNew();

            eventLoop.ProcessEvents(TestSize);
            stopwatch.Stop();
            double singleCoreSpeed = (TestSize * 1000.0) / stopwatch.ElapsedMilliseconds;

            void BenchmarkProc()
            {
                var el = new FastestEventLoop();

                el.ProcessEvents(TestSize);
            }

            // Assuming that we are running on the HyperThreading CPU.
            int trueCoresCount = Environment.ProcessorCount;

            var threads = Enumerable.Range(0, trueCoresCount).Select(x => new Thread(BenchmarkProc)).ToList();

            stopwatch = Stopwatch.StartNew();
            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            stopwatch.Stop();
            double multiCoreSpeed = (TestSize * threads.Count * 1000.0) / stopwatch.ElapsedMilliseconds;
            TestContext.WriteLine(
                $"single-thread:{singleCoreSpeed / 1000_000.0:F4} millions/sec  {trueCoresCount}-thread: {multiCoreSpeed / 1000_000.0:F4} millions/sec scalability={(multiCoreSpeed / (singleCoreSpeed * threads.Count)) * 100.0:F3}%");
        }

        /// <summary>
        /// Result: ~ 4.5 Millions/sec with TPL Yield, 20 Millions/sec with special fast Yield, 350 Millions with no Yield (but
        /// with true await).
        /// </summary>
        [Test]
        [Category("Manual")]
        public void TaskSchedulerEventLoopBenchmark()
        {
            const int TestSize = 100_000_000;

            var ts = new SingleThreadTaskScheduler();

            // Warm-up
            var t = Task.Factory.StartNew(() => { }, CancellationToken.None, TaskCreationOptions.None, ts);
            t.Wait();

            Stopwatch stopwatch = Stopwatch.StartNew();

            t = Task.Factory.StartNew(
                () => EmptyLoop(TestSize, ts),
                CancellationToken.None,
                TaskCreationOptions.None,
                ts).Unwrap();
            t.Wait();

            stopwatch.Stop();
            double singleCoreSpeed = (TestSize * 1000.0) / stopwatch.ElapsedMilliseconds;
            TestContext.WriteLine($"single-thread:{singleCoreSpeed / 1000_000.0:F4} millions/sec");
            TestContext.WriteLine(
                $"LocalQueued: {ts.LocalQueuedCount}; RemoteQueued: {ts.RemoteQueuedCount}; ExecutedInline: {ts.InlineExecutedCount}");

            var tasks = new List<Task>();
            var schedulers = Enumerable.Range(0, 8).Select(x => new SingleThreadTaskScheduler()).ToList();

            stopwatch = Stopwatch.StartNew();

            foreach (var scheduler in schedulers)
            {
                var tsk = Task.Factory.StartNew(
                    () => EmptyLoop(TestSize, scheduler),
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    scheduler).Unwrap();
                tasks.Add(tsk);
            }

            foreach (var tsk in tasks)
            {
                tsk.Wait();
            }

            stopwatch.Stop();
            double multiCoreSpeed = (TestSize * schedulers.Count * 1000.0) / stopwatch.ElapsedMilliseconds;
            TestContext.WriteLine(
                $"{schedulers.Count()}-thread: {multiCoreSpeed / 1000_000.0:F4} millions/sec scalability={(multiCoreSpeed / (singleCoreSpeed * schedulers.Count)) * 100.0:F3}%");
        }

        private async Task EmptyLoop(int count, SingleThreadTaskScheduler ts)
        {
            for (int i = 0; i < count; i++)
            {
                {
                    // if ((i & 0xF) == 0)
                    // await Task.Yield();
                    await ts.Yield();
                }
            }
        }

        private class FastestEventLoop
        {
            private readonly Action[] _actions = new Action[3];

            private readonly Action _doSomeWorkAction;

            private int _scheduledCount;

            private int _processedCount;

            private int _itemsToProcess;

            public FastestEventLoop()
            {
                _doSomeWorkAction = DoSomeWork;
            }

            public void ProcessEvents(int eventsCount)
            {
                _itemsToProcess = eventsCount;
                _processedCount = 0;
                _scheduledCount = 0;
                _actions[_scheduledCount++] = _doSomeWorkAction;
                while (_scheduledCount > 0)
                {
                    _scheduledCount--;
                    _actions[0]();
                }
            }

            private void DoSomeWork()
            {
                if (++_processedCount < _itemsToProcess)
                {
                    _actions[_scheduledCount++] = _doSomeWorkAction;
                }
            }
        }

        private class SingleThreadTaskScheduler : TaskScheduler
        {
            private static readonly Task[] EmptyTasksArray = new Task[0];

            [ThreadStatic]
            private static int _currentThreadId;

            private readonly Queue<Task> _tasksQueue = new Queue<Task>();

            private readonly Queue<Action> _fastYielded = new Queue<Action>();

            private readonly BlockingCollection<Action> _directExecutionActions = new BlockingCollection<Action>();

            private readonly int _eventLoopThreadId;

            //// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
            private readonly Thread _thread;

            private long _localQueuedCount;

            private long _remoteQueuedCount;

            private long _inlineExecutedCount;

            public SingleThreadTaskScheduler()
            {
                _thread = new Thread(ThreadProc);
                _thread.Start();

                _eventLoopThreadId = _thread.ManagedThreadId;
            }

            public long LocalQueuedCount => _localQueuedCount;

            public long RemoteQueuedCount => _remoteQueuedCount;

            public long InlineExecutedCount => _inlineExecutedCount;

            private int GetCurrentThreadIdFast
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    if (_currentThreadId == 0)
                    {
                        _currentThreadId = Thread.CurrentThread.ManagedThreadId;
                    }

                    return _currentThreadId;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FastYieldAwaitable Yield()
            {
                return new FastYieldAwaitable(this);
            }

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return EmptyTasksArray;
            }

            protected override void QueueTask([NotNull] Task task)
            {
                if (GetCurrentThreadIdFast == _eventLoopThreadId)
                {
                    _localQueuedCount++;
                    _tasksQueue.Enqueue(task);
                }
                else
                {
                    Interlocked.Increment(ref _remoteQueuedCount);
                    _directExecutionActions.Add(() => _tasksQueue.Enqueue(task));
                }
            }

            protected override bool TryExecuteTaskInline([NotNull] Task task, bool taskWasPreviouslyQueued)
            {
                if (GetCurrentThreadIdFast == _eventLoopThreadId)
                {
                    _inlineExecutedCount++;
                    return TryExecuteTask(task);
                }

                _directExecutionActions.Add(() => _tasksQueue.Enqueue(task));
                return false;
            }

            private void ThreadProc()
            {
                while (true)
                {
                    while (_fastYielded.TryDequeue(out var fya))
                    {
                        fya();
                    }

                    for (int i = 0; i < 1000; i++)
                    {
                        if (_tasksQueue.TryDequeue(out var task))
                        {
                            TryExecuteTask(task);
                        }
                        else
                        {
                            break;
                        }
                    }

                    while (_directExecutionActions.TryTake(out var action))
                    {
                        action();
                    }

                    if ((_tasksQueue.Count == 0) && (_fastYielded.Count == 0))
                    {
                        var action = _directExecutionActions.Take();
                        action();
                    }
                }

                // ReSharper disable once FunctionNeverReturns
            }

            public struct FastYieldAwaitable
            {
                private readonly SingleThreadTaskScheduler _owner;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public FastYieldAwaitable(SingleThreadTaskScheduler owner)
                {
                    _owner = owner;
                }

                //// ReSharper disable once UnusedMethodReturnValue.Local
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public FastYieldAwaiter GetAwaiter() => new FastYieldAwaiter(_owner);
            }

            public struct FastYieldAwaiter : INotifyCompletion
            {
                private readonly SingleThreadTaskScheduler _owner;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public FastYieldAwaiter(SingleThreadTaskScheduler owner)
                {
                    _owner = owner;
                }

                public bool IsCompleted => false;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnCompleted([NotNull] Action continuation)
                {
                    if (_owner.GetCurrentThreadIdFast == _currentThreadId)
                    {
                        _owner._fastYielded.Enqueue(continuation);
                    }
                    else
                    {
                        _owner._directExecutionActions.Add(continuation);
                    }
                }

                /// <summary>Ends the await operation.</summary>
                public void GetResult()
                {
                }
            }
        }
    }
}
#endif