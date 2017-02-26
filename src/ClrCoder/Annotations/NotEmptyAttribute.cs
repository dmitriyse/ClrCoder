// <copyright file="NotEmptyAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Annotations
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Annotates that specified target is/should always be non empty.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(
        AttributeTargets.Class
        | AttributeTargets.Interface
        | AttributeTargets.Field
        | AttributeTargets.ReturnValue
        | AttributeTargets.Parameter)]
    public class NotEmptyAttribute : Attribute
    {
    }
}