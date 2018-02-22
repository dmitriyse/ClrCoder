// <copyright file="InMemoryStorageEntityState.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel.Impl.InMemory
{
    /// <summary>
    /// In-memory storage entity state.
    /// </summary>
    public enum InMemoryStorageEntityState
    {
        /// <summary>
        /// Entity was inserted.
        /// </summary>
        Inserted,

        /// <summary>
        /// Entity was modified.
        /// </summary>
        Modified,

        /// <summary>
        /// Entity wsa removed.
        /// </summary>
        Removed
    }
}