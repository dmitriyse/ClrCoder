// <copyright file="JsonRouterLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System;
    using System.Linq;

    using Json;

    using Threading;

    using Validation;

    /// <summary>
    /// Routes log entries across inner loggers.
    /// </summary>
    public class JsonRouterLogger : IJsonLogger
    {
        private readonly IJsonLogger[] _innerLoggers;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRouterLogger"/> class.
        /// </summary>
        /// <param name="innerLoggers">Inner loggers to route entries to.</param>
        public JsonRouterLogger(params IJsonLogger[] innerLoggers)
        {
            VxArgs.NotNull(innerLoggers, nameof(innerLoggers));

            _innerLoggers = innerLoggers;
            SerializerSource = _innerLoggers.FirstOrDefault()?.SerializerSource ?? JsonDefaults.RestRpcSerializerSource;
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler { get; } = new SyncHandler();

        /// <inheritdoc/>
        public IJsonSerializerSource SerializerSource { get; }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            foreach (IJsonLogger logger in _innerLoggers)
            {
                try
                {
                    logger.AsyncHandler.RunAsync((l, e) => { l.Log(e); }, logger, entry);
                }
                catch (Exception ex) when (ex.IsProcessable())
                {
                    // Do nothing.
                }
            }
        }
    }
}