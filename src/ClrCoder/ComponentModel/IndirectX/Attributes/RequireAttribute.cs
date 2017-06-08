// <copyright file="RequireAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    /// <summary>
    /// Informs container that component implicitly requires dependency.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public class RequireAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequireAttribute"/> class.
        /// </summary>
        /// <param name="type">Type of required dependency.</param>
        public RequireAttribute(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Type of asked dependency.
        /// </summary>
        public Type Type { get; }
    }
}