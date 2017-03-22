// <copyright file="IxScopeTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    using System;
    using System.Collections.Generic;
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
        private interface IDummyConfigUser
        {
        }

        /// <summary>
        /// Derived scope should be resolved.
        /// </summary>
        /// <returns>Async execution TPL Task.</returns>
        [Test]
        public async Task DerivedScopeTest()
        {
            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes =>
                               rootNodes.Add(new DummyScopeConfig()))
                       .Build())
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<IDummyConfigUser> dummy = await host.Resolver.Get<IDummyConfigUser>())
                            {
                                dummy.Target.Should().NotBeNull();
                            }
                        });
        }

        /// <summary>
        /// Registered scope should be resolved.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Registered_scope_should_be_resolved_with_the_same_instance()
        {
            await (await new IxHostBuilder()
                       .Configure(n => n.AddScope("test"))
                       .Build())
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
            await (await new IxHostBuilder()
                       .Configure(n => n.AddScope("test"))
                       .Build())
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
            await (await new IxHostBuilder()
                       .Configure()
                       .Build())
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

            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes =>
                               rootNodes.AddScope(
                                   "private",
                                   nodes: nodes =>
                                       {
                                           nodes.Add<DummyObject>(
                                               instanceBuilder:
                                               new IxExistingInstanceFactoryConfig<DummyObject>(instance),
                                               disposeHandler: obj => Task.CompletedTask);
                                       }))
                       .Build())
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

            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes =>
                               rootNodes.AddScope(
                                   exportToParentFilter: new IxStdVisibilityFilterConfig(),
                                   nodes: nodes =>
                                       {
                                           nodes.Add<DummyObject>(
                                               instanceBuilder:
                                               new IxExistingInstanceFactoryConfig<DummyObject>(instance),
                                               disposeHandler: obj => Task.CompletedTask);
                                       }))
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

        private class DummyConfigUser : IDummyConfigUser
        {
            public DummyConfigUser(DummyScopeConfig config)
            {
                Config = config;
            }

            public DummyScopeConfig Config { get; }
        }

        private class DummyObject
        {
        }

        private class DummyScopeConfig : IxScopeConfig, IIxProviderNodeConfig
        {
            ICollection<IIxProviderNodeConfig> IIxProviderNodeConfig.Nodes =>
                new List<IIxProviderNodeConfig>
                    {
                        new IxStdProviderConfig
                            {
                                Identifier = new IxIdentifier(typeof(DummyConfigUser)),
                                InstanceBuilder = new IxClassInstanceBuilderConfig<DummyConfigUser>()
                            },
                        new IxStdProviderConfig
                            {
                                Identifier = new IxIdentifier(typeof(IDummyConfigUser)),
                                InstanceBuilder =
                                    IxDelegateInstanceBuilderConfig.New(
                                        async (DummyConfigUser user) => (IDummyConfigUser)user)
                            },
                        new IxStdProviderConfig
                            {
                                Identifier = new IxIdentifier(typeof(DummyScopeConfig)),
                                InstanceBuilder = new IxExistingInstanceFactoryConfig<DummyScopeConfig>(this)
                            }
                    };

            IIxVisibilityFilterConfig IIxProviderNodeConfig.ExportToParentFilter =>
                new IxStdVisibilityFilterConfig
                    {
                        WhiteList = new HashSetEx<IxIdentifier>
                                        {
                                            new IxIdentifier(typeof(IDummyConfigUser)),
                                        }
                    };
        }
    }
}