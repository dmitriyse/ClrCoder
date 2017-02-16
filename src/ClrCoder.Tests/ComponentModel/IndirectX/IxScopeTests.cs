// <copyright file="IxScopeTests.cs" company="ClrCoder project">
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
    /// Scope related tests for IndirectX.
    /// </summary>
    [TestFixture]
    public class IxScopeTests
    {
        /// <summary>
        /// Registered scope should be resolved.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Registered_scope_should_be_resolved_with_the_same_instance()
        {
            await new IxHostBuilder()
                .Configure(n => n.AddScope("test"))
                .Build()
                .AsyncUsing(
                    async host =>
                        {
                            object firstResolvedInstance;
                            using (IxLock<IxScope> scope = await host.Resolver.Get<IxScope>("test"))
                            {
                                scope.Target.Should().NotBeNull();
                                firstResolvedInstance = scope.Target;
                            }

                            using (IxLock<IxScope> scope = await host.Resolver.Get<IxScope>("test"))
                            {
                                scope.Target.Should().NotBeNull();
                                scope.Target.Should().Be(firstResolvedInstance);
                            }
                        });
        }

        /// <summary>
        /// Registered scope should be resolved.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task RegisteredScopeShouldBeResolved()
        {
            await new IxHostBuilder()
                .Configure(n => n.AddScope("test"))
                .Build()
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<IxScope> rootScope = await host.Resolver.Get<IxScope>("test"))
                            {
                                rootScope.Target.Should().NotBeNull();
                            }
                        });
        }

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
        public async Task Scope_registration_should_follow_visibility_rule()
        {
            var instance = new DummyObject();

            await new IxHostBuilder()
                .Configure(
                    rootNodes =>
                        rootNodes.AddScope(
                            "private",
                            nodes: nodes =>
                                {
                                    nodes.Add<DummyObject>(
                                        factory: new IxExistingInstanceFactoryConfig<DummyObject>(instance),
                                        disposeHandler: obj => Task.CompletedTask);
                                }))
                .Build()
                .AsyncUsing(
                    host =>
                        {
                            try
                            {
                                Func<Task> action = async () =>
                                    {
                                        using (
                                            IxLock<DummyObject> resolvedInstanceLock =
                                                await host.Resolver.Get<DummyObject>())
                                        {
                                            resolvedInstanceLock.Target.Should().Be(instance);
                                        }
                                    };

                                action.ShouldThrow<IxResolveTargetNotFound>()
                                    .Which.Identifier.Should().Be(new IxIdentifier(typeof(DummyObject)));
                            }
                            catch (Exception ex)
                            {
                                Task.FromException(ex);
                            }

                            return Task.CompletedTask;
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
                        rootNodes.AddScope(
                            exportToParentFilter: new IxStdVisibilityFilterConfig(),
                            nodes: nodes =>
                                {
                                    nodes.Add<DummyObject>(
                                        factory: new IxExistingInstanceFactoryConfig<DummyObject>(instance),
                                        disposeHandler: obj => Task.CompletedTask);
                                }))
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