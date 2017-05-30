// <copyright file="UPathTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests
{
    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class UPathTests
    {
        [TestCase("c:/test/me", "/test/me")]
        [TestCase("/test/me", "/test/me")]
        [TestCase("test/me", "test/me")]
        public void ToUnixPathTest(string source, string expected)
        {
            new UPath(source).ToUnixPath().Should().Be(expected);
        }

        [TestCase("c:/test/me", "C:\\test\\me")]
        [TestCase("/test/me", "C:\\test\\me")]
        [TestCase("test/me", "test\\me")]
        public void ToWindowsPathTest(string source, string expected)
        {
            new UPath(source).ToWindowsPath().Should().Be(expected);
        }
    }
}