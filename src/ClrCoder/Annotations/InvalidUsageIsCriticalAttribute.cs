// <copyright file="InvalidUsageIsCriticalAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Annotations
{
    using System;

    /// <summary>
    /// Specifies that target will give unprocessable exception or crash in a case of invalid usage.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Method
        | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface)]
    public class InvalidUsageIsCriticalAttribute : Attribute
    {
    }
}