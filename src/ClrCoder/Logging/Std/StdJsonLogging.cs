// <copyright file="StdJsonLogging.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System;
    using System.Linq;

    using Annotations;

    using JetBrains.Annotations;

    using Json;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Validation;

    /// <summary>
    /// Root for standard json logging subsystem methods.
    /// </summary>
    [PublicAPI]
    public static class StdJsonLogging
    {
        [NotNull]
        private static IJsonSerializerSource _defaultSerializerSettings = new JsonSerializerSource(
            () =>
                {
                    JsonSerializerSettings settings = JsonDefaults.BaseSerializerSource.CreateSettings();

                    // TODO: Init me
                    return settings;
                });

        /// <summary>
        /// Creates serializer settings for standard json logging.
        /// </summary>
        /// <remarks>
        /// When this method will become performance critical we needs to add new property families with settings and serializers
        /// cache.
        /// </remarks>
        [NotNull]
        [ThreadUnsafe]
        public static IJsonSerializerSource DefaultSerializerSource
        {
            get => _defaultSerializerSettings;

            set
            {
                VxArgs.NotNull(value, nameof(value));
                _defaultSerializerSettings = value;
            }
        }

        /// <summary>
        /// Normalizes log <c>entry</c> to <see cref="LogEntry"/> form.
        /// </summary>
        /// <param name="entry">Log entry in any form.</param>
        /// <param name="serializerSource">Serializer settings.</param>
        /// <returns>Entry in LogEntry form.</returns>
        [NotNull]
        public static LogEntry NormalizeToLogEntry(object entry, IJsonSerializerSource serializerSource)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (serializerSource == null)
            {
                throw new ArgumentNullException(nameof(serializerSource));
            }

            var logEntry = entry as LogEntry;
            if (logEntry != null)
            {
                return logEntry;
            }

            JsonSerializerSettings serializerSettings = serializerSource.Settings;

            var serializedLogEntry = entry as string;
            if (serializedLogEntry != null)
            {
                var deserializedLogEntry = JsonConvert.DeserializeObject<LogEntry>(
                    serializedLogEntry,
                    serializerSource.Settings);
                return deserializedLogEntry;
            }

            var jsonObjectLogEntry = entry as JObject;
            if (jsonObjectLogEntry != null)
            {
                var deserializedLogEntry = jsonObjectLogEntry.ToObject<LogEntry>(serializerSource.Serializer);
                return deserializedLogEntry;
            }

            var jsonLogEntry = entry as JLogEntry;
            if (jsonLogEntry != null)
            {
                string serializedJLogEntry = JsonConvert.SerializeObject(jsonLogEntry, serializerSettings);
                var deserializedLogEntry = JsonConvert.DeserializeObject<LogEntry>(
                    serializedJLogEntry,
                    serializerSettings);
                return deserializedLogEntry;
            }

            throw new ArgumentException("Log entry should be string, JLogEntry or JObject");
        }

        /// <summary>
        /// Normalizes log <c>entry</c> to string (actually serialize).
        /// </summary>
        /// <param name="entry">The log <c>entry</c>.</param>
        /// <param name="serializerSource">The serializer source.</param>
        /// <returns>Serialized string.</returns>
        public static string NormalizeToString(object entry, IJsonSerializerSource serializerSource)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (serializerSource == null)
            {
                throw new ArgumentNullException(nameof(serializerSource));
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

            JsonSerializerSettings settings = serializerSource.Settings;
            var logEntry = entry as JLogEntry;
            if (logEntry != null)
            {
                string serializedEntry = JsonConvert.SerializeObject(logEntry, settings);
                return serializedEntry;
            }

            var jsonLogEntry = entry as JObject;
            if (jsonLogEntry != null)
            {
                string serializedEntry = jsonLogEntry.ToString(
                    settings.Formatting,
                    settings.Converters.ToArray());
                return serializedEntry;
            }

            throw new ArgumentException("Log entry should be string, JLogEntry or JObject");
        }
    }
#endif
}