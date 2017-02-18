// <copyright file="IxClassRawInstanceFactoryTests.cs" company="ClrCoder project">
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

    using JetBrains.Annotations;

    using NUnit.Framework;

    /// <summary>
    /// Tests related to <see cref="IxClassInstanceBuilderConfig{T}"/>
    /// </summary>
    [NoReorder]
    [TestFixture]
    public class IxClassRawInstanceFactoryTests
    {
        /// <summary>
        /// Simplest dependency resolve test.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Simplest_dependency_should_be_resolved()
        {
            await (await new IxHostBuilder()
                .Configure(
                    rootNodes =>
                        rootNodes
                            .Add<SimplestDummy>(
                                factory: new IxClassInstanceBuilderConfig<SimplestDummy>())
                            .Add<WithSimplestDependencyDummy>(
                                factory: new IxClassInstanceBuilderConfig<WithSimplestDependencyDummy>()))
                .Build())
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<SimplestDummy> dummyLock =
                                await host.Resolver.Get<SimplestDummy>())
                            using (IxLock<WithSimplestDependencyDummy> dependentOnDummyLock =
                                await host.Resolver.Get<WithSimplestDependencyDummy>())
                            {
                                dependentOnDummyLock.Target.Dummy.Should().Be(dummyLock.Target);
                            }
                        });
        }

        /// <summary>
        /// Private constructor should be acceptable.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Class_with_private_constructor_should_be_instantiated()
        {
            await (await new IxHostBuilder()
                .Configure(
                    rootNodes =>
                        rootNodes
                            .Add<PrivateConstructorDummy>(
                                factory: new IxClassInstanceBuilderConfig<PrivateConstructorDummy>(),
                                disposeHandler: obj => Task.CompletedTask))
                .Build())
                .AsyncUsing(
                    async host =>
                        {
                            using (
                                IxLock<PrivateConstructorDummy> resolvedInstanceLock =
                                    await host.Resolver.Get<PrivateConstructorDummy>()
                            )
                            {
                                resolvedInstanceLock.Target.Should().BeOfType<PrivateConstructorDummy>();
                            }
                        });
        }

        /// <summary>
        /// Duplicates across constructor arguments error raise test.
        /// </summary>
        [Test]
        public void Duplicate_constructors_arguments_error_should_be_raised()
        {
            Func<Task> action = async () =>
                {
                    await new IxHostBuilder()
                        .Configure(
                            n =>
                                n.Add<DuplicateArgumentConstructorDummy>(
                                    factory:
                                    new IxClassInstanceBuilderConfig<DuplicateArgumentConstructorDummy>()))
                        .Build();
                };

            action.ShouldThrow<IxConfigurationException>().Which.Message.Should().Contain("Multiple");
        }

        /// <summary>
        /// Multipile constructors error should be raised.
        /// </summary>
        [Test]
        public void Multiple_constructors_error_should_be_raised()
        {
            Func<Task> action = async () =>
                {
                    await new IxHostBuilder()
                        .Configure(
                            n =>
                                n.Add<MultiConstructorsDummy>(
                                    factory:
                                    new IxClassInstanceBuilderConfig<MultiConstructorsDummy>()))
                        .
                        Build();
                };

            action.ShouldThrow<IxConfigurationException>().Which.Message.Should().Contain("more than one");
        }

        /// <summary>
        /// Most simple class without dependencies should be instantiated.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Simplest_class_should_be_instantiated()
        {
            await (await new IxHostBuilder()
                .Configure(
                    rootNodes =>
                        rootNodes.Add<SimplestDummy>(
                            factory: new IxClassInstanceBuilderConfig<SimplestDummy>(),
                            disposeHandler: obj => Task.CompletedTask))
                .Build())
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<SimplestDummy> resolvedInstanceLock = await host.Resolver.Get<SimplestDummy>()
                            )
                            {
                                resolvedInstanceLock.Target.Should().BeOfType<SimplestDummy>();
                            }
                        });
        }

        private class PrivateConstructorDummy
        {
            private PrivateConstructorDummy()
            {
            }
        }

        private class SimplestDummy
        {
        }

        private class DuplicateArgumentConstructorDummy
        {
            [UsedImplicitly]
            public DuplicateArgumentConstructorDummy(string a, string b)
            {
            }
        }

        private class WithSimplestDependencyDummy
        {
            [UsedImplicitly]
            public WithSimplestDependencyDummy(SimplestDummy dummy)
            {
                Dummy = dummy;
            }

            public SimplestDummy Dummy { get; }
        }

        private class MultiConstructorsDummy
        {
            [UsedImplicitly]
            public MultiConstructorsDummy()
            {
            }

            [UsedImplicitly]
            public MultiConstructorsDummy(int abc)
            {
            }
        }
    }
}