// <copyright file="MultiEventLoopTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using ClrCoder.Threading;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="MultiEventLoop"/> component.
    /// </summary>
    [TestFixture]
    public class MultiEventLoopTests
    {
        /// <summary>
        /// Simple init/shutdown cycle test.
        /// </summary>
        /// <param name="count">The number of init/shutdown iterations.</param>
        [Test]
        [TestCase(1)]
        [TestCase(1000)]
        public void SimpleInitShutdownTest(int count)
        {
            for (int i = 0; i < count; i++)
            {
                MultiEventLoop.Initialize();
                MultiEventLoop.Shutdown();
            }
        }

        /// <summary>
        /// Event loop simplest iteration benchmark.
        /// </summary>
        [Test]
        [Category("Manual")]
        public void SimpleMultiEventLoopBenchmark()
        {
            const int TestSize = 30_000_000;

            MultiEventLoop.Initialize();
            try
            {
                // Warm-up
                var t = Task.Factory.StartNew(
                    () => { },
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    MultiEventLoop.Scheduler);
                t.Wait();

                Stopwatch stopwatch = Stopwatch.StartNew();

                t = Task.Factory.StartNew(
                    () => EmptyLoop(TestSize),
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    MultiEventLoop.Scheduler).Unwrap();
                t.Wait();

                stopwatch.Stop();
                double singleCoreSpeed = (TestSize * 1000.0) / stopwatch.ElapsedMilliseconds;
                TestContext.WriteLine($"single-thread:{singleCoreSpeed / 1000_000.0:F4} millions/sec");

                var tasks = new List<Task>();

                stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < Environment.ProcessorCount; i++)
                {
                    t = Task.Factory.StartNew(
                        () => EmptyLoop(TestSize),
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        MultiEventLoop.Scheduler).Unwrap();
                    tasks.Add(t);
                }

                foreach (var tsk in tasks)
                {
                    tsk.Wait();
                }

                stopwatch.Stop();
                double multiCoreSpeed = ((long)TestSize * tasks.Count * 1000.0) / stopwatch.ElapsedMilliseconds;
                TestContext.WriteLine(
                    $"{tasks.Count}-thread: {multiCoreSpeed / 1000_000.0:F4} millions/sec scalability={(multiCoreSpeed / (singleCoreSpeed * tasks.Count)) * 100.0:F3}%");
            }
            finally
            {
                MultiEventLoop.Shutdown();
            }
        }

        private async Task EmptyLoop(int count)
        {
            var yieldAwaitable = MultiEventLoop.Yield();
            for (int i = 0; i < count; i++)
            {
                //await Task.Yield();
                await yieldAwaitable;
            }

            // ------- Use this trick to get totally unlimited calculation power, without blocking other tasks.
            ////var eventLoop = MultiEventLoop.CurrentEventLoop;
            ////for (int i = 0; i < count; i++)
            ////{
            ////    // ReSharper disable once PossibleNullReferenceException
            ////    eventLoop.DoEventsUnsafe((i & 0xFFFF) == 0);
            ////}
        }
    }
}