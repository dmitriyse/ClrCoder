// <copyright file="ThreadPoolBehaviorAtFullCpuLoadTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETCOREAPP1_0 && !NETCOREAPP1_1
namespace ClrCoder.Tests.Threading
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;
    using System.Threading;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Strange ThreadPool behavior tests.
    /// </summary>
    [TestFixture]
    [Category("Manual")]
    public class ThreadPoolBehaviorAtFullCpuLoadTests
    {
        private const int ItemsInTest = 3000;

        private const double TargetTestTimeInSeconds = 4;

        private readonly double _itemProcessingTimeInSeconds =
            (TargetTestTimeInSeconds * Environment.ProcessorCount) / ItemsInTest;

        private int _spinsForMiddleLoad = 1000000;

        private int _spinsForSseLoad = 1000000;

        /// <summary>
        /// Synchronous worker pool scheduling.
        /// </summary>
        /// <param name="cpuLoad">The cpu load style.</param>
        /// <param name="useThreadPool">Enables to schedule next work items through thread pool.</param>
        [TestCase("low", false)]
        [TestCase("middle", false)]
        [TestCase("high", false)]
        [TestCase("low", true)]
        [TestCase("middle", true)]
        [TestCase("high", true)]
        [Test]
        public void SyncSchedulingTest(string cpuLoad, bool useThreadPool)
        {
            // Running test.
            SyncSchedulingTestImpl(cpuLoad, useThreadPool);
        }

        [Test]
        public void ThreadPoolUnderHighCpuLoadTest()
        {
            ThreadPool.GetMinThreads(out var minThreads, out var _);
            ThreadPool.GetMaxThreads(out var maxThreads, out var _);
            TestContext.Progress.WriteLine($"ThreadPool: MinThreads={minThreads} MaxThreads={maxThreads}");

            int availableThreads = WarmUpThreadPoolThreads();

            // Running test.
            double idealSpeed = SyncSchedulingTestImpl("high", true);

            double realSpeed = 0;

            DoWithThreadPoolActivity(
                () => { realSpeed = SyncSchedulingTestImpl("high", true); },
                availableThreads - 2);

            TestContext.WriteLine(
                $"realSpeed: {realSpeed:F2} item/sec, idealSpeed: {idealSpeed:F2} item/sec, efficiency: {(realSpeed / idealSpeed) * 100.0:F2}%");
        }

        ////private static Task WrapWorkersPoolAction(WorkersPool workersPool, Action action)
        ////{
        ////    var tcs = new TaskCompletionSource<int>();
        ////    workersPool.EnqueueWorkItem(
        ////        () =>
        ////            {
        ////                try
        ////                {
        ////                    action();
        ////                    Task.Run(() => tcs.SetResult(42));
        ////                }
        ////                catch (Exception ex)
        ////                {
        ////                    Task.Run(() => tcs.SetException(ex));
        ////                }
        ////            });
        ////    return tcs.Task;
        ////}

        /////// <summary>
        /////// Synchronous worker pool scheduling.
        /////// </summary>
        /////// <param name="cpuLoad">The cpu load style.</param>
        /////// <returns>The async execution TPL task.</returns>
        ////[TestCase("low")]
        ////[TestCase("middle")]
        ////[TestCase("high")]
        ////public async Task AsyncSchedulingTest(string cpuLoad)
        ////{
        ////    // We need to be free from possibly special NUnit scheduling.
        ////    // So running the test on action genuine ThreadPool.
        ////    await Task.Run(() => AsyncSchedulingTestImpl(cpuLoad));

        ////    ////// Direct call also works
        ////    ////await AsyncSchedulingTestImpl(cpuLoad);
        ////}

        [OneTimeSetUp]
        public void TestFixtureSetUpAttribute()
        {
            // Warming up assemblies.
            // Task.Run(() => { }).GetAwaiter().GetResult();
            // ThreadPool.UnsafeQueueUserWorkItem(state => { }, null);

            // Performing initial benchmark.
            int benchSpinsCount = 10000000;
            var sw = Stopwatch.StartNew();
            Thread.SpinWait(benchSpinsCount);
            _spinsForMiddleLoad = (int)((benchSpinsCount / sw.Elapsed.TotalSeconds) * _itemProcessingTimeInSeconds);

            int benchSseCount = 1000000;

            sw = Stopwatch.StartNew();

            // SSE workload.
            Vector4 v = new Vector4(0.53F, 0.21F, 0.42F, 0.52F);
            Vector4 a = new Vector4(0.13F, 0.25F, 0.33F, 0.98F);
            for (int i = 0; i < benchSseCount; i++)
            {
                a *= v;
            }

            _spinsForSseLoad = (int)((benchSseCount / sw.Elapsed.TotalSeconds) * _itemProcessingTimeInSeconds);
        }

        private void DoWithThreadPoolActivity(Action action, int degreeOfParallelism)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            ManualResetEvent mre = new ManualResetEvent(false);
            int activeWorkItems = 1;

            void WorkAndSchedule()
            {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref activeWorkItems);

                // Emulating some work.
                string a = "my beauty: ";
                for (int i = 0; i < 100; i++)
                {
                    a += i;
                }

                // And some OS waits.
                Thread.Sleep(50);

                var currentlyActiveCount = Interlocked.Decrement(ref activeWorkItems);
                if (currentlyActiveCount == 0)
                {
                    mre.Set();
                }

                if (!ct.IsCancellationRequested)
                {
                    ThreadPool.UnsafeQueueUserWorkItem(
                        state => { WorkAndSchedule(); },
                        null);
                }
            }

            for (int i = 0; i < degreeOfParallelism; i++)
            {
                ThreadPool.UnsafeQueueUserWorkItem(
                    state => { WorkAndSchedule(); },
                    null);
            }

            try
            {
                action();
            }
            finally
            {
                Interlocked.Decrement(ref activeWorkItems);
                cts.Cancel();
                mre.WaitOne();
            }
        }

        ////        private async Task AsyncSchedulingTestImpl(string cpuLoad)
        ////        {
        ////            using (var workersPool = new WorkersPool())
        ////            {
        ////                var workItemToProcess = new ConcurrentQueue<int>();
        ////                int processedWorkItems = 0;

        ////                // Collecting all waiting occurrences.
        ////                var queueWaitingOccurrences = new List<(int queueLength, double waitTimeInSeconds)>(ItemsInTest);
        ////                workersPool.RegisterEmptyQueueCondition = waitOccurrence =>
        ////                    {
        ////                        lock (queueWaitingOccurrences)
        ////                        {
        ////                            queueWaitingOccurrences.Add(waitOccurrence);
        ////                        }
        ////                    };

        ////                var wrappedStopWatch = new Stopwatch[1];
        ////                double totalProcessingTime = 0;

        ////                async Task WorkAndSchedule()
        ////                {
        ////                    if (!workItemToProcess.TryDequeue(out var workItemId))
        ////                    {
        ////                        return;
        ////                    }

        ////                    try
        ////                    {
        ////                        if (workItemId == 0)
        ////                        {
        ////                            wrappedStopWatch[0] = Stopwatch.StartNew();
        ////                        }

        ////                        ////TestContext.Progress.WriteLine($"Starting work item {workItemId}");
        ////                        // ReSharper disable once AccessToDisposedClosure
        ////                        await WrapWorkersPoolAction(
        ////                            workersPool,
        ////                            () =>
        ////                                {               
        ////                                    RunWorkload(cpuLoad);
        ////                                });

        ////                        ////TestContext.Progress.WriteLine($"Finishing work item {workItemId}");
        ////                    }
        ////                    catch
        ////                    {
        ////                        // Do nothing.
        ////                    }
        ////                    finally
        ////                    {
        ////                        var processedCount = Interlocked.Increment(ref processedWorkItems);
        ////                        if (processedCount == ItemsInTest)
        ////                        {
        ////                            totalProcessingTime = wrappedStopWatch[0].Elapsed.TotalSeconds;
        ////                        }

        ////                        // Fire and forget.
        ////#pragma warning disable 4014

        ////                        // ReSharper disable once AccessToModifiedClosure
        ////                        // ReSharper disable once PossibleNullReferenceException
        ////                        Task.Run(WorkAndSchedule).EnsureStarted();
        ////#pragma warning restore 4014
        ////                    }
        ////                }

        ////                // Scheduling work items.
        ////                for (int i = 0; i < ItemsInTest; i++)
        ////                {
        ////                    workItemToProcess.Enqueue(i);
        ////                }

        ////                // Firing initial work items. It's also action degree of parallelism.
        ////                for (int i = 0; i < Environment.ProcessorCount * 16; i++)
        ////                {
        ////                    // Fire and forget.
        ////#pragma warning disable 4014

        ////                    // ReSharper disable once AccessToModifiedClosure
        ////                    // ReSharper disable once PossibleNullReferenceException
        ////                    Task.Run(WorkAndSchedule).EnsureStarted();
        ////#pragma warning restore 4014
        ////                }

        ////                // When the work items queue of the WorkersPool becomes filled, starting processing.
        ////                workersPool.StartProcessing();

        ////                // Stupid but effective wait
        ////                while (processedWorkItems < ItemsInTest)
        ////                {
        ////                    await Task.Delay(500);
        ////                }

        ////                double totalLoosedTime = 0;
        ////                foreach (var w in queueWaitingOccurrences)
        ////                {
        ////                    TestContext.WriteLine(
        ////                        $"queue wait time = {w.waitTimeInSeconds * 1000.0:F6} ms (queueLength = {w.queueLength}");
        ////                    totalLoosedTime += w.waitTimeInSeconds;
        ////                }

        ////                double waitLossPercentage = (totalLoosedTime / totalProcessingTime) * 100.0;
        ////                TestContext.WriteLine(
        ////                    $"Performance = {ItemsInTest / totalProcessingTime:F2} items/sec, QueueWaitLoss={waitLossPercentage:F4}%");
        ////                waitLossPercentage.Should().BeLessThan(5.0, "We are loosing more that 5% on queue waiting.");
        ////            }
        ////        }
        private void RunWorkload(string cpuLoad)
        {
            switch (cpuLoad)
            {
                case "low":
                    Thread.Sleep((int)(_itemProcessingTimeInSeconds * 1000));
                    break;
                case "middle":
                    Thread.SpinWait(_spinsForMiddleLoad);
                    break;
                case "high":
                    // SSE workload.
                    Vector4 v = new Vector4(0.53F, 0.21F, 0.42F, 0.52F);
                    Vector4 a = new Vector4(0.13F, 0.25F, 0.33F, 0.98F);
                    for (int i = 0; i < _spinsForSseLoad; i++)
                    {
                        a *= v;
                    }

                    break;
            }
        }

        private double SyncSchedulingTestImpl(string cpuLoad, bool useThreadPool)
        {
            using (var workersPool = new WorkersPool())
            {
                var workItemToProcess = new ConcurrentQueue<int>();
                int processedWorkItems = 0;

                // Collecting all waiting occurrences.
                var queueWaitingOccurrences = new List<(int queueLength, double waitTimeInSeconds)>(ItemsInTest);
                workersPool.RegisterEmptyQueueCondition = waitOccurrence =>
                    {
                        lock (queueWaitingOccurrences)
                        {
                            queueWaitingOccurrences.Add(waitOccurrence);
                        }

                        ////TestContext.Progress.WriteLine(
                        ////    $"queue wait time = {waitOccurrence.waitTimeInSeconds * 1000.0:F6} ms (queueLength = {waitOccurrence.queueLength})");
                    };

                var wrappedStopWatch = new Stopwatch[1];
                double totalProcessingTime = 0;

                void WorkAndSchedule()
                {
                    if (!workItemToProcess.TryDequeue(out var workItemId))
                    {
                        return;
                    }

                    try
                    {
                        if (workItemId == 0)
                        {
                            wrappedStopWatch[0] = Stopwatch.StartNew();
                        }

                        RunWorkload(cpuLoad);
                    }
                    catch
                    {
                        // Do nothing.
                    }
                    finally
                    {
                        var processedCount = Interlocked.Increment(ref processedWorkItems);
                        if (processedCount == ItemsInTest)
                        {
                            totalProcessingTime = wrappedStopWatch[0].Elapsed.TotalSeconds;
                        }

                        // ReSharper disable AccessToDisposedClosure
                        // ReSharper disable AccessToModifiedClosure
                        // ReSharper disable AssignNullToNotNullAttribute
                        if (useThreadPool)
                        {
                            ThreadPool.UnsafeQueueUserWorkItem(
                                s => { workersPool.EnqueueWorkItem(WorkAndSchedule); },
                                null);
                        }
                        else
                        {
                            workersPool.EnqueueWorkItem(WorkAndSchedule);
                        }

                        // ReSharper restore AccessToDisposedClosure
                        // ReSharper restore AccessToModifiedClosure
                        // ReSharper restore AssignNullToNotNullAttribute
                    }
                }

                // Scheduling work items.
                for (int i = 0; i < ItemsInTest; i++)
                {
                    workItemToProcess.Enqueue(i);
                }

                // Firing initial work items. It's also action degree of parallelism.
                for (int i = 0; i < Environment.ProcessorCount * 16; i++)
                {
                    workersPool.EnqueueWorkItem(WorkAndSchedule);
                }

                // When the work items queue of the WorkersPool becomes filled, starting processing.
                workersPool.StartProcessing();

                // Stupid but effective wait
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                while (processedWorkItems < ItemsInTest)
                {
                    Thread.Sleep(500);
                }

                double totalLoosedTime = 0;
                foreach (var w in queueWaitingOccurrences)
                {
                    // TestContext.WriteLine(
                    // $"queue wait time = {w.waitTimeInSeconds * 1000.0:F6} ms (queueLength = {w.queueLength})");
                    totalLoosedTime += w.waitTimeInSeconds;
                }

                totalLoosedTime = totalLoosedTime / Environment.ProcessorCount;

                double waitLossPercentage = (totalLoosedTime / totalProcessingTime) * 100.0;
                TestContext.WriteLine(
                    $"Performance = {ItemsInTest / totalProcessingTime:F2} items/sec, QueueWaitLoss={waitLossPercentage:F4}%");
                waitLossPercentage.Should().BeLessThan(5.0, "We are loosing more that 5% on queue waiting.");

                return ItemsInTest / totalProcessingTime;
            }
        }

        private int WarmUpThreadPoolThreads()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            int warmupsToPerformLeft = 64;
            var visitedThreadIds = new ConcurrentDictionary<int, int>();

            void WorkAndSchedule()
            {
                int warmupsLeft = Interlocked.Decrement(ref warmupsToPerformLeft);
                if (warmupsLeft >= 0)
                {
                    visitedThreadIds.TryAdd(Thread.CurrentThread.ManagedThreadId, 42);

                    Thread.Sleep(2000);
                    if (warmupsLeft == 0)
                    {
                        TestContext.Progress.WriteLine(
                            $"Parallel warm-up finished. Visited threads on warmup: {visitedThreadIds.Count}");
                        mre.Set();
                    }
                    else
                    {
                        ThreadPool.UnsafeQueueUserWorkItem(
                            state => { WorkAndSchedule(); },
                            null);
                    }
                }
            }

            for (int i = 0; i < 32; i++)
            {
                ThreadPool.UnsafeQueueUserWorkItem(
                    state => { WorkAndSchedule(); },
                    null);
            }

            mre.WaitOne();

            return visitedThreadIds.Count;
        }

        private class WorkersPool : IDisposable
        {
            private readonly Thread[] _workerThreads;

            private readonly CancellationTokenSource _shutdownCts;

            private readonly CancellationToken _shutdownCt;

            private readonly BlockingCollection<Action> _workItemsQueue = new BlockingCollection<Action>();

            private bool _threadsWasStarted;

            public WorkersPool()
            {
                _shutdownCts = new CancellationTokenSource();
                _shutdownCt = _shutdownCts.Token;

                _workerThreads = new Thread[Environment.ProcessorCount];
                for (int i = 0; i < _workerThreads.Length; i++)
                {
                    _workerThreads[i] = new Thread(WorkerThreadBody);
                }
            }

            /// <summary>
            /// Handler for the empty work items queue condition.
            /// </summary>
            public Action<(int queueLength, double waitTimeInSeconds)> RegisterEmptyQueueCondition { get; set; }

            public void Dispose()
            {
                _shutdownCts.Cancel();

                foreach (Thread workerThread in _workerThreads)
                {
                    if (_threadsWasStarted)
                    {
                        workerThread.Join();
                    }
                }
            }

            public void EnqueueWorkItem(Action action)
            {
                // ReSharper disable once MethodSupportsCancellation
                _workItemsQueue.Add(action);
            }

            public void StartProcessing()
            {
                _threadsWasStarted = true;

                foreach (var workerThread in _workerThreads)
                {
                    workerThread.Start();
                }
            }

            private void WorkerThreadBody()
            {
                while (!_shutdownCt.IsCancellationRequested)
                {
                    if (!_workItemsQueue.TryTake(out var workItem))
                    {
                        try
                        {
                            int count = _workItemsQueue.Count;

                            var st = Stopwatch.StartNew();

                            // Detecting blocking wait.
                            workItem = _workItemsQueue.Take(_shutdownCt);

                            RegisterEmptyQueueCondition?.Invoke((count, st.Elapsed.TotalSeconds));
                        }
                        catch (OperationCanceledException)
                        {
                            // Do nothing, this is valid shutdown path.
                            break;
                        }
                    }

                    try
                    {
                        // Executing work item.
                        workItem();
                    }
                    catch
                    {
                        // Do nothing.
                    }
                }
            }
        }
    }
}
#endif