// <copyright file="IDisposablePluginEntry.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel.Impl
{
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System.Threading.Tasks;

    /// <summary>
    /// Marks persistence plugin data associated with UoW as disposable.
    /// </summary>
    /// <typeparam name="TPersistence">Final type of persistence.</typeparam>
    /// <typeparam name="TUnitOfWork">Final type of unit of work.</typeparam>
    public interface IDisposablePluginEntry<TPersistence, TUnitOfWork>
        where TPersistence : PersistenceBase<TPersistence, TUnitOfWork>
        where TUnitOfWork : UnitOfWorkBase<TPersistence, TUnitOfWork>
    {
        /// <summary>
        /// Ensures that <c>plugin</c> data disposed.
        /// </summary>
        /// <param name="plugin">Plugin that handles <c>this</c> data.</param>
        /// <param name="uow">Unit of where where data associated.</param>
        /// <param name="isDiscardRequested">Shows that dispose is performs with changes discarding.</param>
        /// <returns>Async execution TPL task.</returns>
        Task EnsureDisposed(
            PersistencePluginBase<TPersistence, TUnitOfWork> plugin,
            TUnitOfWork uow,
            bool isDiscardRequested);
    }
#endif
}