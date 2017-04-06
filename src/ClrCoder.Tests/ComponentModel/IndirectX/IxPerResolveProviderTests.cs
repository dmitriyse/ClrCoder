// <copyright file="IxPerResolveProviderTests.cs" company="ClrCoder project">
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
    /// Tests for the <see cref="IxPerResolveMultiplicityConfig"/>.
    /// </summary>
    [TestFixture]
    public class IxPerResolveProviderTests
    {
        /// <summary>
        /// Tests that instance is reused.
        /// </summary>
        [Test]
        public async Task Instance_should_be_reused_in_one_resolve()
        {
            await(await new IxHostBuilder()
                      .Configure(
                          n => n
                              .Add<Dummy>(
                                  instanceBuilder: new IxClassInstanceBuilderConfig<Dummy>(),
                                  multiplicity: new IxPerResolveMultiplicityConfig())
                              .Add<DummyUser1>(
                                  instanceBuilder: new IxClassInstanceBuilderConfig<DummyUser1>(),
                                  multiplicity: new IxPerResolveMultiplicityConfig())
                              .Add<DummyUser2>(
                                  instanceBuilder: new IxClassInstanceBuilderConfig<DummyUser2>(),
                                  multiplicity: new IxPerResolveMultiplicityConfig()))
                      .Build())
                .AsyncUsing(
                    async host =>
                        {
                            Dummy dummy;
                            using (IxLock<DummyUser2> dummyLock = await host.Resolver.Get<DummyUser2>())
                            {
                                dummyLock.Target.Should().NotBeNull();
                                dummyLock.Target.DummyUser1.Dummy.Should().BeSameAs(dummyLock.Target.Dummy);
                                dummy = dummyLock.Target.Dummy;
                            }

                            using (IxLock<DummyUser2> dummyLock = await host.Resolver.Get<DummyUser2>())
                            {
                                dummyLock.Target.Dummy.Should().NotBeSameAs(dummy);
                            }
                        });
        }

        private class Dummy
        {
            public override string ToString()
            {
                return $"Dummy{GetHashCode()}";
            }
        }

        private class DummyUser1
        {
            private DummyUser1(Dummy dummy)
            {
                Dummy = dummy;
            }

            public Dummy Dummy { get; }
        }

        private class DummyUser2
        {
            private DummyUser2(Dummy dummy, DummyUser1 dummyUser1)
            {
                Dummy = dummy;
                DummyUser1 = dummyUser1;
            }

            public Dummy Dummy { get; }

            public DummyUser1 DummyUser1 { get; }
        }
    }
}