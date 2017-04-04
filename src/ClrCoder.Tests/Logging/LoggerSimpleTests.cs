﻿// <copyright file="LoggerSimpleTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Logging
{
    using System.Collections.Generic;

    using ClrCoder.Logging;
    using ClrCoder.Logging.Std;

    using FluentAssertions;

    using Newtonsoft.Json;

    using NodaTime;

    using NUnit.Framework;

    /// <summary>
    /// Std logger simple tests.
    /// </summary>
    [TestFixture]
    public class LoggerSimpleTests
    {
        /// <summary>
        /// Log entry serialization test.
        /// </summary>
        [Test]
        public void LogEntrySerializationTest()
        {
            var tstLogEntry = new LogEntry
                                  {
                                      Message = "Msg",
                                      Details = "Some details.",
                                      CallerInfo = new CallerInfo("File.cs", "Namespace.Class.Method", 15),
                                      Severity = LogSeverity.Critical,
                                      Instant = Instant.FromUtc(2017, 1, 1, 2, 3),
                                      DotNetType = "SomeType",
                                      ExtensionData = new Dictionary<string, object>
                                                          {
                                                              { "Prop", "Str value" }
                                                          }
                                  };
            string serializedLogEntry = JsonConvert.SerializeObject(
                tstLogEntry,
                Formatting.Indented,
                StdJsonLogging.DefaultSerializerSource.Settings);
            TestContext.WriteLine(serializedLogEntry);
            var deserializedLogEntry = JsonConvert.DeserializeObject<LogEntry>(
                serializedLogEntry,
                StdJsonLogging.DefaultSerializerSource.Settings);

            deserializedLogEntry.ShouldBeEquivalentTo(tstLogEntry);
        }

        /// <summary>
        /// Simple logging API test.
        /// </summary>
        [Test]
        public void SimpleTest()
        {
            IJsonLogger log = RootTestingEnvironment.Logger;
            log.Debug(
                10,
                (_, a) => _("Test")
                    .Details(a.ToString()));
        }
    }
}