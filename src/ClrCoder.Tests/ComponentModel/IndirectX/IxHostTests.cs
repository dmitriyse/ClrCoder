// <copyright file="IxHostTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
    using System.Threading.Tasks;

    using ClrCoder.ComponentModel.IndirectX;
    using ClrCoder.Threading;

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
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task EmptyHostCycle()
        {
            await new IxHostBuilder()
                .Configure(rootNodes => { })
                .Build()
                .AsyncUsing(host => Task.CompletedTask);
        }
    }
}