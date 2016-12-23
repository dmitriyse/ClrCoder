// <copyright file="ReflectionExtensionsTest.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using FluentAssertions;

    using JetBrains.Annotations;

    using NUnit.Framework;

    /// <summary>
    /// Test for the <see cref="ReflectionExtensions"/> methods.
    /// </summary>
    [TestFixture]
    public class ReflectionExtensionsTest
    {
        /// <summary>
        /// Tests for get set members by reflection.
        /// </summary>
        [Test]
        public void GetSetMemberValueTest()
        {
            var dc = new DummyClass();
            dc.SetMemberValue("TestField", 12323);
            dc.SetMemberValue("TestProperty", "MyValue");

            dc.GetMember("TestField").Value<int>().Should().Be(12323);
            dc.GetMember("TestProperty").Value<string>().Should().Be("MyValue");
        }

        /// <summary>
        /// Tests for getting type member name.
        /// </summary>
        [Test]
        public void Type_member_name_should_be_received()
        {
            default(DummyClass).NameOf(_ => _.TestField).Should().Be("TestField");
            default(DummyClass).NameOf(_ => _.TestProperty).Should().Be("TestProperty");
        }

        /// <summary>
        /// Type member name should be enumerated.
        /// </summary>
        [Test]
        public void Type_members_accessiblity_should_be_determined()
        {
            var dc = new DummyClass();
            dc.GetMember("TestField").IsPublic.Should().BeTrue();
            dc.GetMember("TestProperty").IsPublic.Should().BeTrue();
            dc.GetMember("_privateField").IsPublic.Should().BeFalse();
            dc.GetMember("PrivateProperty").IsPublic.Should().BeFalse();
        }

        /// <summary>
        /// Type member name should be enumerated.
        /// </summary>
        [Test]
        public void Type_members_should_be_enumerated()
        {
            var dc = new DummyClass();
            dc.GetMembers()
                .Select(x => x.Name)
                .ShouldBeEquivalentTo(new[] { "TestField", "TestProperty", "_privateField", "PrivateProperty" });
        }

#pragma warning disable CS0649, CS0169
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Required for reflection tests.")]
        private class DummyClass
        {
            [UsedImplicitly]
            public static string StaticField;

            [UsedImplicitly]
            private static double PrivateStaticField;

            [UsedImplicitly]
            public int TestField;

            [UsedImplicitly]
            private int _privateField;

            [UsedImplicitly]
            public static string StaticProperty { get; private set; }

            [UsedImplicitly]
            private static double PrivateStaticProperty { get; set; }

            [UsedImplicitly]
            public string TestProperty { get; set; }

            [UsedImplicitly]
            protected string PrivateProperty { get; private set; }
        }
#pragma warning restore CS0649, CS0169
    }
}