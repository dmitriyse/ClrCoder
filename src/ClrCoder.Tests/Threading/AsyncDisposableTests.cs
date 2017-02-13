// <copyright file="AsyncDisposableTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Threading
{
    using System;
    using System.Threading.Tasks;

    using ClrCoder.Threading;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="AsyncDisposableBase"/> class.
    /// </summary>
    [TestFixture]
    public class AsyncDisposableTests
    {
        /// <summary>
        /// Stress test for AsyncDisposableBase.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task AsyncDisposableStressTest()
        {
            var rnd = new Random(0);
            for (var i = 0; i < 100; i++)
            {
                var d = new MonkeyAsyncDisposalbe(rnd);
                d.StartDispose();
                if (rnd.NextBool())
                {
                    await Task.Delay(10);
                }

                if (rnd.NextBool())
                {
                    d.StartDispose();
                }

                if (rnd.NextBool())
                {
                    await Task.Delay(0);
                }

                await d.DisposeTask;

                if (rnd.NextBool())
                {
                    await Task.Delay(10);
                }

                if (rnd.NextBool())
                {
                    d.StartDispose();
                }

                if (rnd.NextBool())
                {
                    await Task.Delay(0);
                }
            }
        }

        /// <summary>
        /// Tests with <c>true</c> asynchronous await of <see cref="AsyncDisposableBase.DisposeTask"/>.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task SimpleAsynchronousAwaitTest()
        {
            var awaitable = new AsyncDisposableForSimpleTest();
            Task<Task> tst = Task.Factory.StartNew(
                async () =>
                    {
                        awaitable.StartDispose();
                        await awaitable.DisposeTask;
                    });
            await tst;

            awaitable.IsCalled.Should().BeTrue();
        }

        private class AsyncDisposableForSimpleTest : AsyncDisposableBase
        {
            public bool IsCalled { get; set; }

            protected override async Task AsyncDispose()
            {
                IsCalled = true;
            }
        }

        private class MonkeyAsyncDisposalbe : AsyncDisposableBase
        {
            private readonly Random _rnd;

            private bool _onDisposingCalled;

            private bool _disposeCalled;

            public MonkeyAsyncDisposalbe(Random rnd)
            {
                _rnd = rnd;
            }

            protected override async Task AsyncDispose()
            {
                if (_disposeCalled)
                {
                    throw new InvalidOperationException();
                }

                _disposeCalled = true;

                if (_rnd.NextBool())
                {
                    await Task.Delay(10);
                }

                SetDisposeSuspended(false);

                if (_rnd.NextBool())
                {
                    StartDispose();
                }

                if (_rnd.NextBool())
                {
                    await Task.Delay(10);
                }
            }

            protected override void OnDisposeStarted()
            {
                if (_onDisposingCalled)
                {
                    throw new InvalidOperationException();
                }

                if (!_disposeCalled && _rnd.NextBool())
                {
                    SetDisposeSuspended(true);
                }

                _onDisposingCalled = true;

                if (_rnd.NextBool())
                {
                    StartDispose();
                }

                if (_rnd.NextBool())
                {
                    StartDispose();
                }

                SetDisposeSuspended(false);
                base.OnDisposeStarted();
            }
        }
    }
}