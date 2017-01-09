// <copyright file="StreamJsonLogger.cs" company="ClrCoder project">
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
    /// Writes logs messages into text stream
    /// </summary>
    public class TextJsonLogger: IJsonLogger
    {
        [NotNull]
        private readonly Action<string> _writeAction;

        [NotNull]
        private readonly DateTimeZone _localZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleJsonLogger"/> class.
        /// </summary>
        /// <param name="asyncHandler">Asynchronous log write handler.</param>
        /// <param name="writeAction">Action that performs entry write operation.</param>
        public TextJsonLogger([NotNull] IAsyncHandler asyncHandler, [NotNull] Action<string> writeAction)
        {
            _writeAction = writeAction;
            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }
            if (writeAction == null)
            {
                throw new ArgumentNullException(nameof(writeAction));
            }

            AsyncHandler = asyncHandler;

            _localZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler { get; }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            var logEntry = LoggerUtils.NormalizeToLogEntry(entry);

            var dotNetTypePrefix = logEntry.DotNetType == null ? string.Empty : $"{logEntry.DotNetType}: ";

            _writeAction($"{logEntry.Instant.InZone(_localZone):hh:mm:ss.f}: {dotNetTypePrefix}{logEntry.Message}");
        }
    }
}