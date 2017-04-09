// <copyright file="ConsoleJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using System;
    using System.Linq;

    using Json;

    using Newtonsoft.Json;

    using NodaTime;

    using Runtime.Serialization;

    using Threading;

    /// <summary>
    /// Standard console json logger.
    /// </summary>
    public class ConsoleJsonLogger : IJsonLogger
    {
        private readonly DateTimeZone _localZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleJsonLogger"/> class.
        /// </summary>
        /// <param name="asyncHandler">Asynchronous log write handler.</param>
        /// <param name="serializerSource">Logging serializer. Used to convert objects to strings and to JObject.</param>
        public ConsoleJsonLogger(IAsyncHandler asyncHandler, IJsonSerializerSource serializerSource = null)
        {
            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            AsyncHandler = asyncHandler;

            SerializerSource = serializerSource ?? StdJsonLogging.DefaultSerializerSource;

            _localZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler { get; }

        /// <inheritdoc/>
        public IJsonSerializerSource SerializerSource { get; }

        /// <inheritdoc/>
        public JsonSerializer Serializer { get; }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            LogEntry logEntry = StdJsonLogging.NormalizeToLogEntry(entry, SerializerSource);

            string dotNetTypePrefix = logEntry.DotNetType == null ? string.Empty : $"{logEntry.DotNetType}: ";
            ConsoleColor colorBackup = Console.ForegroundColor;
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
                    case LogSeverity.Trace:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogSeverity.Debug:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                }

                Console.WriteLine(
                    $"{logEntry.Instant.InZone(_localZone):hh:mm:ss.f}: {dotNetTypePrefix}{logEntry.Message}");
                if (logEntry.Exception != null)
                {
                    var baseIntent = "  |";
                    Console.Write(baseIntent);
                    Console.WriteLine(
                        "------------------------------------- Exception ---------------------------------------");
                    string intent = string.Empty;
                    ExceptionDto curException = logEntry.Exception;
                    do
                    {
                        Console.Write(baseIntent);
                        Console.Write(intent);
                        if (intent != string.Empty)
                        {
                            Console.Write("<--");
                        }

                        string name = curException.TypeFullName?.Split('.')?.Last() ?? "NullName";
                        Console.WriteLine($"{name}: {curException.Message}");
                        curException = curException.InnerException;
                        intent += "    ";
                    }
                    while (curException != null);
                    Console.Write(baseIntent);
                    Console.WriteLine(
                        "---------------------------------------------------------------------------------------");
                }
            }
            finally
            {
                Console.ForegroundColor = colorBackup;
            }
        }
    }
}