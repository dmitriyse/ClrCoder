// <copyright file="StdLoggerExtensions.EmitMethods.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System;
    using System.Runtime.CompilerServices;

    /// <content>Log emmit methods.</content>
    public static partial class StdLoggerExtensions
    {
        /// <summary>
        /// Writes critical log entry.
        /// </summary>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Critical(
            this IJsonLogger logger,
            Func<Func<string, ILogEntryBuilder>, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Critical,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes critical log entry.
        /// </summary>
        /// <typeparam name="T">Log entry builder argument 1 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg">Log entry builder argument 1.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Critical<T>(
            this IJsonLogger logger,
            T arg,
            Func<Func<string, ILogEntryBuilder>, T, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Critical,
                arg,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes critical log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Critical<T1, T2>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            Func<Func<string, ILogEntryBuilder>, T1, T2, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Critical,
                arg1,
                arg2,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes critical log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <typeparam name="T3">Log entry builder argument 3 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="arg3">Log entry builder argument 3.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Critical<T1, T2, T3>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            Func<Func<string, ILogEntryBuilder>, T1, T2, T3, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Critical,
                arg1,
                arg2,
                arg3,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes debug log entry.
        /// </summary>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Debug(
            this IJsonLogger logger,
            Func<Func<string, ILogEntryBuilder>, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Debug,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes debug log entry.
        /// </summary>
        /// <typeparam name="T">Log entry builder argument 1 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg">Log entry builder argument 1.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Debug<T>(
            this IJsonLogger logger,
            T arg,
            Func<Func<string, ILogEntryBuilder>, T, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Debug,
                arg,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes debug log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Debug<T1, T2>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            Func<Func<string, ILogEntryBuilder>, T1, T2, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Debug,
                arg1,
                arg2,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes debug log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <typeparam name="T3">Log entry builder argument 3 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="arg3">Log entry builder argument 3.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Debug<T1, T2, T3>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            Func<Func<string, ILogEntryBuilder>, T1, T2, T3, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Debug,
                arg1,
                arg2,
                arg3,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes error log entry.
        /// </summary>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Error(
            this IJsonLogger logger,
            Func<Func<string, ILogEntryBuilder>, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Error,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes error log entry.
        /// </summary>
        /// <typeparam name="T">Log entry builder argument 1 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg">Log entry builder argument 1.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Error<T>(
            this IJsonLogger logger,
            T arg,
            Func<Func<string, ILogEntryBuilder>, T, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Error,
                arg,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes error log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Error<T1, T2>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            Func<Func<string, ILogEntryBuilder>, T1, T2, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Error,
                arg1,
                arg2,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes error log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <typeparam name="T3">Log entry builder argument 3 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="arg3">Log entry builder argument 3.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Error<T1, T2, T3>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            Func<Func<string, ILogEntryBuilder>, T1, T2, T3, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Error,
                arg1,
                arg2,
                arg3,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes info log entry.
        /// </summary>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Info(
            this IJsonLogger logger,
            Func<Func<string, ILogEntryBuilder>, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Info,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes info log entry.
        /// </summary>
        /// <typeparam name="T">Log entry builder argument 1 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg">Log entry builder argument 1.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Info<T>(
            this IJsonLogger logger,
            T arg,
            Func<Func<string, ILogEntryBuilder>, T, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Info,
                arg,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes info log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Info<T1, T2>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            Func<Func<string, ILogEntryBuilder>, T1, T2, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Info,
                arg1,
                arg2,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes info log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <typeparam name="T3">Log entry builder argument 3 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="arg3">Log entry builder argument 3.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Info<T1, T2, T3>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            Func<Func<string, ILogEntryBuilder>, T1, T2, T3, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Info,
                arg1,
                arg2,
                arg3,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes trace log entry.
        /// </summary>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Trace(
            this IJsonLogger logger,
            Func<Func<string, ILogEntryBuilder>, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Trace,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes trace log entry.
        /// </summary>
        /// <typeparam name="T">Log entry builder argument 1 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg">Log entry builder argument 1.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Trace<T>(
            this IJsonLogger logger,
            T arg,
            Func<Func<string, ILogEntryBuilder>, T, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Trace,
                arg,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes trace log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Trace<T1, T2>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            Func<Func<string, ILogEntryBuilder>, T1, T2, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Debug,
                arg1,
                arg2,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes trace log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <typeparam name="T3">Log entry builder argument 3 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="arg3">Log entry builder argument 3.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Trace<T1, T2, T3>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            Func<Func<string, ILogEntryBuilder>, T1, T2, T3, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Debug,
                arg1,
                arg2,
                arg3,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes warning log entry.
        /// </summary>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Warning(
            this IJsonLogger logger,
            Func<Func<string, ILogEntryBuilder>, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Warning,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes warning log entry.
        /// </summary>
        /// <typeparam name="T">Log entry builder argument 1 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg">Log entry builder argument 1.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Warning<T>(
            this IJsonLogger logger,
            T arg,
            Func<Func<string, ILogEntryBuilder>, T, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Warning,
                arg,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes warning log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Warning<T1, T2>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            Func<Func<string, ILogEntryBuilder>, T1, T2, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Warning,
                arg1,
                arg2,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Writes warning log entry.
        /// </summary>
        /// <typeparam name="T1">Log entry builder argument 1 type.</typeparam>
        /// <typeparam name="T2">Log entry builder argument 2 type.</typeparam>
        /// <typeparam name="T3">Log entry builder argument 3 type.</typeparam>
        /// <param name="logger">Logger to write to.</param>
        /// <param name="arg1">Log entry builder argument 1.</param>
        /// <param name="arg2">Log entry builder argument 2.</param>
        /// <param name="arg3">Log entry builder argument 3.</param>
        /// <param name="logEntryBuilder">Log entry builder <c>delegate</c>.</param>
        /// <param name="callerFilePath">Log entry origin file path.</param>
        /// <param name="callerMemberName">Log entry origin member name.</param>
        /// <param name="callerLineNumber">Log entry origin line number.</param>
        public static void Warning<T1, T2, T3>(
            this IJsonLogger logger,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            Func<Func<string, ILogEntryBuilder>, T1, T2, T3, ILogEntryBuilder> logEntryBuilder,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            WriteLogEntry(
                logger,
                LogSeverity.Warning,
                arg1,
                arg2,
                arg3,
                logEntryBuilder,
                callerFilePath,
                callerMemberName,
                callerLineNumber);

            // ReSharper restore AssignNullToNotNullAttribute
        }
    }
#endif
}