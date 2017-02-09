// <copyright file="DelegateLogEntryBuilderForLogEntry.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Log entry builder based on <c>delegate</c> that works on <see cref="LogEntry"/>.
    /// </summary>
    [PublicAPI]
    public class DelegateLogEntryBuilderForLogEntry : ILogEntryBuilder
    {
        [CanBeNull]
        private readonly ILogEntryBuilder _innerBuilder;

        [NotNull]
        private readonly Func<LogEntry, LogEntry> _buildDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogEntryBuilderForLogEntry"/> class.
        /// </summary>
        /// <param name="innerBuilder">Previous build block in a fluent chain.</param>
        /// <param name="buildDelegate">Delegate that performs build.</param>
        public DelegateLogEntryBuilderForLogEntry(
            [NotNull] ILogEntryBuilder innerBuilder,
            [NotNull] Func<LogEntry, LogEntry> buildDelegate)
        {
            if (innerBuilder == null)
            {
                throw new ArgumentNullException(nameof(innerBuilder));
            }

            if (buildDelegate == null)
            {
                throw new ArgumentNullException(nameof(buildDelegate));
            }

            _innerBuilder = innerBuilder;
            _buildDelegate = buildDelegate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogEntryBuilderForLogEntry"/> class.
        /// </summary>
        /// <param name="buildDelegate">Delegates that builds entry.</param>
        public DelegateLogEntryBuilderForLogEntry(
            [NotNull] Func<LogEntry, LogEntry> buildDelegate)
        {
            if (buildDelegate == null)
            {
                throw new ArgumentNullException(nameof(buildDelegate));
            }

            _buildDelegate = buildDelegate;
        }

        /// <inheritdoc/>
        public object Build(object entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            object currentEntry = _innerBuilder?.Build(entry) ?? entry;
            LogEntry logEntry = LoggerUtils.NormalizeToLogEntry(currentEntry);
            return _buildDelegate(logEntry);
        }
    }
}