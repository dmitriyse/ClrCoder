// <copyright file="ImmutableAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    /// <summary>
    /// Annotate that target is/should be immutable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.ReturnValue | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter  )]
    public class ImmutableAttribute: Attribute
    {
    }
}