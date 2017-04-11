// <copyright file="LoggerExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using System;

    using JetBrains.Annotations;

    using Newtonsoft.Json.Linq;

    using NodaTime;

    using Runtime.Serialization;

    /// <summary>
    /// Extension methods that represents logging api.
    /// </summary>
    [PublicAPI]
    public static partial class LoggerExtensions
    {
        /// <summary>
        /// Adds <c>data</c> to the json entry.
        /// </summary>
        /// <param name="builder">Entry <c>builder</c>.</param>
        /// <param name="data">Object with <c>data</c>.</param>
        /// <returns>The same <c>builder</c> for fluent syntax.</returns>
        public static ILogEntryBuilder Data(this ILogEntryBuilder builder, object data)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return new DelegateLogEntryBuilderForLogEntry(
                builder,
                e =>
                    {
                        try
                        {
                            JObject jsonData = JObject.FromObject(data, builder.SerializerSource.Serializer);
                            foreach (JProperty jProperty in jsonData.Properties())
                            {
                                e.SetExtensionData(jProperty.Name, jProperty.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!ex.IsProcessable())
                            {
                                throw;
                            }

                            // TODO: Dump exception.
                            e.SetExtensionData("DataSerializationError", ex.ToString());
                        }

                        return e;
                    });
        }

        /// <summary>
        /// Adds <c>details</c> text to a log entry.
        /// </summary>
        /// <param name="builder">Log entry <c>builder</c>.</param>
        /// <param name="details">Details text.</param>
        /// <returns>Updated <c>builder</c>.</returns>
        public static ILogEntryBuilder Details(this ILogEntryBuilder builder, [NotNull] string details)
        {
            if (details == null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            return new DelegateLogEntryBuilderForLogEntry(
                builder,
                e =>
                    {
                        if (string.IsNullOrWhiteSpace(e.Details))
                        {
                            e.Details = details;
                        }
                        else
                        {
                            e.Details = $"{e.Details}{Environment.NewLine}{details}";
                        }

                        return e;
                    });
        }

        /// <summary>
        /// Adds <c>exception</c> dump to the json entry.
        /// </summary>
        /// <param name="builder">Entry <c>builder</c>.</param>
        /// <param name="exception">Exception to log.</param>
        /// <returns>The same <c>builder</c> for fluent syntax.</returns>
        public static ILogEntryBuilder Exception(this ILogEntryBuilder builder, Exception exception)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var exceptionDump = new JsonExceptionDumper().Dump<ExceptionDto>(exception);

            return new DelegateLogEntryBuilderForLogEntry(
                builder,
                e =>
                    {
                        e.Exception = exceptionDump;
                        return e;
                    });
        }

        /// <summary>
        /// Creates logger proxy for the provided scope id.
        /// </summary>
        /// <param name="logger">The parent logger.</param>
        /// <param name="scopeId">Scope identifier.</param>
        /// <returns>The scoped logger.</returns>
        public static IJsonLogger UseScope(this IJsonLogger logger, object scopeId)
        {
            return new ScopedLogger(logger, scopeId);
        }

        private static void WriteLogEntry(
            IJsonLogger logger,
            LogSeverity severity,
            Func<Func<string, ILogEntryBuilder>, ILogEntryBuilder> logEntryBuilder,
            string callerFilePath,
            string callerMemberName,
            int callerLineNumber)
        {
            if (callerFilePath == null)
            {
                throw new ArgumentNullException(nameof(callerFilePath));
            }

            if (callerMemberName == null)
            {
                throw new ArgumentNullException(nameof(callerMemberName));
            }

            Instant instant = SystemClock.Instance.GetCurrentInstant();

            // TODO: Optimize performance here.
            logger.AsyncHandler.RunAsync(
                (instantArg, callInfo) =>
                    {
                        ILogEntryBuilder entryBuilder = logEntryBuilder(
                            msg => new DelegateLogEntryBuilderForLogEntry(
                                e =>
                                    {
                                        e.Message = msg;
                                        return e;
                                    },
                                logger.SerializerSource));

                        var entry = new LogEntry
                                        {
                                            Severity = severity,
                                            Instant = instant,
                                            CallerInfo = callInfo
                                        };
                        object buildedEntry = entryBuilder.Build(entry);
                        logger.Log(buildedEntry);
                    },
                instant,
                new CallerInfo(callerFilePath, callerMemberName, callerLineNumber));
        }

        private static void WriteLogEntry<T>(
            IJsonLogger logger,
            LogSeverity severity,
            T arg,
            Func<Func<string, ILogEntryBuilder>, T, ILogEntryBuilder> logEntryBuilder,
            string callerFilePath,
            string callerMemberName,
            int callerLineNumber)
        {
            if (callerFilePath == null)
            {
                throw new ArgumentNullException(nameof(callerFilePath));
            }

            if (callerMemberName == null)
            {
                throw new ArgumentNullException(nameof(callerMemberName));
            }

            Instant instant = SystemClock.Instance.GetCurrentInstant();

            logger.AsyncHandler.RunAsync(
                (instantArg, callInfo, arg1) =>
                    {
                        ILogEntryBuilder entryBuilder = logEntryBuilder(
                            msg => new DelegateLogEntryBuilderForLogEntry(
                                e =>
                                    {
                                        e.Message = msg;
                                        return e;
                                    },
                                logger.SerializerSource),
                            arg1);

                        var entry = new LogEntry
                                        {
                                            Severity = severity,
                                            Instant = instant,
                                            CallerInfo = callInfo
                                        };
                        object buildedEntry = entryBuilder.Build(entry);
                        logger.Log(buildedEntry);
                    },
                instant,
                new CallerInfo(callerFilePath, callerMemberName, callerLineNumber),
                arg);
        }

        private static void WriteLogEntry<T1, T2>(
            IJsonLogger logger,
            LogSeverity severity,
            T1 arg1,
            T2 arg2,
            Func<Func<string, ILogEntryBuilder>, T1, T2, ILogEntryBuilder> logEntryBuilder,
            string callerFilePath,
            string callerMemberName,
            int callerLineNumber)
        {
            if (callerFilePath == null)
            {
                throw new ArgumentNullException(nameof(callerFilePath));
            }

            if (callerMemberName == null)
            {
                throw new ArgumentNullException(nameof(callerMemberName));
            }

            Instant instant = SystemClock.Instance.GetCurrentInstant();

            logger.AsyncHandler.RunAsync(
                (instantArg, callInfo, a1, a2) =>
                    {
                        ILogEntryBuilder entryBuilder = logEntryBuilder(
                            msg => new DelegateLogEntryBuilderForLogEntry(
                                e =>
                                    {
                                        e.Message = msg;
                                        return e;
                                    },
                                logger.SerializerSource),
                            a1,
                            a2);

                        var entry = new LogEntry
                                        {
                                            Severity = severity,
                                            Instant = instant,
                                            CallerInfo = callInfo
                                        };

                        object buildedEntry = entryBuilder.Build(entry);
                        logger.Log(buildedEntry);
                    },
                instant,
                new CallerInfo(callerFilePath, callerMemberName, callerLineNumber),
                arg1,
                arg2);
        }

        private static void WriteLogEntry<T1, T2, T3>(
            IJsonLogger logger,
            LogSeverity severity,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            Func<Func<string, ILogEntryBuilder>, T1, T2, T3, ILogEntryBuilder> logEntryBuilder,
            string callerFilePath,
            string callerMemberName,
            int callerLineNumber)
        {
            if (callerFilePath == null)
            {
                throw new ArgumentNullException(nameof(callerFilePath));
            }

            if (callerMemberName == null)
            {
                throw new ArgumentNullException(nameof(callerMemberName));
            }

            Instant instant = SystemClock.Instance.GetCurrentInstant();

            logger.AsyncHandler.RunAsync(
                (instantArg, callInfo, a1, a2, a3) =>
                    {
                        ILogEntryBuilder entryBuilder = logEntryBuilder(
                            msg => new DelegateLogEntryBuilderForLogEntry(
                                e =>
                                    {
                                        e.Message = msg;
                                        return e;
                                    },
                                logger.SerializerSource),
                            a1,
                            a2,
                            a3);

                        var entry = new LogEntry
                                        {
                                            Severity = severity,
                                            Instant = instant,
                                            CallerInfo = callInfo
                                        };

                        object buildedEntry = entryBuilder.Build(entry);
                        logger.Log(buildedEntry);
                    },
                instant,
                new CallerInfo(callerFilePath, callerMemberName, callerLineNumber),
                arg1,
                arg2,
                arg3);
        }
    }
}