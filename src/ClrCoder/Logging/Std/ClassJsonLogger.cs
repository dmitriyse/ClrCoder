// <copyright file="ClassJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using System;

    using Json;

    using Threading;

    using Validation;

    /// <summary>
    /// Logger that adds class information to messages.
    /// </summary>
    /// <typeparam name="T">Target type whose name will be injected into log entries.</typeparam>
    public class ClassJsonLogger<T> : IJsonLogger
    {
        private static readonly string TypeName = typeof(T).Name;

        private readonly IJsonLogger _innerLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassJsonLogger{T}"/> class.
        /// </summary>
        /// <param name="innerLogger">Next logger in chain.</param>
        public ClassJsonLogger(IJsonLogger innerLogger)
        {
            VxArgs.NotNull(innerLogger, nameof(innerLogger));
            _innerLogger = innerLogger;
            SerializerSource = innerLogger.SerializerSource;
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler => _innerLogger.AsyncHandler;

        /// <inheritdoc/>
        public IJsonSerializerSource SerializerSource { get; }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            LogEntry logEntry = StdJsonLogging.NormalizeToLogEntry(entry, SerializerSource);

            logEntry.DotNetType = TypeName;

            _innerLogger.Log(logEntry);
        }
    }
}