// <copyright file="VxValidationException.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Validation
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Generic validation exception from the ClrCoder.Validation library.
    /// </summary>
    [PublicAPI]
    public class VxValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VxValidationException"/> class.
        /// </summary>
        /// <param name="callerFilePath">Calling member file path.</param>
        /// <param name="callerLineNumber">Calling member line number.</param>
        /// <param name="args">Method arguments that was passed to failed method.</param>
        public VxValidationException(string callerFilePath, int callerLineNumber, params object[] args)
        {
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
            Args = args;
        }

        /// <summary>
        /// Method arguments that was passed to failed method.
        /// </summary>
        public object[] Args { get; set; }

        /// <summary>
        /// Calling member file path.
        /// </summary>
        public string CallerFilePath { get; set; }

        /// <summary>
        /// Calling member line number.
        /// </summary>
        public int CallerLineNumber { get; set; }
    }
}