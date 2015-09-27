using FluentAssertions;

using NUnit.Framework;

namespace ClrCoder.System.Tests
{
    /// <summary>
    /// Tests for the <see cref="BclProductivityExtensions"/>.
    /// </summary>
    [TestFixture]
    public class BclProductivityExtensionsTest
    {
        /// <summary>
        /// Test for the <see cref="BclProductivityExtensions.ToDecimal"/>.
        /// </summary>
        /// <param name="decimalString">String to parse.</param>
        /// <param name="result">Expected parse result.</param>
        [Test]
        [TestCase("0.4", 0.4)]
        public void ToDecimalTest(string decimalString, double? result)
        {
            decimal? expectedResult = null;
            if (result != null)
            {
                expectedResult = (decimal)result.Value;
            }
            decimalString.ToDecimal().Should().Be(expectedResult);
        }
    }
}