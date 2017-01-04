// <copyright file="SerializerJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using System;

    using JetBrains.Annotations;

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
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler => _innerLogger.AsyncHandler;

        /// <inheritdoc/>
        public void Log(object entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            var serializedEntry = LoggerUtils.NormalizeToString(entry);
            _innerLogger.Log(serializedEntry);
        }
    }
}