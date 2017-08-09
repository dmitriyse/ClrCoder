// <copyright file="IUnitOfWork.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel
{
    using System.Threading;
    using System.Threading.Tasks;

    using Threading;

    /// <summary>
    /// Unit of work abstraction.
    /// </summary>
    public interface IUnitOfWork : IAbortableAsyncDisposable
    {
        /// <summary>
        /// Use <c>this</c> method only if you want to access some entities after db commit.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        Task Commit();
    }

    /// <summary>
    /// Unit of work with one repository.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    public interface IUnitOfWork<out TR1> : IUnitOfWork
        where TR1 : class, IRepository
    {
        /// <summary>
        /// Repository 1.
        /// </summary>
        TR1 R1 { get; }
    }

    /// <summary>
    /// Unit of work with two repositories.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    public interface IUnitOfWork<out TR1, out TR2> : IUnitOfWork
        where TR1 : class, IRepository
        where TR2 : class, IRepository
    {
        /// <summary>
        /// Repository 1.
        /// </summary>
        TR1 R1 { get; }

        /// <summary>
        /// Repository 2.
        /// </summary>
        TR2 R2 { get; }
    }

    /// <summary>
    /// Unit of work with two repositories.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    /// <typeparam name="TR3">Type of repository 3.</typeparam>
    public interface IUnitOfWork<out TR1, out TR2, out TR3> : IUnitOfWork
        where TR1 : class, IRepository
        where TR2 : class, IRepository
        where TR3 : class, IRepository
    {
        /// <summary>
        /// Repository 1.
        /// </summary>
        TR1 R1 { get; }

        /// <summary>
        /// Repository 2.
        /// </summary>
        TR2 R2 { get; }

        /// <summary>
        /// Repository 3.
        /// </summary>
        TR3 R3 { get; }
    }

    /// <summary>
    /// Unit of work with two repositories.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    /// <typeparam name="TR3">Type of repository 3.</typeparam>
    /// <typeparam name="TR4">Type of repository 4.</typeparam>
    public interface IUnitOfWork<out TR1, out TR2, out TR3, out TR4> : IUnitOfWork
        where TR1 : class, IRepository
        where TR2 : class, IRepository
        where TR3 : class, IRepository
        where TR4 : class, IRepository
    {
        /// <summary>
        /// Repository 1.
        /// </summary>
        TR1 R1 { get; }

        /// <summary>
        /// Repository 2.
        /// </summary>
        TR2 R2 { get; }

        /// <summary>
        /// Repository 3.
        /// </summary>
        TR3 R3 { get; }

        /// <summary>
        /// Repository 4.
        /// </summary>
        TR4 R4 { get; }
    }
}