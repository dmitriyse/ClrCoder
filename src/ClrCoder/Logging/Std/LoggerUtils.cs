// <copyright file="LoggerUtils.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using System;
    using System.Linq;

    using JetBrains.Annotations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Logger utilities methods.
    /// </summary>
    [PublicAPI]
    public static class LoggerUtils
    {
        /// <summary>
        /// Default json serializer for std logging utils.
        /// </summary>
        internal static readonly JsonSerializer LogEntriesSerializer = JsonSerializer.Create(
            JsonDefaults.JsonRestRpcSerializerSettings);

        /// <summary>
        /// Default settings for log entries serialization.
        /// </summary>
        public static JsonSerializerSettings LogEntriesSerializerSettings { get; } =
            JsonDefaults.JsonRestRpcSerializerSettings;

        /// <summary>
        /// Normalizes log <c>entry</c> to <see cref="LogEntry"/> form.
        /// </summary>
        /// <param name="entry">Log entry in any form.</param>
        /// <returns>Entry in LogEntry form.</returns>
        [NotNull]
        public static LogEntry NormalizeToLogEntry([NotNull] object entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            var logEntry = entry as LogEntry;
            if (logEntry != null)
            {
                return logEntry;
            }

            var serializedLogEntry = entry as string;
            if (serializedLogEntry != null)
            {
                var deserializedLogEntry = JsonConvert.DeserializeObject<LogEntry>(
                    serializedLogEntry,
                    LogEntriesSerializerSettings);
                return deserializedLogEntry;
            }

            var jObjectLogEntry = entry as JObject;
            if (jObjectLogEntry != null)
            {
                var deserializedLogEntry = jObjectLogEntry.ToObject<LogEntry>(LogEntriesSerializer);
                return deserializedLogEntry;
            }

            var jLogEntry = entry as JLogEntry;
            if (jLogEntry != null)
            {
                string serializedJLogEntry = JsonConvert.SerializeObject(jLogEntry, LogEntriesSerializerSettings);
                var deserializedLogEntry = JsonConvert.DeserializeObject<LogEntry>(
                    serializedJLogEntry,
                    LogEntriesSerializerSettings);
                return deserializedLogEntry;
            }

            throw new ArgumentException("Log entry should be string, JLogEntry or JObject");
        }

        /// <summary>
        /// Normalizes log <c>entry</c> to string (actually serialize).
        /// </summary>
        /// <param name="entry">Log <c>entry</c>.</param>
        /// <returns>Serialized string.</returns>
        public static string NormalizeToString([NotNull] object entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            var logEntryString = entry as string;
            if (logEntryString != null)
            {
#if (DEBUG)

                // Just to ensure that string is valid log entry.
                JsonConvert.DeserializeObject<LogEntry>(logEntryString);
#endif
                return logEntryString;
            }

            var logEntry = entry as JLogEntry;
            if (logEntry != null)
            {
                string serializedEntry = JsonConvert.SerializeObject(logEntry, LogEntriesSerializerSettings);
                return serializedEntry;
            }

            var jLogEntry = entry as JObject;
            if (jLogEntry != null)
            {
                string serializedEntry = jLogEntry.ToString(
                    LogEntriesSerializerSettings.Formatting,
                    LogEntriesSerializerSettings.Converters.ToArray());
                return serializedEntry;
            }

            throw new ArgumentException("Log entry should be string, JLogEntry or JObject");
        }
    }
}