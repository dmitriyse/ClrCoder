// <copyright file="TextJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using System;

    using Json;

    using NodaTime;

    using Threading;

    /// <summary>
    /// Writes logs messages into text stream
    /// </summary>
    public class TextJsonLogger : IJsonLogger
    {
        private readonly Action<string> _writeAction;

        private readonly DateTimeZone _localZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextJsonLogger"/> class.
        /// </summary>
        /// <param name="asyncHandler">Asynchronous log write handler.</param>
        /// <param name="writeAction">Action that performs entry write operation.</param>
        /// <param name="serializerSource">The serializer source.</param>
        public TextJsonLogger(
            IAsyncHandler asyncHandler,
            Action<string> writeAction,
            IJsonSerializerSource serializerSource = null)
        {
            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            if (writeAction == null)
            {
                throw new ArgumentNullException(nameof(writeAction));
            }

            _writeAction = writeAction;
            AsyncHandler = asyncHandler;
            SerializerSource = serializerSource
                               ?? new JsonSerializerSource(
                                   () => StdJsonLogging.DefaultSerializerSource.CreateSettings());

            _localZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler { get; }

        /// <inheritdoc/>
        public IJsonSerializerSource SerializerSource { get; }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            LogEntry logEntry = StdJsonLogging.NormalizeToLogEntry(entry, SerializerSource);

            string dotNetTypePrefix = logEntry.DotNetType == null ? string.Empty : $"{logEntry.DotNetType}: ";

            _writeAction($"{logEntry.Instant.InZone(_localZone):hh:mm:ss.f}: {dotNetTypePrefix}{logEntry.Message}");
        }
    }
}