﻿// <copyright file="IxStdProviderConfigTests.cs" company="ClrCoder project">
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
    /// Tests related to <see cref="IIxStdProviderConfig"/>.
    /// </summary>
    [TestFixture]
    public class IxStdProviderConfigTests
    {
        private interface IDummy
        {
        }

        /// <summary>
        /// Provides config.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task ProvideConfigAttributeTest()
        {
            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes => rootNodes.Add(new DummyConfig()))
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

        private class Dummy : IDummy
        {
            public Dummy(DummyConfig config)
            {
                config.Should().NotBeNull();
            }
        }

        [ProvideConfig]
        private class DummyConfig : IxStdProviderConfig, IIxStdProviderConfig
        {
            IxIdentifier? IIxProviderNodeConfig.Identifier => new IxIdentifier(typeof(Dummy), Name);

            IIxInstanceBuilderConfig IIxStdProviderConfig.Factory
                => new IxClassInstanceBuilderConfig<Dummy>();
        }
    }
}