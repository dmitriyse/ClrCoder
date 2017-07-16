// <copyright file="ClientNodeAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster.IO
{
    using System;

    /// <summary>
    /// Marks parameter that it contains client cluster node key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ClientNodeAttribute : Attribute
    {
    }
}