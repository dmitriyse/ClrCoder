// <copyright file="TypeChoiceAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Annotations
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Specifies that the target can be one of the following types.
    /// </summary>
    /// <remarks>
    /// Probably in future version of C# we will have "Union types" see https://github.com/dotnet/csharplang/issues/113 . But
    /// currently we can use annotation (this attribute) or some sort of "MayBe" wrappers.
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Class
        | AttributeTargets.Interface
        | AttributeTargets.ReturnValue
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Property
        | AttributeTargets.Parameter,
        AllowMultiple = true)]
    [PublicAPI]
    public class TypeChoiceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeChoiceAttribute"/> class.
        /// </summary>
        /// <param name="types">Expected types.</param>
        public TypeChoiceAttribute(params Type[] types)
        {
            Types = types;
        }

        /// <summary>
        /// Expected types.
        /// </summary>
        public Type[] Types { get; }
    }
}