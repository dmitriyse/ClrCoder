// <copyright file="IxManagerTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.ComponentModel.IndirectX
{
    using System.Threading.Tasks;

    using ClrCoder.ComponentModel.IndirectX;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="IxHost"/> class.
    /// </summary>
    [TestFixture]
    public class IxManagerTests
    {
        /// <summary>
        /// </summary>
        [Test]
        public async Task SimpleTest()
        {
            ////await new IxHostBuilder()
            ////    .Configure(
            ////        nodes: x => x
            ////            .Add<IxManagerTests>(
            ////                3,
            ////                nodes: y => y.AddTransient<IxManagerTests>(3))
            ////            .AddTransient<IxManagerTests>(3))
            ////    .Build()
            ////    .AsyncUsing(
            ////        async host =>
            ////            {
            ////                var obj = await host.Get<T>();
            ////            });
        }

        private class DummyType
        {
        }
    }
}