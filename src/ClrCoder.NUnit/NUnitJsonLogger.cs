// <copyright file="NUnitJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.NUnit
{
    using System;
    using System.Linq;

    using global::NUnit.Framework;

    using Logging;
    using Logging.Std;

    using NodaTime;

    using Runtime.Serialization;

    using Threading;

    /// <summary>
    /// Logger that outputs to NUnit test result.
    /// </summary>
    public class NUnitJsonLogger : IJsonLogger
    {
        private readonly DateTimeZone _localZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitJsonLogger"/> class.
        /// </summary>
        /// <param name="asyncHandler">Asynchronous log write handler.</param>
        public NUnitJsonLogger(IAsyncHandler asyncHandler)
        {
            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            AsyncHandler = asyncHandler;

            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            _localZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler { get; }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            LogEntry logEntry = LoggerUtils.NormalizeToLogEntry(entry);

            string dotNetTypePrefix = logEntry.DotNetType == null ? string.Empty : $"{logEntry.DotNetType}: ";

            TestContext.WriteLine(
                $"{logEntry.Instant.InZone(_localZone):hh:mm:ss.f}: {dotNetTypePrefix}{logEntry.Message}");

            if (logEntry.Exception != null)
            {
                var baseIntent = "  |";
                TestContext.Write(baseIntent);
                TestContext.WriteLine(
                    "------------------------------------- Exception ---------------------------------------");
                string intent = string.Empty;
                ExceptionDto curException = logEntry.Exception;
                do
                {
                    TestContext.Write(baseIntent);
                    TestContext.Write(intent);
                    if (intent != string.Empty)
                    {
                        TestContext.Write("<--");
                    }

                    string name = curException.TypeFullName?.Split('.')?.Last() ?? "NullName";
                    TestContext.WriteLine($"{name}: {curException.Message}");
                    curException = curException.InnerException;
                    intent += "    ";
                }
                while (curException != null);
                TestContext.Write(baseIntent);
                TestContext.WriteLine(
                    "---------------------------------------------------------------------------------------");
            }
        }
    }
}