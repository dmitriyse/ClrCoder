// <copyright file="LwtInMemoryStorageRepository.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel.Impl.LwtInMemory
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using ObjectModel;

    using Validation;

    /// <summary>
    /// Light-weight transactional in-memory storage repository.
    /// </summary>
    /// <typeparam name="TPersistence">The final persistence type.</typeparam>
    /// <typeparam name="TUnitOfWork">The final unit of work type.</typeparam>
    /// <typeparam name="TStorage">The final in-memory storage type.</typeparam>
    /// <typeparam name="TRepository">The final repository type.</typeparam>
    /// <typeparam name="TKey">The entity key type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    [PublicAPI]
    public class LwtInMemoryStorageRepository<TPersistence, TUnitOfWork, TStorage, TRepository, TKey,
                                              TEntity> : IDisposablePluginEntry<TPersistence, TUnitOfWork>
        where TPersistence : PersistenceBase<TPersistence, TUnitOfWork>
        where TUnitOfWork : UnitOfWorkBase<TPersistence, TUnitOfWork>
        where TStorage :
        LwtInMemoryStorage<TPersistence, TUnitOfWork, TStorage, TRepository, TKey, TEntity>
        where TRepository :
        LwtInMemoryStorageRepository<TPersistence, TUnitOfWork, TStorage, TRepository, TKey, TEntity>
        where TKey : class, IEntityKey<TKey>
        where TEntity : class, IKeyed<TKey>, IDeepCloneable<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="LwtInMemoryStorageRepository{TPersistence,TUnitOfWork,TStorage,TRepository,TKey,TEntity}"/>
        /// class.
        /// </summary>
        /// <param name="storage">Storage that is serviced by <c>this</c> repository.</param>
        public LwtInMemoryStorageRepository(TStorage storage)
        {
            VxArgs.NotNull(storage, nameof(storage));
            Storage = storage;
        }

        /// <summary>
        /// Storage that is serviced by <c>this</c> repository.
        /// </summary>
        public TStorage Storage { get; }

        /// <inheritdoc/>
        public async Task EnsureDisposed(
            PersistencePluginBase<TPersistence, TUnitOfWork> plugin,
            TUnitOfWork uow,
            bool isDiscardRequested)
        {
            // Do nothing.
        }
    }
#endif
}