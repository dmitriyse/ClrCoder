// <copyright file="LogEntriesSerializer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using MoreLinq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Serializes log entries.
    /// </summary>
    public class LogEntriesSerializer : IJsonLogger
    {
        [NotNull]
        [ItemNotNull]
        private readonly IReadOnlyCollection<IJsonLogger> _innerLoggers;

        [NotNull]
        private readonly JsonSerializerSettings _serializerSettings = JsonDefaults.JsonRestRpcSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntriesSerializer"/> class.
        /// </summary>
        /// <param name="innerLoggers">Inner loggers.</param>
        public LogEntriesSerializer([NotNull] [ItemNotNull] IReadOnlyCollection<IJsonLogger> innerLoggers)
        {
            if (innerLoggers == null)
            {
                throw new ArgumentNullException(nameof(innerLoggers));
            }

            _innerLoggers = innerLoggers;
        }

        /// <inheritdoc/>
        public void Log(object obj)
        {
            string serializedObj;

            if (obj is JObject)
            {
                serializedObj = ((JObject)obj).ToString(
                    _serializerSettings.Formatting,
                    _serializerSettings.Converters.ToArray());
            }
            else if (obj is string)
            {
                serializedObj = (string)obj;
            }
            else
            {
                serializedObj = JsonConvert.SerializeObject(obj, JsonDefaults.JsonRestRpcSerializerSettings);
            }

            _innerLoggers.ForEach(x => x.Log(serializedObj));
        }
    }
}