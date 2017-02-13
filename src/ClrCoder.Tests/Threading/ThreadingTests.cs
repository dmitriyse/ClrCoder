// <copyright file="ThreadingTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Threading
{
    using System.Threading;
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
        /// General test for AwaitableEvent event.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task AwaitableEventTest()
        {
            // Simple async test.
            using (var ev = new AwaitableEvent())
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Factory.StartNew(
                    async () =>
                        {
                            await Task.Delay(50);

                            // ReSharper disable once AccessToDisposedClosure
                            ev.Set();
                        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                await ev;

                ev.IsSet.Should().BeTrue();
            }
        }

        /// <summary>
        /// CancellationTokenSource behavior test in a reentrant case.
        /// </summary>
        [Test]
        public void CancellationTokenReentrancyTest()
        {
            using (var cts = new CancellationTokenSource())
            {
                CancellationToken token = cts.Token;
                var called = false;
                token.Register(
                    () =>
                        {
                            called = true;
                            token.IsCancellationRequested.Should().BeTrue();
                        });

                cts.Cancel();

                called.Should().BeTrue();
            }
        }

        /// <summary>
        /// Test for the <see cref="ThreadingExtensions.WithSyncDetection{T}"/>.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task WithAsyncDetectionTest()
        {
            bool isTaskDelayAsync = false;
            await Task.Delay(100).WithSyncDetection(isSync=>isTaskDelayAsync = isSync);
            isTaskDelayAsync.Should().BeFalse();

            bool isCompleteTaskWaitAsync = false;
            await Task.CompletedTask.WithSyncDetection(isSync => isCompleteTaskWaitAsync = isSync);
            isCompleteTaskWaitAsync.Should().BeTrue();
        }
    }
}