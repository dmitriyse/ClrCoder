// <copyright file="UPathTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests
{
    using System;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class UPathTests
    {
        [Test]
        [TestCase("c:\\test\\myfile.txt", "txt", null)]
        [TestCase("/test/myfile.txt", "txt", null)]
        public void GetUPathExtensionTest(string path, string expectedExtension, string expectedException)
        {
            if (expectedExtension != null)
            {
                new UPath(path).Extension().Should().Be(expectedExtension);
            }
            else
            {
                Action a = () => { new UPath(path).Extension(); };
                a.ShouldThrow<Exception>().Where(x => x.Message.StartsWith(expectedException));
            }
        }

        [Test]
        [TestCase("c:\\test\\file.txt", "C:/test")]
        [TestCase("/test/file.txt", "/test")]
        public void GetParentDir(string path, string expectedParentDir)
        {
            new UPath(path).ParentDir().ToString().Should().Be(expectedParentDir);
        }

        [Test]
        [TestCase("c:\\simple\\test", UPathParseMode.AllowIncorrectFormat, "C:/simple/test", null)]
        public void ParsePath(string path, UPathParseMode parseMode, string expectedPath, string expectedException)
        {
            if (expectedPath != null)
            {
                var p = new UPath(path, parseMode);
                p.ToString().Should().Be(expectedPath);

                bool result = UPath.TryParse(path, parseMode, out p);
                result.Should().BeTrue();
                p.ToString().Should().Be(expectedPath);
            }
            else
            {
                Action a = () =>
                    {
                        // ReSharper disable once ObjectCreationAsStatement
                        new UPath(path, parseMode);
                    };
                a.ShouldThrow<UPathFormatException>().Where(x => x.Message.StartsWith(expectedException));

                bool result = UPath.TryParse(path, parseMode, out var p);
                result.Should().BeFalse();
            }
        }

        [Test]
        [TestCase("c:/test/me", "/test/me")]
        [TestCase("/test/me", "/test/me")]
        [TestCase("test/me", "test/me")]
        public void ToUnixPathTest(string source, string expected)
        {
            new UPath(source).ToUnixPath().Should().Be(expected);
        }

        [Test]
        [TestCase("c:/test/me", "C:\\test\\me")]
        [TestCase("/test/me", "C:\\test\\me")]
        [TestCase("test/me", "test\\me")]
        public void ToWindowsPathTest(string source, string expected)
        {
            new UPath(source).ToWindowsPath().Should().Be(expected);
        }
    }
}