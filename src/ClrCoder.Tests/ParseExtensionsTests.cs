// <copyright file="ParseExtensionsTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Tests
{
    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Test for <see cref="ParseExtensions"/> methods.
    /// </summary>
    [TestFixture]
    public class ParseExtensionsTests
    {
        /// <summary>
        /// Test for <see cref="ParseExtensions.ParseAnyDouble"/> method.
        /// </summary>
        /// <param name="decimalString">String to parse.</param>
        /// <param name="decimalValue">Expected parsed value.</param>
        [Test]
        [TestCase("10.3", 10.3)]
        [TestCase("10,3", 10.3)]
        public void ParseAnyDecimalTest(string decimalString, double decimalValue)
        {
            decimalString.ParseAnyDecimal().Should().Be((decimal)decimalValue);
        }

        /// <summary>
        /// Test for <see cref="ParseExtensions.ParseAnyDouble"/> method.
        /// </summary>
        /// <param name="doubleString">String to parse.</param>
        /// <param name="doubleValue">Expected parsed value.</param>
        [Test]
        [TestCase("10.3", 10.3)]
        [TestCase("10,3", 10.3)]
        public void ParseAnyDoubleTest(string doubleString, double doubleValue)
        {
            doubleString.ParseAnyDouble().Should().Be(doubleValue);
        }
    }
}