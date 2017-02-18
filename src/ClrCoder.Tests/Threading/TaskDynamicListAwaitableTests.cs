// <copyright file="TaskDynamicListAwaitableTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Threading
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using ClrCoder.Threading;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="TaskDynamicListAwaitable"/>.
    /// </summary>
    [TestFixture]
    public class TaskDynamicListAwaitableTests
    {
        /// <summary>
        /// Waits some task with exception. This exception should be re-raised.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task TestWithExceptionalTasks()
        {
            var s = new Stopwatch();
            s.Start();
            try
            {
                var dynList = new TaskDynamicListAwaitable();
                dynList.AddTask(
                    ((Func<Task>)(async () => { await Task.Delay(10000); }))());
                dynList.AddTask(
                    ((Func<Task>)(async () =>
                        {
                            await Task.Delay(200);
                            throw new Exception("Test me");
                        }))());
                dynList.AddTask(
                    ((Func<Task>)(async () => { await Task.Delay(15000); }))());

                await dynList;

                Assert.Fail();
            }
            catch (Exception ex)
            {
                s.Stop();
                s.ElapsedMilliseconds.Should().BeLessThan(3000);
                ex.Should().BeOfType<Exception>();
            }
        }
    }
}