// <copyright file="IxThreadingTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
    using System.Threading;
    using System.Threading.Tasks;

    using ClrCoder.ComponentModel.IndirectX;
    using ClrCoder.Threading;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Threading behavior tests for IndirectX.
    /// </summary>
    [TestFixture]
    public class IxThreadingTests
    {
        /// <summary>
        /// Simple dependency without async init/dispose should be resolved fully synchronously.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task SimpleResolveShouldBeSynchronous()
        {
            var instance = new DummyObject();

            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes =>
                               rootNodes.Add<DummyObject>(
                                   instanceBuilder:
                                   new IxExistingInstanceFactoryConfig<DummyObject>(
                                       instance),
                                   disposeHandler: obj => Task.CompletedTask))
                       .Build())
                .AsyncUsing(
                    async host =>
                        {
                            int beforeAwaitThreadId = Thread.CurrentThread.ManagedThreadId;
                            using (IxLock<DummyObject> resolvedInstanceLock = await host.Resolver.Get<DummyObject>())
                            {
                                Thread.CurrentThread.ManagedThreadId.Should().Be(beforeAwaitThreadId);
                                resolvedInstanceLock.Target.Should().Be(instance);
                            }
                        });
        }

        private class DummyObject
        {
        }
    }
}