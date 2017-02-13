// <copyright file="ProcessableSetTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ClrCoder.Collections;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="ProcessableSet{T}"/> class.
    /// </summary>
    [TestFixture]
    public class ProcessableSetTests
    {
        /// <summary>
        /// Stress monkey test for the <see cref="ProcessableSet{T}"/> class.
        /// </summary>
        [Test]
        public void StressTest()
        {
            var originalList = Enumerable.Range(0, 10000).Select(x => new Dummy()).ToList();
            var rnd = new Random(0);
            var checkHashSet = new HashSet<Dummy>(rnd.RandomUniqueSet(0, originalList.Count, originalList.Count/2).Select(x=>originalList[x]));
            var setToTest = new ProcessableSet<Dummy>();
            foreach (Dummy dummy in checkHashSet)
            {
                setToTest.Add(dummy);
            }

            setToTest.ForEach(
                x =>
                    {
                        if (rnd.NextDouble() < 0.3)
                        {
                            int itemIndex = rnd.Next(0, originalList.Count);
                            var testItem = originalList[itemIndex];
                            bool goodResult;
                            bool testResult;
                            if (rnd.NextBool())
                            {
                                goodResult = checkHashSet.Add(testItem);
                                testResult = setToTest.Add(testItem);
                                testResult.Should().Be(goodResult, "Add operation wrong.");
                            }
                            else
                            {
                                goodResult = checkHashSet.Remove(testItem);
                                testResult = setToTest.Remove(testItem);
                                testResult.Should().Be(goodResult, "Remove operation wrong.");
                                if (testResult)
                                {
                                    //Forgetting that processed, if it was removed.
                                    testItem.ProcessedCount = 0; 
                                }
                            }
                        }

                        x.ProcessedCount++;
                    });

            foreach (Dummy dummy in setToTest)
            {
                dummy.ProcessedCount.Should().Be(1, "Some element processed more than once.");
            }

            setToTest.Count.Should().Be(checkHashSet.Count);
            foreach (Dummy dummy in checkHashSet)
            {
                setToTest.Should().Contain(dummy);
            }
        }

        private class Dummy
        {
            public int ProcessedCount { get; set; }
        }
    }
}