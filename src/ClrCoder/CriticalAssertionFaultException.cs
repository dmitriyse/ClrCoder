// <copyright file="CriticalAssertionFaultException.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
#if NETSTANDARD2_0
    using System.Runtime.Serialization;
#endif

    using JetBrains.Annotations;

    /// <summary>
    /// Error produced by <see cref="Critical.Assert"/> failure.
    /// </summary>
    public class CriticalAssertionFaultException : CriticalException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalAssertionFaultException"/> class.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="detailMessage"><c>Critical</c> assertion failure details.</param>
        /// <param name="callerInfo">Caller info.</param>
        public CriticalAssertionFaultException(string message, [CanBeNull] string detailMessage, CallerInfo callerInfo)
            : base(message)
        {
            DetailMessage = detailMessage;
            CallerInfo = callerInfo;
        }

#if NETSTANDARD2_0

        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalAssertionFaultException"/> class with serialization data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being
        /// thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or
        /// destination.
        /// </param>
        public CriticalAssertionFaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

#endif

        /// <summary>
        /// <c>Critical</c> assertion failure details.
        /// </summary>
        [CanBeNull]
        public string DetailMessage { get; }

        /// <summary>
        /// Caller info.
        /// </summary>
        public CallerInfo CallerInfo { get; }
    }
}