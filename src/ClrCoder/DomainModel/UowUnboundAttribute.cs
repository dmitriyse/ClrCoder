// <copyright file="UowUnboundAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Shows that a value received from a tagged property can be used out of UoW.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [PublicAPI]
    public class UowUnboundAttribute : Attribute
    {
    }
}