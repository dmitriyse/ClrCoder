// <copyright file="IContainerLease.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel
{
    using System;

    /// <summary>
    /// <c>Container</c> node lease.
    /// </summary>
    public interface IContainerLease : IDisposable
    {
        /// <summary>
        /// Resolved <c>object</c>.
        /// </summary>
        object Object { get; set; }

        /// <summary>
        /// <c>Container</c>, that owns resloved <c>object</c>.
        /// </summary>
        IContainer ParentContainer { get; set; }
    }
}