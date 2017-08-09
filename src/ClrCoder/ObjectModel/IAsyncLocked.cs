// <copyright file="IAsyncLocked.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ObjectModel
{
    using System.Threading;

    using Threading;

    /// <summary>
    /// Lock on some <c>object</c> that releases asynchronously.
    /// </summary>
    /// <typeparam name="T">Type of <c>object</c>.</typeparam>
    public interface IAsyncLocked<out T> : IAsyncDisposable
    {
        /// <summary>
        /// Locked <c>object</c>.
        /// </summary>
        T Target { get; }
    }
}