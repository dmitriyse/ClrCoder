﻿// <copyright file="NUnitJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Testing
{
    using System;
    using System.Linq;

    using Json;

    using Logging;
    using Logging.Std;

    using NodaTime;

    using NUnit.Framework.Internal;

    using Runtime.Serialization;

    using Threading;

    /// <summary>
    /// Logger that outputs to NUnit test result.
    /// </summary>
    public class NUnitJsonLogger : IJsonLogger
    {
        private readonly DateTimeZone _localZone;

        private TestExecutionContext _testExecutionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitJsonLogger"/> class.
        /// </summary>
        public NUnitJsonLogger()
            : this(new SyncHandler())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitJsonLogger"/> class.
        /// </summary>
        /// <param name="asyncHandler">Asynchronous log write handler.</param>
        /// <param name="context">Test execution context, temporary required to workaround NUnit bug.</param>
        /// <param name="serializerSource">The serializer source.</param>
        public NUnitJsonLogger(
            IAsyncHandler asyncHandler,
            TestExecutionContext context = null,
            IJsonSerializerSource serializerSource = null)
        {
            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            AsyncHandler = asyncHandler;

            _testExecutionContext = context ?? TestExecutionContext.CurrentContext;

            SerializerSource = serializerSource ?? StdJsonLogging.DefaultSerializerSource;

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

            _testExecutionContext.OutWriter.WriteLine(
                $"{logEntry.Instant.InZone(_localZone):hh:mm:ss.f}: {dotNetTypePrefix}{logEntry.Message}");

            if (logEntry.Exception != null)
            {
                var baseIntent = "  |";
                _testExecutionContext.OutWriter.Write(baseIntent);
                _testExecutionContext.OutWriter.WriteLine(
                    "------------------------------------- Exception ---------------------------------------");
                string intent = string.Empty;
                ExceptionDto curException = logEntry.Exception;
                do
                {
                    _testExecutionContext.OutWriter.Write(baseIntent);
                    _testExecutionContext.OutWriter.Write(intent);
                    if (intent != string.Empty)
                    {
                        _testExecutionContext.OutWriter.Write("<--");
                    }

                    string name = curException.TypeFullName?.Split('.')?.Last() ?? "NullName";
                    _testExecutionContext.OutWriter.WriteLine($"{name}: {curException.Message}");
                    curException = curException.InnerException;
                    intent += "    ";
                }
                while (curException != null);
                _testExecutionContext.OutWriter.Write(baseIntent);
                _testExecutionContext.OutWriter.WriteLine(
                    "---------------------------------------------------------------------------------------");
            }
        }
    }
}