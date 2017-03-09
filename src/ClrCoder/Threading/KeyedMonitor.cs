// <copyright file="KeyedMonitor.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading;

    /// <summary>
    /// Collection of synchronization root objects accessable by arbitrary keys.
    /// </summary>
    /// <remarks>
    /// ATTENTION: It is possible that a same root sync root will be used for different keys.
    /// Random locking order with nested locks can lead to deadlock.
    /// </remarks>
    /// <typeparam name="T">Key type.</typeparam>
    public class KeyedMonitor<T>
    {
        //// TODO: Add strict checks. (double dispose for example).
        private readonly Token[] _tokens = new Token[0x1000];

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedMonitor{T}"/> class.
        /// </summary>
        public KeyedMonitor()
        {
            for (var i = 0; i < this._tokens.Length; i++)
            {
                this._tokens[i] = new Token();
            }
        }

        /// <summary>
        /// Locks a sync root that corresponds to the specified <c>key</c>.
        /// </summary>
        /// <param name="key">Target resource <c>key</c>.</param>
        /// <returns>Lock token, use <see cref="IDisposable.Dispose"/> to release lock.</returns>
        public ILockToken Lock(T key)
        {
            int tokenIndex = key.GetHashCode() & 0xFFF;
            Monitor.Enter(this._tokens[tokenIndex]);
            return this._tokens[tokenIndex];
        }

        private class Token : ILockToken
        {
            public void Dispose()
            {
                Monitor.Exit(this);
            }
        }
    }
}