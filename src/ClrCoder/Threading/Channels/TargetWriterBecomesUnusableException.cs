// <copyright file="TargetWriterBecomesUnusableException.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Error when copying target stops receiving items.
    /// </summary>
    public class TargetWriterBecomesUnusableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetWriterBecomesUnusableException"/> class.
        /// </summary>
        public TargetWriterBecomesUnusableException()
            : base("The target writer can get no more items.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetWriterBecomesUnusableException"/> class with a specified error
        /// message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public TargetWriterBecomesUnusableException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetWriterBecomesUnusableException"/> class with a specified error
        /// message and inner
        /// exception.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="innerException">Inner error.</param>
        public TargetWriterBecomesUnusableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NETSTANDARD2_0

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetWriterBecomesUnusableExceptionException"/> class with serialization
        /// data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being
        /// thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or
        /// destination.
        /// </param>
        public TargetWriterBecomesUnusableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

#endif
    }
}