// <copyright file="ReferenceImmutableAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    using JetBrains.Annotations;

    /// <summary>
    /// Shows that target is always the same reference.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    [PublicAPI]
    public class ReferenceImmutableAttribute : Attribute
    {
    }
}