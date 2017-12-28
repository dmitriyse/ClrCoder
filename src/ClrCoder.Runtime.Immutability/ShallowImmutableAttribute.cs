﻿// <copyright file="ShallowImmutableAttribute.cs" company="ClrCoder project">
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
        | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true)]
    [PublicAPI]
    public class ShallowImmutableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShallowImmutableAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of part/aspect of an instance that is/should be shallow immutable.</param>
        public ShallowImmutableAttribute(Type type = null)
        {
            Type = type;
        }

        /// <summary>
        /// The type of part/aspect of an instance that is/should be shallow immutable.
        /// </summary>
        public Type Type { get; }
    }
}