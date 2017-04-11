// <copyright file="ComponentFaultException.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;

    /// <summary>
    /// Operation fault due to component fault (case when connection to component lost, component moved to fault state or
    /// disposed or any
    /// another long running problem with the component).
    /// </summary>
    public class ComponentFaultException : OperationFaultException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentFaultException"/> class.
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the error.</param>
        public ComponentFaultException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentFaultException"/> class.
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the
        /// <paramref name="innerException"/> parameter is not null, the current exception is raised in a catch block that handles
        /// the inner exception.
        /// </param>
        public ComponentFaultException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}