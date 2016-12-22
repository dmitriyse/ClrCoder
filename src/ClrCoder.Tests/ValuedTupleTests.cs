// <copyright file="ValuedTupleTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Tests
{
    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Test for valued tuple API.
    /// </summary>
    [TestFixture]
    public class ValuedTupleTests
    {
        /// <summary>
        /// Tests of behavior with <see langword="null"/> values.
        /// </summary>
        [Test]
        public void NullValuesShouldNotRiseExceptionsTest()
        {
            var st = new ValuedTuple<string>(null);
            st.GetHashCode().Should().Be(0);
            st.Should().NotBe(new ValuedTuple<string>("df"));
        }
    }
}