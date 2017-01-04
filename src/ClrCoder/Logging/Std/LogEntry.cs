// <copyright file="LogEntry.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using NodaTime;

    /// <summary>
    /// Typed log entry.
    /// </summary>
    public class LogEntry : JLogEntry
    {
        /// <summary>
        /// Call place information.
        /// </summary>
        public CallerInfo? CallerInfo { get; set; }

        /// <summary>
        /// Event details.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// DotNet class that produced this log.
        /// </summary>
        public string DotNetType { get; set; }

        /// <summary>
        /// UTC when event was raised.
        /// </summary>
        public Instant Instant { get; set; }

        /// <summary>
        /// Event message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Log entry severity.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public LogSeverity Severity { get; set; }
    }
}