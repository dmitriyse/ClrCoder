// <copyright file="TextExtensionsTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Text
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    using ClrCoder.Text;

    using Diagnostics;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// <see cref="System.Text"/> <see langword="namespace"/> related tests.
    /// </summary>
    [TestFixture]
    public class TextExtensionsTests
    {
        /// <summary>
        /// Test for the <see cref="TextExtensions.NormalizeLineEndings"/> method.
        /// </summary>
        /// <param name="str">Original string.</param>
        /// <param name="normalizedStr">Normalized string.</param>
        [Test]
        [TestCase("\r\r\na\r\n\r", "\r\n\r\na\r\n\r\n")]
        public void NormalizeLineEndingsTest(string str, string normalizedStr)
        {
            string platformDependentNormalizedStr = normalizedStr.Replace("\r\n", Environment.NewLine);
            str.NormalizeLineEndings().Should().Be(platformDependentNormalizedStr);
        }

        /// <summary>
        /// Test for the <see cref="TextExtensions.NormalizeLineEndings"/>
        /// </summary>
        /// <param name="text">Original text.</param>
        /// <param name="lines">Expected lines in the text.</param>
        [Test]
        [TestCase("")]
        [TestCase("a", "a")]
        [TestCase("\r\n", "")]
        [TestCase("\r\na", "", "a")]
        [TestCase("\r\na\nb", "", "a", "b")]
        [TestCase("\r\na\nb\n", "", "a", "b")]
        public void ReadLinesTests(string text, params string[] lines)
        {
            new StringReader(text).ReadLines().ToArray().Should().BeEquivalentTo(lines);
        }

        /// <summary>
        /// Benchmark test for the <see cref="TextExtensions.NormalizeLineEndings"/> method.
        /// </summary>
        [Test]
        [Category("Benchmark")]
        public void RegexNewlineNormalizerTest()
        {
            CodeTimer.WarmUp();

            var sb = new StringBuilder();
            int length = (1024 * 1024) / sizeof(char);
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

            CodeTimer timer = CodeTimer.Start();
            string normalizedString = oneMbString.NormalizeLineEndings();
            if (normalizedString.Length > 10)
            {
                TestContext.Out.WriteLine("String normalization speed = {0:F2} Mb/s", 1.0 / timer.Time);
            }
        }
    }
}