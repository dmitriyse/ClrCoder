// <copyright file="IxExistingInstanceFactoryTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
    using System.Threading.Tasks;

    using ClrCoder.ComponentModel.IndirectX;
    using ClrCoder.Threading;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Existing instance factory tests.
    /// </summary>
    [TestFixture]
    public class IxExistingInstanceFactoryTests
    {
        /// <summary>
        /// Simplest use case. Single instantiation, no dispose on parent scope dispose.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task SingleInstantiateWithoutDispose()
        {
            var instance = new DummyObject();

            await (await new IxHostBuilder()
                .Configure(
                    rootNodes =>
                        rootNodes.Add<DummyObject>(
                            factory: new IxExistingInstanceFactoryConfig<DummyObject>(instance),
                            disposeHandler: obj => Task.CompletedTask))
                .Build())
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<DummyObject> resolvedInstanceLock = await host.Resolver.Get<DummyObject>())
                            {
                                resolvedInstanceLock.Target.Should().Be(instance);
                            }
                        });
        }

        private class DummyObject
        {
        }
    }
}