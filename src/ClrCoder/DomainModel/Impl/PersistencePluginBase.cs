// <copyright file="PersistencePluginBase.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel.Impl
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Threading;

    /// <summary>
    /// Base implementation of plugin for persistence.
    /// </summary>
    /// <typeparam name="TPersistence">Final persistence type.</typeparam>
    /// <typeparam name="TUnitOfWork">Final unit of work type.</typeparam>
    public abstract class PersistencePluginBase<TPersistence, TUnitOfWork> : AsyncDisposableBase
        where TPersistence : PersistenceBase<TPersistence, TUnitOfWork>
        where TUnitOfWork : UnitOfWorkBase<TPersistence, TUnitOfWork>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistencePluginBase{TPersistence,TUnitOfWork}"/> class.
        /// </summary>
        /// <param name="persistence">Owner persistence.</param>
        /// <param name="supportedRepositoryTypes">Provides repository types that can be resolved with <c>this</c> plugin.</param>
        /// <param name="areNonDeclaredRepositoryTypesSupported">
        /// Shows that plugin can resolve non declared repository types in the
        /// <see cref="SupportedRepositoryTypes"/>.
        /// </param>
        protected PersistencePluginBase(
            TPersistence persistence,
            [Immutable] IReadOnlyCollection<Type> supportedRepositoryTypes,
            bool areNonDeclaredRepositoryTypesSupported)
        {
            if (persistence == null)
            {
                throw new ArgumentNullException(nameof(persistence));
            }

            Persistence = persistence;
            SupportedRepositoryTypes = supportedRepositoryTypes;
            AreNonDeclaredRepositoryTypesSupported = areNonDeclaredRepositoryTypesSupported;
            persistence.RegisterPlugin(this);
        }

        /// <summary>
        /// Provides repository types that can be resolved with <c>this</c> plugin.
        /// </summary>
        public IReadOnlyCollection<Type> SupportedRepositoryTypes { get; }

        /// <summary>
        /// Shows that plugin can resolve non declared repository types in the <see cref="SupportedRepositoryTypes"/>.
        /// </summary>
        public bool AreNonDeclaredRepositoryTypesSupported { get; }

        /// <summary>
        /// Owner persistence.
        /// </summary>
        public TPersistence Persistence { get; }

        /// <summary>
        /// Resolves repository by type.
        /// </summary>
        /// <typeparam name="TRepository">Type of repository to resolve.</typeparam>
        /// <param name="uow">Unit for which <c>this</c> repository should be resolved.</param>
        /// <returns>Queried repository. Returns <c>null</c>, if asked repository cannot be resolved by <c>this</c> plugin.</returns>
        [CanBeNull]
        public abstract TRepository ResolveRepository<TRepository>(TUnitOfWork uow)
            where TRepository : class, IRepository;
    }
}