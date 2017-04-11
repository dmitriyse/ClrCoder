// <copyright file="OperationFaultException.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Operation canceled due to some fault.
    /// </summary>
    [PublicAPI]
    public class OperationFaultException : OperationCanceledException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationFaultException"/> class.
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the error.</param>
        public OperationFaultException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationFaultException"/> class.
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the
        /// <paramref name="innerException"/> parameter is not null, the current exception is raised in a catch block that handles
        /// the inner exception.
        /// </param>
        public OperationFaultException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}