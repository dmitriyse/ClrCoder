// <copyright file="EnvironmentExTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests
{
    using System.IO;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="EnvironmentEx"/>.
    /// </summary>
    [TestFixture]
    public class EnvironmentExTests
    {
        /// <summary>
        /// Test for the <see cref="EnvironmentEx.GetFileSystemRoot"/> method.
        /// </summary>
        [Test]
        public void GetFileSystemRootPath()
        {
            var fileSystemRoot = EnvironmentEx.GetFileSystemRoot();
            TestContext.WriteLine(fileSystemRoot);

            fileSystemRoot.Should().Be(Path.GetFullPath(fileSystemRoot));
            Directory.Exists(fileSystemRoot).Should().BeTrue();
        }
    }
}