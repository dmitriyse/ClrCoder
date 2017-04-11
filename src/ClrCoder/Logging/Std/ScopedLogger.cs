// <copyright file="ScopedLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using System;

    using JetBrains.Annotations;

    using Json;

    using Threading;

    /// <summary>
    /// Logger proxy that writes adds scopes to log entries.
    /// </summary>
    /// <remarks>
    /// TODO: Implement nested scopes.
    /// </remarks>
    public class ScopedLogger : IJsonLogger
    {
        private readonly object _scopeId;

        private IJsonLogger _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedLogger"/> class.
        /// </summary>
        /// <param name="inner">The parent logger.</param>
        /// <param name="scopeId">Scope identifier object.</param>
        public ScopedLogger(IJsonLogger inner, object scopeId)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            _inner = inner;
            _scopeId = scopeId;
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler => _inner.AsyncHandler;

        /// <inheritdoc/>
        public IJsonSerializerSource SerializerSource => _inner.SerializerSource;

        /// <inheritdoc/>
        public void Log(object entry)
        {
            LogEntry logEntry = StdJsonLogging.NormalizeToLogEntry(entry, SerializerSource);

            // TODO: Implement nested scopes.
            logEntry.SetExtensionData(
                "logScope",
                new LogScope
                    {
                        Id = _scopeId
                    });

            _inner.Log(logEntry);
        }

        private class LogScope
        {
            public object Id { get; set; }

            [CanBeNull]
            public LogScope NestedScope { get; set; }
        }
    }
}