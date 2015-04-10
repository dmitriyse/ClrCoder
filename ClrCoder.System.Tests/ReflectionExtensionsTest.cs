using System.Diagnostics.CodeAnalysis;

using FluentAssertions;

using JetBrains.Annotations;

using NUnit.Framework;

namespace ClrCoder.System.Tests
{
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

        private class DummyClass
        {
            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
                Justification = "Reviewed. Suppression is OK here.")]
            [UsedImplicitly]
            public int TestField;

            public string TestProperty { get; set; }
        }
    }
}