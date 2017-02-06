// <copyright file="IxHostTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
    using System.Threading.Tasks;

    using ClrCoder.ComponentModel;
    using ClrCoder.ComponentModel.IndirectX;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="IxHost"/> class.
    /// </summary>
    [TestFixture]
    public class IxHostTests
    {
        /// <summary>
        /// Init/dispose cycle with zero configuration and resolves.
        /// </summary>
        /// <return>Async execution TPL task.</return>
        [Test]
        public async Task EmptyHostCycle()
        {
            await new IxHostBuilder()
                .Configure(rootNodes => { })
                .Build()
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                .AsyncUsing(async host => { });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        private class DummyType
        {
        }
    }
}