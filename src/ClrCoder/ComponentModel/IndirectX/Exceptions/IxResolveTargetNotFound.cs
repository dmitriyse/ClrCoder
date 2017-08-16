// <copyright file="IxResolveTargetNotFound.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

#if NETSTANDARD2_0
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Queried target object not found or some plugins missed.
    /// </summary>
    [PublicAPI]
    public class IxResolveTargetNotFound : IxResolveException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxResolveTargetNotFound"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="identifier">Resolve target identifier.</param>
        public IxResolveTargetNotFound(string message, IxIdentifier identifier)
            : base(message)
        {
            if (identifier == default(IxIdentifier))
            {
                throw new ArgumentException("Identifier should not be empty.", nameof(identifier));
            }

            Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IxResolveTargetNotFound"/> class with a specified error message and inner
        /// exception.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="identifier">Resolve target identifier.</param>
        /// <param name="innerException">Inner error.</param>
        public IxResolveTargetNotFound(string message, IxIdentifier identifier, Exception innerException)
            : base(message, innerException)
        {
            if (identifier == default(IxIdentifier))
            {
                throw new ArgumentException("Identifier should not be empty.", nameof(identifier));
            }

            Identifier = identifier;
        }

        /// <summary>
        /// Resolve target identifier.
        /// </summary>
        public IxIdentifier Identifier { get; }

#if NETSTANDARD2_0

/// <summary>
/// Initializes a new instance of the <see cref="IxResolveTargetNotFound"/> class with serialization data.
/// </summary>
/// <param name="info">
/// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being
/// thrown.
/// </param>
/// <param name="context">
/// The <see cref="StreamingContext"/> that contains contextual information about the source or
/// destination.
/// </param>
        public IxResolveTargetNotFound(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

#endif
    }
}