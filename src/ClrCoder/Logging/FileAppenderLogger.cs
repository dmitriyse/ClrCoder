// <copyright file="FileAppenderLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System;
    using System.IO;

    using JetBrains.Annotations;

    using Json;

    using Newtonsoft.Json;

    using NodaTime;

    using Std;

    using Text;

    using Threading;

    /// <summary>
    /// Appends log entries to a file.
    /// </summary>
    public class FileAppenderLogger : IJsonLogger
    {
        [NotNull]
        private readonly string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAppenderLogger"/> class.
        /// </summary>
        /// <param name="fileName">File name to append log entries to.</param>
        /// <param name="asyncHandler">Asynchronous handler for logs processing.</param>
        /// <param name="serializerSource">The serializer source.</param>
        public FileAppenderLogger(
            string fileName,
            IAsyncHandler asyncHandler,
            IJsonSerializerSource serializerSource = null)
        {
            // ReSharper disable JoinNullCheckWithUsage
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            // ReSharper restore JoinNullCheckWithUsage
            _fileName = fileName;
            AsyncHandler = asyncHandler;
            SerializerSource = serializerSource ?? StdJsonLogging.DefaultSerializerSource;
            if (SerializerSource.Settings.Formatting != Formatting.None)
            {
                throw new NotSupportedException("Serialized json strings should be formatted in a one line.");
            }
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler { get; }

        /// <inheritdoc/>
        public IJsonSerializerSource SerializerSource { get; }

        /// <summary>
        /// Gets logs file name for current application execution.
        /// </summary>
        /// <returns>File name for logging.</returns>
        public static string GetLogFileNameForCurrentAppRun()
        {
            string logsDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }

            DateTimeZone timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();

            ZonedDateTime localTime = SystemClock.Instance.GetCurrentInstant().InZone(timeZone);
            string fileName = $"{localTime:yyyy-MM-dd_HH-mm-ss}.jlog";
            return Path.GetFullPath(Path.Combine(logsDirectory, fileName));
        }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            string entryString = StdJsonLogging.NormalizeToString(entry, SerializerSource);
            File.AppendAllLines(_fileName, new[] { entryString }, EncodingEx.UTF8NoBom);
        }
    }
}