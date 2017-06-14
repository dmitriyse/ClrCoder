// <copyright file="AsyncInitReadonlyAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Annotations
{
    using System;

    /// <summary>
    /// Marks property or attribute as asynchronously initialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AsyncInitReadonlyAttribute : Attribute
    {
    }
}