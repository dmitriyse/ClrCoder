using System;
using System.Threading;

using ClrCoder.System.Collections;

using FluentAssertions;

using NUnit.Framework;

namespace ClrCoder.System.Tests.Collections
{
    /// <summary>
    /// Tests for the <see cref="TtlDictionary{TKey,TValue}"/>
    /// </summary>
    [TestFixture]
    public class TtlDictionaryTest
    {
        /// <summary>
        /// Item remove test after ttl elapsed.
        /// </summary>
        [Test]
        public void ItemShouldBeRemovedAfterTtl()
        {
            var d = new TtlDictionary<int, string>();
            d.Set(10, "Hello world!", new TimeSpan(0, 0, 2));
            Thread.Sleep(1000);
            d.Should().ContainKey(10);
            Thread.Sleep(2000);
            d.Count.Should().Be(0);
        }
    }
}