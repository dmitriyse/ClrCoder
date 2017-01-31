// <copyright file="BclProductivityExtensionsTest.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="BclProductivityExtensions"/>.
    /// </summary>
    [TestFixture]
    public class BclProductivityExtensionsTest
    {
        /// <summary>
        /// Test for the <see cref="BclProductivityExtensions.RandomUniqueSet"/> method.
        /// </summary>
        /// <param name="minValue">Test set size.</param>
        /// <param name="maxValue">Test subset size.</param>
        /// <param name="size"></param> 
        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(0, 1, 1)]
        [TestCase(0, 17, 5)]
        [TestCase(19999, 20000, 1)]
        [TestCase(1, 20000, 19999)]
        [TestCase(3, 17, 0)]
        [TestCase(-100, -50, 30)]
        [TestCase(50, 100, 30)]
        [TestCase(7, 10, 2)]
        public void RandomSubset_chekSeveralSubsetInSets_returnCorectRandomSubset(int minValue, int maxValue, int size)
        {
            var rnd = new Random();
            List<int> randomSubset = rnd.RandomUniqueSet(minValue, maxValue, size);

            foreach (int v in randomSubset)
            {
                v.Should()
                    .BeGreaterOrEqualTo(minValue).And.BeLessThan(maxValue);
            }

            randomSubset.Should()
                .HaveCount(size)
                .And.OnlyHaveUniqueItems();
        }

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