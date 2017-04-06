// <copyright file="IxBaseTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
    using System;
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
            await (await new IxHostBuilder()
                       .Build())
                .AsyncUsing(host => Task.CompletedTask);
        }

        /// <summary>
        /// Exception from constructor should rise to resolve consumer.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Exception_from_constructor_should_rise_up_to_resolve()
        {
            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes =>
                               rootNodes
                                   .Add<DummyWithError>(
                                       instanceBuilder: new IxClassInstanceBuilderConfig<DummyWithError>()))
                       .Build())
                .AsyncUsing(
                    async host =>
                        {
                            try
                            {
                                using (IxLock<DummyWithError> resolvedInstanceLock =
                                    await host.Resolver.Get<DummyWithError>())
                                {
                                    resolvedInstanceLock.Target.Should().NotBeNull();
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.GetType().Should().Be<Exception>();
                                ex.Message.Should().Be("The Error!");
                            }
                        });
        }

        /// <summary>
        /// IIxHost resolve test.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Host_should_be_resolved_by_interface()
        {
            await (await new IxHostBuilder()
                       .Build())
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
            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes =>
                               rootNodes
                                   .Add<Dummy>(
                                       instanceBuilder: new IxClassInstanceBuilderConfig<Dummy>(),
                                       nodes:
                                       nodes =>
                                           nodes.Add<string>(
                                               instanceBuilder: new IxExistingInstanceFactoryConfig<string>(
                                                   "Test me!")))
                       )
                       .Build())
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

        public class DummyWithError
        {
            public DummyWithError()
            {
                throw new Exception("The Error!");
            }
        }
    }
}