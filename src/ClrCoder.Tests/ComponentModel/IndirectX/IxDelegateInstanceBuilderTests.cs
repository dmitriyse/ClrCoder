// <copyright file="IxDelegateInstanceBuilderTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using ClrCoder.ComponentModel.IndirectX;
    using ClrCoder.Threading;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="IxDelegateInstanceBuilderConfig"/> instance builder.
    /// </summary>
    [TestFixture]
    public class IxDelegateInstanceBuilderTests
    {
        /// <summary>
        /// Simple test.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task ExceptionInDelegateTest()
        {
            Task testTask = (await new IxHostBuilder()
                                 .Configure(
                                     rootNodes =>
                                         rootNodes
                                             .Add<Dummy>(
                                                 instanceBuilder:
                                                 new IxClassInstanceBuilderConfig<Dummy>())
                                             .Add<AnotherDummy>(
                                                 instanceBuilder:
                                                 IxDelegateInstanceBuilderConfig.New<Dummy, AnotherDummy>(
                                                     async dummy =>
                                                         {
                                                             throw new InvalidOperationException("My Error");
                                                         })))
                                 .Build())
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<AnotherDummy> anotherDummyLock =
                                await host.Resolver.Get<AnotherDummy>())
                            {
                                anotherDummyLock.Target.Dummy.Should().NotBeNull();
                            }
                        });

            ((Func<Task>)(async () => await testTask)).ShouldThrow<InvalidOperationException>().WithMessage("My Error");
        }

        /// <summary>
        /// Simple test.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task SimpleTest()
        {
            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes =>
                               rootNodes
                                   .Add<Dummy>(
                                       instanceBuilder:
                                       new IxClassInstanceBuilderConfig<Dummy>())
                                   .Add<AnotherDummy>(
                                       instanceBuilder:
                                       IxDelegateInstanceBuilderConfig.New<Dummy, AnotherDummy>(
                                           async dummy => new AnotherDummy(dummy))))
                       .Build())
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<AnotherDummy> anotherDummyLock =
                                await host.Resolver.Get<AnotherDummy>())
                            {
                                anotherDummyLock.Target.Dummy.Should().NotBeNull();
                            }
                        });
        }

        private class AnotherDummy
        {
            public AnotherDummy(Dummy dummy)
            {
                Dummy = dummy;
            }

            public Dummy Dummy { get; }
        }

        private class Dummy
        {
        }
    }
}