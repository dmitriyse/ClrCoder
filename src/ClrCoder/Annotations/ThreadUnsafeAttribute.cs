// <copyright file="ThreadUnsafeAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Annotations
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Shows that member or type is non thread-safe.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(
        AttributeTargets.Class
        | AttributeTargets.Interface
        | AttributeTargets.Property
        | AttributeTargets.Method)]
    public class ThreadUnsafeAttribute : Attribute
    {
    }
}