// <copyright file="IxResolveException.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

#if NET46
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Base class for all IndirectX processable exceptions.
    /// </summary>
    [PublicAPI]
    public class IxResolveException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxResolveException"/> class.
        /// </summary>
        public IxResolveException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IxResolveException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public IxResolveException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IxResolveException"/> class with a specified error message and inner
        /// exception.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="innerException">Inner error.</param>
        public IxResolveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET46

/// <summary>
/// Initializes a new instance of the <see cref="IxResolveException"/> class with serialization data.
/// </summary>
/// <param name="info">
/// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being
/// thrown.
/// </param>
/// <param name="context">
/// The <see cref="StreamingContext"/> that contains contextual information about the source or
/// destination.
/// </param>
        public IxResolveException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

#endif
    }
}