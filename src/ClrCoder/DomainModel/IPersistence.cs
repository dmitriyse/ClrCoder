// <copyright file="IPersistence.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel
{
    /// <summary>
    /// Provides persistence with 1 repository.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    public interface IPersistence<out TR1>
        where TR1 : class, IRepository
    {
        /// <summary>
        /// Opens new unit of work.
        /// </summary>
        /// <returns>New unit of work.</returns>
        IUnitOfWork<TR1> OpenUnitOfWork();
    }

    /// <summary>
    /// Provides persistence with 2 repositories.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    public interface IPersistence<out TR1, out TR2>
        where TR1 : class, IRepository
        where TR2 : class, IRepository
    {
        /// <summary>
        /// Opens new unit of work.
        /// </summary>
        /// <returns>New unit of work.</returns>
        IUnitOfWork<TR1, TR2> OpenUnitOfWork();
    }

    /// <summary>
    /// Provides persistence with 3 repositories.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    /// <typeparam name="TR3">Type of repository 3.</typeparam>
    public interface IPersistence<out TR1, out TR2, out TR3>
        where TR1 : class, IRepository
        where TR2 : class, IRepository
        where TR3 : class, IRepository
    {
        /// <summary>
        /// Opens new unit of work.
        /// </summary>
        /// <returns>New unit of work.</returns>
        IUnitOfWork<TR1, TR2, TR3> OpenUnitOfWork();
    }

    /// <summary>
    /// Provides persistence with 3 repositories.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    /// <typeparam name="TR3">Type of repository 3.</typeparam>
    /// <typeparam name="TR4">Type of repository 4.</typeparam>
    public interface IPersistence<out TR1, out TR2, out TR3, out TR4>
        where TR1 : class, IRepository
        where TR2 : class, IRepository
        where TR3 : class, IRepository
        where TR4 : class, IRepository
    {
        /// <summary>
        /// Opens new unit of work.
        /// </summary>
        /// <returns>New unit of work.</returns>
        IUnitOfWork<TR1, TR2, TR3, TR4> OpenUnitOfWork();
    }
}