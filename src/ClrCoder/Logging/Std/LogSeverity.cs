// <copyright file="LogSeverity.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
    using JetBrains.Annotations;

    /// <summary>
    /// Standard log severities.
    /// </summary>
    [PublicAPI]
    public enum LogSeverity
    {
        /// <summary>
        /// Critical error, usually not preventing application crash.
        /// </summary>
        Critical = 100,

        /// <summary>
        /// Some regular error.
        /// </summary>
        Error = 200,

        /// <summary>
        /// Some issue.
        /// </summary>
        Warning = 300,

        /// <summary>
        /// Important information.
        /// </summary>
        Info = 400,

        /// <summary>
        /// Detailed information.
        /// </summary>
        Trace = 500,

        /// <summary>
        /// Overkill detailed information required to debug.
        /// </summary>
        Debug = 600,
    }
}