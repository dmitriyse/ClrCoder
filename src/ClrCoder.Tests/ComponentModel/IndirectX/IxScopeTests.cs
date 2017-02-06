// <copyright file="IxScopeTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
    using System.Threading.Tasks;

    using ClrCoder.ComponentModel;
    using ClrCoder.ComponentModel.IndirectX;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Scope related tests for IndirectX.
    /// </summary>
    [TestFixture]
    public class IxScopeTests
    {
        /// <summary>
        /// Root scope resolve test.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task RootScopeShouldBeResolved()
        {
            await new IxHostBuilder()
                .Configure()
                .Build()
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<IxScope> rootScope = await host.Resolver.Get<IxScope>())
                            {
                                rootScope.Target.Should().NotBeNull();
                            }
                        });
        }

        /// <summary>
        /// Simplest use case. Single instantiation, no dispose on parent scope dispose.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task SimplestScopeRegistrationShouldWork()
        {
            var instance = new DummyObject();

            await new IxHostBuilder()
                .Configure(
                    rootNodes =>
                        {
                            rootNodes.AddScope(
                                exportToParentFilter: new IxStdVisibilityFilterConfig(),
                                nodes: nodes =>
                                    {
                                        nodes.Add<DummyObject>(
                                            factory: new IxExistingInstanceFactoryConfig<DummyObject>(instance),
                                            disposeHandler: obj => Task.CompletedTask);
                                    });
                        })
                .Build()
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