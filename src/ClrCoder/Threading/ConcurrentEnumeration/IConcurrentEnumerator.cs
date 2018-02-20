// <copyright file="IConcurrentEnumerator.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;

    /// <summary>
    /// The enumerator that allows to gather items concurrently.
    /// </summary>
    /// <typeparam name="T">The type of an item.</typeparam>
    public interface IConcurrentEnumerator<T>: IDisposable
    {
        /// <summary>
        /// Gets the next item, if enumeration is not being finished.
        /// </summary>
        /// <param name="item">The next enumerated item.</param>
        /// <returns><see langword="true"/>, if the next item is obtained, <see langword="false"/> if enumeration has been finished.</returns>
        bool TryGetNext(out T item);
    }
}