// <copyright file="BclProductivityExtensionsTest.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Tests
{
    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="BclProductivityExtensions"/>.
    /// </summary>
    [TestFixture]
    public class BclProductivityExtensionsTest
    {
        /// <summary>
        /// Test for the <see cref="BclProductivityExtensions.ToDecimal"/>.
        /// </summary>
        /// <param name="decimalString">String to parse.</param>
        /// <param name="result">Expected parse result.</param>
        [Test]
        [TestCase("0.4", 0.4d)]
        public void ToDecimalTest(string decimalString, decimal? result)
        {
            decimalString.ToDecimal().Should().Be(result);
        }
    }
}