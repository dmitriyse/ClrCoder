// <copyright file="ConvertExTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests
{
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="ConvertEx"/>.
    /// </summary>
    [TestFixture]
    public class ConvertExTests
    {
        /// <summary>
        /// Test for the method <see cref="ConvertEx"/>.
        /// </summary>
        [Test]
        public void ToTraceJsonTest()
        {
            TestContext.WriteLine(ConvertEx.ToTraceJson(new MethodImplAttribute(MethodImplOptions.AggressiveInlining)));
            ConvertEx.ToTraceJson(null).Should().Be("null");
        }
    }
}