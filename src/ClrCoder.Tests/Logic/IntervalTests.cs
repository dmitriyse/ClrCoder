// <copyright file="IntervalTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Logic
{
    using System;
    using System.Linq;
    using System.Text;

    using ClrCoder.Logic;

    using FluentAssertions;

    using Newtonsoft.Json;

    using NUnit.Framework;

    /// <summary>
    /// <see cref="Interval{T}"/> class tests.
    /// </summary>
    [TestFixture]
    public class IntervalTests
    {
        /// <summary>
        /// Interval serialization test.
        /// </summary>
        [Test]
        public void SerializationTest()
        {
            var original =
                new Interval<double>(Math.PI, 10);
            var serialized = JsonConvert.SerializeObject(original, JsonDefaults.JsonConfigSerializerSettings);
            TestContext.WriteLine(serialized);

            var deserialized = JsonConvert.DeserializeObject<Interval<double>>(serialized);
            deserialized.Should().Be(original);
        }

        /// <summary>
        /// <see cref="MultiInterval{T}.Union"/> method test.
        /// </summary>
        /// <param name="originalStateString">MultiInterval original state.</param>
        /// <param name="intervalString">An interval to union.</param>
        /// <param name="expectedNewState">Expected state after union.</param>
        [Test]
        [TestCase("", "3-5", "3-5")]
        [TestCase("2-", "3-5", "2-")]
        [TestCase("2-5;8-10;19-40", "-", "-")]
        [TestCase("2-5;8-10;19-40", "5-11", "2-11;19-40")]
        [TestCase("2-5;8-10;19-40", "1-3", "1-5;8-10;19-40")]
        [TestCase("2-5;8-10;19-40", "20-30", "2-5;8-10;19-40")]
        [TestCase("2-5;8-10;19-40", "2-19", "2-40")]
        [TestCase("2-5;8-10;19-40", "-9", "-10;19-40")]
        [TestCase("2-5;8-10;19-40", "-8", "-10;19-40")]
        [TestCase("2-5;8-10;19-40", "-7", "-7;8-10;19-40")]
        [TestCase("2-5;8-10;19-40", "6-6", "2-5;8-10;19-40")]
        [TestCase("2-5;8-10;19-40", "7-51", "2-5;7-51")]
        [TestCase("2-5;8-10;19-40", "7-", "2-5;7-")]
        [TestCase("2-5;8-10;19-40", "9-", "2-5;8-")]
        [TestCase("2-5;8-10;19-40", "14-18", "2-5;8-10;14-18;19-40")]
        [TestCase("2-5;8-10;19-40", "7-11", "2-5;7-11;19-40")]
        [TestCase("2-5;8-10;19-40", "12-14", "2-5;8-10;12-14;19-40")]
        [TestCase("2-5;8-10;19-40", "12-19", "2-5;8-10;12-40")]
        [TestCase("2-5;8-10;19-40", "12-21", "2-5;8-10;12-40")]
        [TestCase("2-5;8-10;19-40", "12-50", "2-5;8-10;12-50")]
        [TestCase("2-5;8-10;19-40", "4-8", "2-10;19-40")]
        [TestCase("2-5;8-10;19-40", "4-9", "2-10;19-40")]
        [TestCase("2-5;8-10;19-40", "4-11", "2-11;19-40")]
        [TestCase("2-", "-1", "-1;2-")]
        [TestCase("1-", "-1", "-")]
        [TestCase("3-", "-6", "-")]
        [TestCase("3-", "-", "-")]
        [TestCase("-8", "-", "-")]
        [TestCase("-", "-", "-")]
        [TestCase("", "-", "-")]
        [TestCase("", "-6", "-6")]
        [TestCase("", "6-", "6-")]
        public void UnionTest(string originalStateString, string intervalString, string expectedNewState)
        {
            var multiInterval = ParseMultiInterval(originalStateString);
            var interval = ParseInterval(intervalString);
            multiInterval.Union(interval);
            MultiIntervalToString(multiInterval).Should().Be(expectedNewState);
        }

        private string MultiIntervalToString(MultiInterval<int> multiInterval)
        {
            StringBuilder sb = null;
            foreach (var interval in multiInterval)
            {
                if (sb == null)
                {
                    sb = new StringBuilder();
                }
                else
                {
                    sb.Append(';');
                }

                sb.AppendFormat("{0}-{1}", interval.Start, interval.EndExclusive);
            }

            return sb == null ? string.Empty : sb.ToString();
        }

        private int? ParseInt(string boundString)
        {
            if (string.IsNullOrWhiteSpace(boundString))
            {
                return null;
            }

            return int.Parse(boundString);
        }

        private Interval<int> ParseInterval(string intervalString)
        {
            var startEnd = intervalString.Split('-');
            return new Interval<int>(ParseInt(startEnd[0]), ParseInt(startEnd[1]));
        }

        private MultiInterval<int> ParseMultiInterval(string multiIntervalString)
        {
            return
                new MultiInterval<int>(
                    multiIntervalString.Split(';').Where(x => !string.IsNullOrEmpty(x)).Select(ParseInterval));
        }
    }
}