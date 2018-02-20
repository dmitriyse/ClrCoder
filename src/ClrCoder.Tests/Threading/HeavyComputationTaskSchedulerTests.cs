// <copyright file="HeavyComputationTaskSchedulerTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ClrCoder.Threading;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="HeavyComputationTaskSchedulerTests"/>.
    /// </summary>
    [TestFixture]
    public class HeavyComputationTaskSchedulerTests
    {
        /// <summary>
        /// Tests for the init/shutdown mechanics.
        /// </summary>
        /// <returns>Async execution TPL tasks.</returns>
        [Test]
        public async Task InitShutdownTest()
        {
            var scheduler = new HeavyComputationTaskScheduler();
            await scheduler.DisposeAsync();
        }
#if NET461 || NETCOREAPP2_0
        /// <summary>
        /// Checks that scheduling satisfies minimal required criteria: max threads usage and threads priority.
        /// </summary>
        /// <returns>Async execution TPL tasks.</returns>
        [Test]
        public async Task SchedulingSimpleTest()
        {
            const int TasksCount = 5000;
            var threadIds = new HashSetEx<int>();
            var priorities = new HashSetEx<ThreadPriority>();
            var taskWasNotContinuedInline = false;
            var tasks = new Task[TasksCount];
            var rnd = new Random(0);
            for (int i = 0; i < TasksCount; i++)
            {
                tasks[i] = HeavyComputationTaskScheduler.Run(
                    async () =>
                        {
                            lock (threadIds)
                            {
                                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                                priorities.Add(Thread.CurrentThread.Priority);
                            }

                            Thread.SpinWait(100_000 * rnd.Next(3, 6));

                            await Task.Yield();

                            lock (threadIds)
                            {
                                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                                priorities.Add(Thread.CurrentThread.Priority);
                            }

                            await Task.Delay(1);

                            var tid = Thread.CurrentThread.ManagedThreadId;

                            await Task.CompletedTask;

                            if (tid != Thread.CurrentThread.ManagedThreadId)
                            {
                                taskWasNotContinuedInline = true;
                            }

                            lock (threadIds)
                            {
                                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                                priorities.Add(Thread.CurrentThread.Priority);
                            }

                            Thread.SpinWait(100_000 * rnd.Next(3, 6));
                        });
            }

            await Task.WhenAll(tasks);

            //// threadIds.Count.Should().Be(Environment.ProcessorCount);
            priorities.Count.Should().Be(1);
            priorities.Single().Should().Be(ThreadPriority.BelowNormal);
            taskWasNotContinuedInline.Should().BeFalse();
        }
#endif
    }
}