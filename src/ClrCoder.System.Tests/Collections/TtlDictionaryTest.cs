// <copyright file="TtlDictionaryTest.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.System.Tests.Collections
{
    using FluentAssertions;

    using global::System;
    using global::System.Threading;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="System.Collections.TtlDictionary{TKey,TValue}"/>
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
            var d = new System.Collections.TtlDictionary<int, string>();
            d.Set(10, "Hello world!", new TimeSpan(0, 0, 2));
            Thread.Sleep(1000);
            d.Should().ContainKey(10);
            Thread.Sleep(2000);
            d.Count.Should().Be(0);
        }
    }
}