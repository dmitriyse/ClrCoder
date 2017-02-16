// <copyright file="IxBaseTests.cs" company="ClrCoder project">
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
    /// Tests for <see cref="IxHost"/> class.
    /// </summary>
    [TestFixture]
    public class IxBaseTests
    {
        /// <summary>
        /// Init/dispose cycle with zero configuration and resolves.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task EmptyHostCycle()
        {
            await new IxHostBuilder()
                .Configure(rootNodes => { })
                .Build()
                .AsyncUsing((IIxHost host) => Task.CompletedTask);
        }

        /// <summary>
        /// IIxHost resolve test.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Host_should_be_resolved_by_interface()
        {
            await new IxHostBuilder()
                .Build()
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<IIxHost> hostLock = await host.Resolver.Get<IIxHost>())
                            {
                                hostLock.Target.Should().NotBeNull();
                            }
                        });
        }

        /// <summary>
        /// Dependencies registered directly in some object should be resolved by this object.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Self_registered_should_be_resolved()
        {
            await new IxHostBuilder()
                .Configure(
                    rootNodes =>
                        {
                            rootNodes.Add<Dummy>(
                                factory: new IxClassInstanceBuilderConfig<Dummy>(),
                                nodes:
                                nodes =>
                                    nodes.Add<string>(factory: new IxExistingInstanceFactoryConfig<string>("Test me!")));
                        })
                .Build()
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<Dummy> resolvedInstanceLock = await host.Resolver.Get<Dummy>())
                            {
                                resolvedInstanceLock.Target.Should().NotBeNull();
                            }
                        });
        }

        public class Dummy
        {
            public Dummy(string test)
            {
            }
        }
    }
}