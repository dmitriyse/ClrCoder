// <copyright file="JsonConfigTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Json
{
    using System.IO;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// JsonConfig class tests.
    /// </summary>
    [TestFixture]
    public class JsonConfigTests
    {
#if !PCL

        /// <summary>
        /// Config file load test.
        /// </summary>
        [Test]
        public void LoadConfigTest()
        {
            var fileName = "test.cfg.json";
            File.WriteAllText(fileName, "{\"test\":\"data\"}");
            var testConfig = JsonConfig.Load<TestConfig>(fileName);
            testConfig.Test.Should().Be("data");
        }

        private class TestConfig
        {
            public string Test { get; set; }
        }
#endif
    }
}