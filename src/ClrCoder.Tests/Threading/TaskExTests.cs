// <copyright file="TaskExTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using ClrCoder.Threading;

    using Diagnostics;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="TaskEx"/>.
    /// </summary>
    [TestFixture]
    public class TaskExTests
    {
        /// <summary>
        /// Async invoke methods test.
        /// </summary>
        /// <returns>Async execution TPL tasks.</returns>
        [Test]
        public async Task AsyncInvokeTest()
        {
            CodeTimer timer = CodeTimer.Start();
            Task task = TaskEx.AsyncInvoke(DelayInSyncPart, Thread.CurrentThread.ManagedThreadId);
            timer.Time.Should().BeLessThan(0.5);
            await task;
            timer.Time.Should().BeGreaterThan(2.0);

            try
            {
                await TaskEx.AsyncInvoke(ThrowTestException);
            }
            catch (InvalidOperationException)
            {
                // This is expected exception.
            }

            int my42 = await TaskEx.AsyncInvoke(GiveMeTheAnswer);
            my42.Should().Be(42);

            try
            {
                // ReSharper disable once UnusedVariable
                int hope42 = await TaskEx.AsyncInvoke(GiveMeTheError);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // This is expected exception.
            }
        }

        private async Task DelayInSyncPart(int threadId)
        {
            Thread.CurrentThread.ManagedThreadId.Should().NotBe(threadId);

            Thread.Sleep(1000);
            await Task.Delay(1000);
        }

        private async Task<int> GiveMeTheAnswer()
        {
            await Task.Delay(200);
            return 42;
        }

        private async Task<int> GiveMeTheError()
        {
            await Task.Delay(200);
            throw new InvalidOperationException("Expected exception instead of 42.");
        }

        private async Task ThrowTestException()
        {
            await Task.Delay(200);
            throw new InvalidOperationException("This is expected exception");
        }
    }
}