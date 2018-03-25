// <copyright file="MonitorSyncObject.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading;

    /// <summary>
    /// The <see cref="ISyncObject"/> implementation for the <see cref="Monitor"/>.
    /// </summary>
    public struct MonitorSyncObject : ISyncObject
    {
        private readonly object _syncRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorSyncObject"/> struct.
        /// </summary>
        /// <param name="syncRoot">The sync root.</param>
        public MonitorSyncObject(object syncRoot)
        {
            _syncRoot = syncRoot ?? throw new ArgumentNullException(nameof(syncRoot));
        }

        /// <inheritdoc/>
        public void Enter()
        {
            Monitor.Enter(_syncRoot);
        }

        /// <inheritdoc/>
        public void Exit()
        {
            Monitor.Exit(_syncRoot);
        }
    }
}