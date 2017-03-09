// <copyright file="IPersistenceImpl.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel
{
    /// <summary>
    /// Base contract for persistence implementation.
    /// </summary>
    public interface IPersistenceImpl
    {
        /// <summary>
        /// Opens Unit of Work. TODO: Add debug information on UoW open, when Persistence on Disposing should check that all UoW
        /// finalized.
        /// </summary>
        /// <returns>New Unit of work.</returns>
        IUnitOfWorkImpl OpenUnitOfWork();
    }
}