// <copyright file="JsonInterceptedLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System;

    using JetBrains.Annotations;

    using Json;

    using Threading;

    /// <summary>
    /// The proxy logger with the interceptor.
    /// </summary>
    public class JsonInterceptedLogger : IJsonLogger, ILogEntryBuilder
    {
        private readonly IJsonLogger _innerLogger;

        private readonly ILogEntryBuilder _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonInterceptedLogger"/> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger.</param>
        /// <param name="interceptor">The interceptor.</param>
        public JsonInterceptedLogger(IJsonLogger innerLogger, [CanBeNull] JsonLogEntryInterceptor interceptor)
        {
            _innerLogger = innerLogger ?? throw new ArgumentNullException(nameof(innerLogger));
            interceptor = interceptor.DoNothingIfNull();

            _builder = interceptor(this);
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler => _innerLogger.AsyncHandler;

        /// <inheritdoc cref="IJsonLogger"/>
        /// .
        public IJsonSerializerSource SerializerSource => _innerLogger.SerializerSource;

        /// <inheritdoc/>
        object ILogEntryBuilder.Build(object entry)
        {
            return entry;
        }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            _innerLogger.Log(_builder.Build(entry));
        }
    }
}