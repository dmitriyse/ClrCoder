// <copyright file="SyncFreeObject.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    /// <summary>
    /// The <see cref="ISyncObject"/> implementation stub with no any sync.
    /// </summary>
    public struct SyncFreeObject : ISyncObject
    {
        /// <inheritdoc/>
        public void Enter()
        {
        }

        /// <inheritdoc/>
        public void Exit()
        {
        }
    }
}