using System;
using System.Text;

using ClrCoder.System.Diagnostics;
using ClrCoder.System.Text;

using FluentAssertions;

using NUnit.Framework;

namespace ClrCoder.System.Tests.Text
{
    /// <summary>
    /// <see cref="ClrCoder.System.Text"/> <see langword="namespace"/> related tests.
    /// </summary>
    [TestFixture]
    public class TextTests
    {
        /// <summary>
        /// Benchmark test for the <see cref="TextExtensions.NormalizeLineEndings"/> method.
        /// </summary>
        [Test]
        [Category("Benchmark")]
        public void RegexNewlineNormalizerTest()
        {
            CodeTimer.WarmUp();

            var sb = new StringBuilder();
            int length = 1024 * 1024 / sizeof(char);
            string[] words =
                {
                    "Dummy", "\r", "\n", "\r\n", " ", "hello", "world", "this", "is", "text", "!",
                    "benchmark", "for", "text"
                };
            var rnd = new Random(0);

            while (sb.Length < length)
            {
                sb.Append(rnd.From(words));
            }

            string oneMbString = sb.ToString();

            CodeTimer timer = CodeTimer.Start(3);
            string normalizedString = oneMbString.NormalizeLineEndings();
            if (normalizedString.Length > 10)
            {
                Console.WriteLine("String normalization speed = {0:F2} Mb/s", 1.0 / timer.Time);
            }
        }

        /// <summary>
        /// Test for the <see cref="TextExtensions.NormalizeLineEndings"/> method.
        /// </summary>
        /// <param name="str">Original string.</param>
        /// <param name="normalizedStr">Normalized string.</param>
        [Test]
        [TestCase("\r\r\na\r\n\r", "\r\n\r\na\r\n\r\n")]
        public void NormalizeLineEndingsTest(string str, string normalizedStr)
        {
            str.NormalizeLineEndings().Should().Be(normalizedStr);
        }
    }
}