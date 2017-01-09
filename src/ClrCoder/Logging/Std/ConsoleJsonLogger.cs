// <copyright file="ConsoleJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using System;

    using JetBrains.Annotations;

    using NodaTime;

    using Threading;

    /// <summary>
    /// Standard console json logger.
    /// </summary>
    public class ConsoleJsonLogger : IJsonLogger
    {
        [NotNull]
        private readonly DateTimeZone _localZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleJsonLogger"/> class.
        /// </summary>
        /// <param name="asyncHandler">Asynchronous log write handler.</param>
        public ConsoleJsonLogger([NotNull] IAsyncHandler asyncHandler)
        {
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
            var logEntry = LoggerUtils.NormalizeToLogEntry(entry);

            var dotNetTypePrefix = logEntry.DotNetType == null ? string.Empty : $"{logEntry.DotNetType}: ";
            var colorBackup = Console.ForegroundColor;
            try
            {
                switch (logEntry.Severity)
                {
                    case LogSeverity.Critical:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogSeverity.Error:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case LogSeverity.Warning:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case LogSeverity.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogSeverity.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogSeverity.Trace:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                }

                Console.WriteLine(
                    $"{logEntry.Instant.InZone(_localZone):hh:mm:ss.f}: {dotNetTypePrefix}{logEntry.Message}");
            }
            finally
            {
                Console.ForegroundColor = colorBackup;
            }
        }
    }
}