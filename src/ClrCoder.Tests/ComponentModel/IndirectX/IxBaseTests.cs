// <copyright file="IxBaseTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
#pragma warning disable 1998
    using System;
    using System.Threading;
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
        public interface IDummy
        {
            string AboutMe { get; }
        }

        /// <summary>
        /// Tests the case, when dependency overrides parent dependency and uses it.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task DependencyOverrideAndUseTest()
        {
            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes =>
                               rootNodes
                                   .Add<IDummy>(
                                       instanceBuilder: new IxClassInstanceBuilderConfig<DummyParent>())
                                   .Add<SomeContainer>(
                                       instanceBuilder: new IxClassInstanceBuilderConfig<SomeContainer>(),
                                       nodes:
                                       nodes =>
                                           nodes.Add<IDummy>(
                                               instanceBuilder: new IxClassInstanceBuilderConfig<DummyNested>())))
                       .Build())
                .AsyncUsing(
                    async host =>
                        {
                            using (IxLock<SomeContainer> someContainerLock =
                                await host.Resolver.Get<SomeContainer>())
                            {
                                someContainerLock.Target.Dummy.AboutMe.Should().Be("I am parent; I know");
                            }
                        });
        }

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
        /// Tests disposing through IIxSelf.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Self_object_dispose_test()
        {
            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes =>
                               rootNodes
                                   .Add<DummyDisposable>(
                                       instanceBuilder: new IxClassInstanceBuilderConfig<DummyDisposable>(),
                                       multiplicity: new IxPerResolveMultiplicityConfig()))
                       .Build())
                .AsyncUsing(
                    async host =>
                        {
                            var wasCalled = false;
                            DummyDisposable dummyTmp;
                            using (IxLock<DummyDisposable> dummyLock =
                                await host.Resolver.Get<DummyDisposable>())
                            {
                                dummyLock.Target.DisposeAction = () => { wasCalled = true; };
                                dummyLock.Target.DoDispose();
                                dummyTmp = dummyLock.Target;
                                wasCalled.Should().BeFalse();
                            }

                            dummyTmp.Disposed.Wait(TimeSpan.FromSeconds(5));

                            wasCalled.Should().BeTrue();

                            wasCalled = false;
                            using (IxLock<DummyDisposable> dummyLock =
                                await host.Resolver.Get<DummyDisposable>())
                            {
                                dummyLock.Target.DisposeAction = () => { wasCalled = true; };
                                dummyLock.Target.DoDispose();
                                wasCalled.Should().BeFalse();
                            }
                        });
        }

        /// <summary>
        /// Tests disposing through IIxSelf.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task Auto_dispose_test()
        {
            await (await new IxHostBuilder()
                       .Configure(
                           rootNodes =>
                               rootNodes
                                   .Add<DummyDisposable>(
                                       instanceBuilder: new IxClassInstanceBuilderConfig<DummyDisposable>(),
                                       multiplicity: new IxPerResolveMultiplicityConfig(),
                                       autoDisposeEnabled: true))
                       .Build())
                .AsyncUsing(
                    async host =>
                    {
                        var wasCalled = false;
                        DummyDisposable dummyTmp;
                        using (IxLock<DummyDisposable> dummyLock =
                            await host.Resolver.Get<DummyDisposable>())
                        {
                            dummyLock.Target.DisposeAction = () => { wasCalled = true; };
                            dummyTmp = dummyLock.Target;
                        }

                        dummyTmp.Disposed.Wait(TimeSpan.FromSeconds(5));

                        wasCalled.Should().BeTrue();

                        wasCalled = false;
                        using (IxLock<DummyDisposable> dummyLock =
                            await host.Resolver.Get<DummyDisposable>())
                        {
                            dummyLock.Target.DisposeAction = () => { wasCalled = true; };
                            dummyTmp = dummyLock.Target;
                        }

                        dummyTmp.Disposed.Wait(TimeSpan.FromSeconds(5));

                        wasCalled.Should().BeTrue();
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

        public class DummyDisposable : AsyncDisposableBase
        {
            private readonly IIxSelf _self;

            public DummyDisposable(IIxSelf self)
            {
                _self = self;
            }

            public Action DisposeAction { get; set; }

            public void DoDispose()
            {
                _self.DisposeAsync();
            }

            protected override async Task DisposeAsyncCore()
            {
                DisposeAction?.Invoke();
            }
        }

        public class DummyNested : IDummy
        {
            private readonly IDummy _parent;

            public DummyNested(IDummy parent)
            {
                _parent = parent;
            }

            public string AboutMe => _parent.AboutMe + "; I know";
        }

        public class DummyParent : IDummy
        {
            public string AboutMe => "I am parent";
        }

        public class DummyWithError
        {
            public DummyWithError()
            {
                throw new Exception("The Error!");
            }
        }

        public class SomeContainer
        {
            public SomeContainer(IDummy dummy)
            {
                Dummy = dummy;
            }

            public IDummy Dummy { get; }
        }
    }
}