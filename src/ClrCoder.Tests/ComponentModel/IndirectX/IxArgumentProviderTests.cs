// <copyright file="IxArgumentProviderTests.cs" company="ClrCoder project">
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
    /// Tests for the <see cref="IxArgumentProvider"/>.
    /// </summary>
    [TestFixture]
    public class IxArgumentProviderTests
    {
        /// <summary>
        /// Simple argument resolution test.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Argument_should_be_resolved()
        {
            await(await new IxHostBuilder()
                      .Configure(
                          n => n
                              .Add<Dummy>(
                                  instanceBuilder: new IxClassInstanceBuilderConfig<Dummy>(),
                                  multiplicity: new IxPerResolveMultiplicityConfig()))
                      .Build())
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<Dummy> dummyLock =
                                await host.Resolver.Get<Dummy, string>(null, "Hello world!"))
                            {
                                dummyLock.Target.Should().NotBeNull();
                                dummyLock.Target.MyArg.Should().Be("Hello world!");
                            }
                        });
        }

        public class Dummy
        {
            public Dummy(string myArg)
            {
                MyArg = myArg;
            }

            public string MyArg { get; }
        }
    }
}