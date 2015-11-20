using FluentAssertions;

using NUnit.Framework;

namespace ClrCoder.System.Tests
{
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