// <copyright file="ImmutableAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    using JetBrains.Annotations;

    /// <summary>
    /// Annotate that target should be shallow immutable.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.ReturnValue | AttributeTargets.Interface
        | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    [PublicAPI]
    public class ImmutableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of part/aspect of an instance that is/should be shallow immutable.</param>
        public ImmutableAttribute(Type type = null)
        {
            Type = type ?? typeof(object);
        }

        /// <summary>
        /// The type of part/aspect of an instance that is/should be shallow immutable.
        /// </summary>
        public Type Type { get; }
    }
}