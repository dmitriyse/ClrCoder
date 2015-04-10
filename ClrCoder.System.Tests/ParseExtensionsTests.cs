using FluentAssertions;

using NUnit.Framework;

namespace ClrCoder.System.Tests
{
    /// <summary>
    /// Test for <see cref="ParseExtensions"/> methods.
    /// </summary>
    [TestFixture]
    public class ParseExtensionsTests
    {
        /// <summary>
        /// Test for <see cref="ParseExtensions.ParseAnyDouble"/> method.
        /// </summary>
        /// <param name="doubleString">String to parse.</param>
        /// <param name="doubleValue">Expected parsed value.</param>
        [Test]
        [TestCase("10.3", 10.3)]
        [TestCase("10,3", 10.3)]
        public void ParseAnyDoubleTest(string doubleString, double doubleValue)
        {
            doubleString.ParseAnyDouble().Should().Be(doubleValue);
        }

        /// <summary>
        /// Test for <see cref="ParseExtensions.ParseAnyDouble"/> method.
        /// </summary>
        /// <param name="decimalString">String to parse.</param>
        /// <param name="decimalValue">Expected parsed value.</param>
        [Test]
        [TestCase("10.3", 10.3)]
        [TestCase("10,3", 10.3)]
        public void ParseAnyDecimalTest(string decimalString, double decimalValue)
        {
            decimalString.ParseAnyDecimal().Should().Be((decimal)decimalValue);
        }
    }
}