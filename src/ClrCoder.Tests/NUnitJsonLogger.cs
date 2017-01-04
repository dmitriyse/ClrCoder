// <copyright file="NUnitJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests
{
    using ClrCoder.Logging;
    using ClrCoder.Logging.Std;

    using JetBrains.Annotations;

    using NodaTime;

    using NUnit.Framework;

    using Threading;

    /// <summary>
    /// Logger that outputs to <see cref="TestContext"/>.
    /// </summary>
    public class NUnitJsonLogger : IJsonLogger
    {
        [NotNull]
        private readonly DateTimeZone _localZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitJsonLogger"/> class.
        /// </summary>
        public NUnitJsonLogger()
        {
            AsyncHandler = new SyncHandler();
            _localZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler { get; }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            var logEntry = LoggerUtils.NormalizeToLogEntry(entry);
            var dotNetTypePrefix = logEntry.DotNetType == null ? string.Empty : $"{logEntry.DotNetType}: ";

            TestContext.WriteLine(
                $"{logEntry.Instant.InZone(_localZone):hh:mm:ss.f}:{logEntry.Severity}: {dotNetTypePrefix}{logEntry.Message}");
        }
    }
}