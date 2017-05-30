// <copyright file="LogEntry.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using System;

    using JetBrains.Annotations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using NodaTime;

    using Runtime.Serialization;

    /// <summary>
    /// Typed log entry.
    /// </summary>
    public class LogEntry : JLogEntry
    {
        [JsonConstructor]
        public LogEntry(string entryId)
        {
            EntryId = entryId;
        }

        public LogEntry()
            : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Call place information.
        /// </summary>
        [CanBeNull]
        public CallerInfo? CallerInfo { get; set; }

        /// <summary>
        /// Event details.
        /// </summary>
        [CanBeNull]
        public string Details { get; set; }

        /// <summary>
        /// DotNet class that produced this log.
        /// </summary>
        [CanBeNull]
        public string DotNetType { get; set; }

        /// <summary>
        /// Log entry identifier.
        /// </summary>
        [NotNull]
        public string EntryId { get; }

        /// <summary>
        /// UTC when event was raised.
        /// </summary>
        public Instant Instant { get; set; }

        /// <summary>
        /// Event message.
        /// </summary>
        [CanBeNull]
        public string Message { get; set; }

        /// <summary>
        /// Log entry severity.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// Exception dump.
        /// </summary>
        [CanBeNull]
        public ExceptionDto Exception { get; set; }
    }
}