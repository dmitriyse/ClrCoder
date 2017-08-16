// <copyright file="SerializerJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System;

    using JetBrains.Annotations;

    using Json;

    using Threading;

    /// <summary>
    /// Proxy logger that serializes log entries.
    /// </summary>
    public class SerializerJsonLogger : IJsonLogger
    {
        private readonly IJsonLogger _innerLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="innerLogger"/> class.
        /// </summary>
        /// <param name="innerLogger">Inner logger.</param>
        public SerializerJsonLogger([NotNull] IJsonLogger innerLogger)
        {
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

            string serializedEntry = StdJsonLogging.NormalizeToString(entry, SerializerSource);
            _innerLogger.Log(serializedEntry);
        }
    }
#endif
}