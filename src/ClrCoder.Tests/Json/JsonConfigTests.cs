// <copyright file="JsonConfigTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Json
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using ClrCoder.Json;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// JsonConfig class tests.
    /// </summary>
    [TestFixture]
    public class JsonConfigTests
    {
        /// <summary>
        /// Config file load test.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task LoadConfigTest()
        {
            string fileName = Path.Combine(AppContext.BaseDirectory, "test.cfg.json");
            File.WriteAllText(fileName, "{\"test\":\"data\"}");
            TestConfig testConfig =
                await JsonDefaults.JsonConfigSerializerSource.Serializer.DeserializeFile<TestConfig>(fileName);
            testConfig.Test.Should().Be("data");
        }

        private class TestConfig
        {
            public string Test { get; set; }
        }
    }
}