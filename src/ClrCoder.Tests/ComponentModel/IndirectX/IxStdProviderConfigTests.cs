// <copyright file="IxStdProviderConfigTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using ClrCoder.ComponentModel.IndirectX;
    using ClrCoder.Threading;

    using FluentAssertions;

    using JetBrains.Annotations;

    using NUnit.Framework;

    /// <summary>
    /// Tests related to <see cref="IIxStdProviderConfig"/>.
    /// </summary>
    [TestFixture]
    public class IxStdProviderConfigTests
    {
        /// <summary>
        /// Provides config.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task ProvideConfigAttributeTest()
        {
            await new IxHostBuilder()
                .Configure(
                    rootNodes =>
                    {
                        rootNodes.Add(new DummyConfig());
                    })
                .Build()
                .AsyncUsing(
                    async host =>
                    {
                        using (IxLock<Dummy> resolvedInstanceLock = await host.Resolver.Get<Dummy>())
                        {
                            resolvedInstanceLock.Target.Should().NotBeNull();
                        }
                    });
        }

        private interface IDummy
        {
            
        }

        private class Dummy:IDummy
        {
            public Dummy(DummyConfig config)
            {
                config.Should().NotBeNull();
            }
        }

        [ProvideConfig]
        private class DummyConfig : IIxStdProviderConfig
        {
            IxIdentifier IIxProviderNodeConfig.Identifier => new IxIdentifier(typeof(Dummy), Name);

            IIxVisibilityFilterConfig IIxProviderNodeConfig.ImportFilter => null;

            IIxVisibilityFilterConfig IIxProviderNodeConfig.ExportToParentFilter => null;

            IIxVisibilityFilterConfig IIxProviderNodeConfig.ExportFilter => null;

            [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine",
                Justification = "Reviewed. Suppression is OK here.")]
            ICollection<IIxProviderNodeConfig> IIxProviderNodeConfig.Nodes => new IIxProviderNodeConfig[] { };

            IIxScopeBindingConfig IIxStdProviderConfig.ScopeBinding => null;

            IIxMultiplicityConfig IIxStdProviderConfig.Multiplicity => null;

            IIxInstanceBuilderConfig IIxStdProviderConfig.Factory
                => new IxClassInstanceBuilderConfig<Dummy>();

            IxDisposeHandlerDelegate IIxStdProviderConfig.DisposeHandler => null;

            [NotNull]
            public string Name { get; set; }
        }
    }
}