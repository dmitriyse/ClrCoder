// <copyright file="JsonNetBehaviorTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Json
{
    using System;
    using System.Linq;

    using ClrCoder.Collections;
    using ClrCoder.Json;

    using FluentAssertions;

    using Flurl;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using ObjectModel;

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
            JsonConvert.DeserializeObject<C1>(
                    "{\"prop1\":\"MyTest\"}",
                    JsonDefaults.JsonConfigSerializerSource.Settings)
                .Prop1.Should()
                .Be("MyTest");
        }

        /// <summary>
        /// Tests json serialization behavior.
        /// </summary>
        [Test]
        [Ignore("For manual testing")]
        public void JsonSerializeTest()
        {
            string result = JsonConvert.SerializeObject(
                new Url("http://test.com"),
                JsonDefaults.JsonConfigSerializerSource.Settings);
            TestContext.WriteLine(result);
        }

        [Test]
        [Ignore("Currently IKeyedCollection has no any convertor for Json.Net")]
        public void MaterializedKeyedCollectionTest()
        {
            var container = new MyContainer
                                {
                                    SomeItems =
                                        {
                                            new MyKeyed
                                                {
                                                    Key = 1,
                                                    Tst = "TestMe1"
                                                },
                                            new MyKeyed
                                                {
                                                    Key = 2,
                                                    Tst = "TestMe2"
                                                }
                                        }
                                };
            string str = JsonConvert.SerializeObject(container, Formatting.Indented);
            TestContext.WriteLine(str);
            var myObj = JsonConvert.DeserializeObject<MyContainer>(str);
            myObj.SomeItems.Count.Should().Be(container.SomeItems.Count);
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
            JsonConvert.DeserializeObject<C2>(
                    "{\"items\":[\"MyTest\"]}",
                    JsonDefaults.JsonConfigSerializerSource.Settings)
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

        private class MyContainer
        {
            public IKeyedCollection<int, MyKeyed> SomeItems { get; } = new KeyedCollectionEx<int, MyKeyed>();
        }

        private class MyKeyed : IKeyed<int>
        {
            public int Key { get; set; }

            public string Tst { get; set; }
        }
    }
}