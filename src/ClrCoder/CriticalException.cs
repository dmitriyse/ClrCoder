// <copyright file="CriticalException.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
#if NET46
    using System.Runtime.Serialization;
#endif

    using JetBrains.Annotations;

    /// <summary>
    /// Library generated non processable exception.
    /// </summary>
    [PublicAPI]
    public class CriticalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalException"/> class.
        /// </summary>
        public CriticalException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public CriticalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalException"/> class with a specified error message and inner
        /// exception.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="innerException">Inner error.</param>
        public CriticalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET46

        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalException"/> class with serialization data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being
        /// thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or
        /// destination.
        /// </param>
        public CriticalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

#endif
    }
}