// <copyright file="JsonNetBehaviorTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Tests
{
    using System;
    using System.Linq;

    using ClrCoder.Collections;

    using FluentAssertions;

    using Newtonsoft.Json;

    using NUnit.Framework;

    /// <summary>
    /// Tests for check behavior of Newtonsoft.Json.
    /// </summary>
    [TestFixture]
    public class JsonNetBehaviorTests
    {
        /// <summary>
        /// Checks that properties can be deserialized from the constructor.
        /// </summary>
        [Test]
        public void ConstructorInitializationTest()
        {
            JsonConvert.DeserializeObject<C1>("{\"prop1\":\"MyTest\"}", JsonConfig.SerializerSettings)
                .Prop1.Should()
                .Be("MyTest");
        }

        /// <summary>
        /// Ensures that private ShouldSerialize* methods are recognized by Json.Net.
        /// </summary>
        [Test]
        public void PrivateShouldSerializeTest()
        {
            JsonConvert.SerializeObject(new C3 { IgnoreMe = "NotIgnored" }).Should().NotContain("NotIgnored");
        }

        /// <summary>
        /// Checks SafeList deserialization.
        /// </summary>
        [Test]
        public void SafeListDeserializationTest()
        {
            JsonConvert.DeserializeObject<C2>("{\"items\":[\"MyTest\"]}", JsonConfig.SerializerSettings)
                .Items.First()
                .Should()
                .Be("MyTest");
        }

        /// <summary>
        /// Checks <see cref="Uri"/> serialization/deserialization test.
        /// </summary>
        [Test]
        public void UriSerializationDeserializationTest()
        {
            // Uri is a buggy beast.
            new Uri("http://test.com").AbsoluteUri.Should().Be("http://test.com/");

            JsonConvert.DeserializeObject<C4>(
                    JsonConvert.SerializeObject(new C4 { UriProp = new Uri("http://test.com") }))
                .UriProp.AbsoluteUri.Should()
                .Be("http://test.com/");
        }

        private class C1
        {
            public C1(string prop1)
            {
                Prop1 = prop1;
            }

            public string Prop1 { get; set; }
        }

        private class C2
        {
            public SafeList<string> Items { get; set; }
        }

        private class C3
        {
            public string IgnoreMe { get; set; }

            /// <summary>
            /// Only public method supported.
            /// </summary>
            /// <returns>Always false.</returns>
            public bool ShouldSerializeIgnoreMe()
            {
                return false;
            }
        }

        private class C4
        {
            public Uri UriProp { get; set; }
        }
    }
}