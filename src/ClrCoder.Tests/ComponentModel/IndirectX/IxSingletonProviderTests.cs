// <copyright file="IxSingletonProviderTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ClrCoder.ComponentModel.IndirectX;
    using ClrCoder.Threading;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// <see cref="IxSingletonProvider"/> related tests.
    /// </summary>
    [TestFixture]
    public class IxSingletonProviderTests
    {
        /// <summary>
        /// Direct registration child should resolve singleton parent.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task ResolveFromInstanceChildTest()
        {
            await (await new IxHostBuilder()
                       .Configure(
                           n => n
                               .Add<Dummy>(
                                   instanceBuilder: new IxClassInstanceBuilderConfig<Dummy>(),
                                   exportToParentFilter:
                                   new IxStdVisibilityFilterConfig
                                       {
                                           WhiteList =
                                               new HashSet<IxIdentifier>
                                                   {
                                                       new IxIdentifier(typeof(Dummy)),
                                                       new IxIdentifier(typeof(DummyChild))
                                                   }
                                       },
                                   nodes:
                                   n1 => n1
                                       .Add<DummyChild>(
                                           instanceBuilder: new IxClassInstanceBuilderConfig<DummyChild>())))
                       .Build())
                .AsyncUsing(
                    async host =>
                        {
                            Dummy dummy;
                            using (IxLock<Dummy> dummyLock = await host.Resolver.Get<Dummy>())
                            {
                                dummyLock.Target.Should().NotBeNull();
                                dummy = dummyLock.Target;
                            }

                            using (IxLock<DummyChild> dummyChildLock = await host.Resolver.Get<DummyChild>())
                            {
                                dummyChildLock.Target.Should().NotBeNull();
                                dummyChildLock.Target.Parent.Should().NotBeNull();
                                dummyChildLock.Target.Parent.Should().BeSameAs(dummy);
                            }
                        });
        }

        private class Dummy
        {
        }

        private class DummyChild
        {
            private DummyChild(Dummy parent)
            {
                Parent = parent;
            }

            public Dummy Parent { get; }
        }
    }
}