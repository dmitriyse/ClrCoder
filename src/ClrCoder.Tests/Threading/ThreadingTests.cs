// <copyright file="ThreadingTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Threading
{
    using System.Threading.Tasks;

    using ClrCoder.Threading;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Threading related tests.
    /// </summary>
    [TestFixture]
    public class ThreadingTests
    {
        /// <summary>
        /// Test for the <see cref="ThreadingExtensions.WithSyncDetection{T}"/>.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task WithAsyncDetectionTest()
        {
            bool isTaskDelayAsync;
            await Task.Delay(100).WithSyncDetection(out isTaskDelayAsync);
            isTaskDelayAsync.Should().BeTrue();

            bool isCompleteTaskWaitAsync;

            await Task.CompletedTask.WithSyncDetection(out isCompleteTaskWaitAsync);
            isCompleteTaskWaitAsync.Should().BeFalse();
        }
    }
}