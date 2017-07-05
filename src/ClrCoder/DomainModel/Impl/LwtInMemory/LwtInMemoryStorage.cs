// <copyright file="LwtInMemoryStorage.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel.Impl.LwtInMemory
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using ObjectModel;

    using Validation;

    /// <summary>
    /// The light-weight transactional in-memory entity storage.
    /// </summary>
    /// <remarks>
    /// TODO: Implement filtering queries.
    /// </remarks>
    /// <typeparam name="TPersistence">The final persistence type.</typeparam>
    /// <typeparam name="TUnitOfWork">The final unit of work type.</typeparam>
    /// <typeparam name="TStorage">The final in-memory storage type.</typeparam>
    /// <typeparam name="TRepository">The final repository type.</typeparam>
    /// <typeparam name="TKey">The entity key type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public abstract class LwtInMemoryStorage<TPersistence, TUnitOfWork, TStorage, TRepository, TKey,
                                             TEntity>
        : PersistencePluginBase<TPersistence, TUnitOfWork>
        where TPersistence : PersistenceBase<TPersistence, TUnitOfWork>
        where TUnitOfWork : UnitOfWorkBase<TPersistence, TUnitOfWork>
        where TStorage :
        LwtInMemoryStorage<TPersistence, TUnitOfWork, TStorage, TRepository, TKey, TEntity>
        where TRepository :
        LwtInMemoryStorageRepository<TPersistence, TUnitOfWork, TStorage, TRepository, TKey, TEntity>
        where TKey : class, IEntityKey<TKey>
        where TEntity : class, IKeyed<TKey>, IDeepCloneable<TEntity>
    {
        private readonly SortedDictionary<TKey, TEntity> _entities;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="LwtInMemoryStorage{TPersistence,TUnitOfWork,TStorage,TRepository,TKey,TEntity}"/> class.
        /// </summary>
        /// <param name="persistence">The owner persistence.</param>
        /// <param name="supportedRepositoryTypes">The repository types set that can be resolved with <c>this</c> plugin.</param>
        /// <param name="keysComparer">The keys comparer.</param>
        public LwtInMemoryStorage(
            TPersistence persistence,
            [Immutable] IReadOnlySet<Type> supportedRepositoryTypes,
            IComparer<TKey> keysComparer)
            : base(persistence, supportedRepositoryTypes, false)
        {
            _entities = new SortedDictionary<TKey, TEntity>(keysComparer);

            persistence.UnitOfWorkOpened += UnitOfWorkOpened;
        }

        /// <summary>
        /// Creates or updates entity.
        /// </summary>
        /// <remarks>
        /// Update or Create operations are performed in the critical section, so left any processing out-of those functions.
        /// </remarks>
        /// <param name="entityKey">The entity key.</param>
        /// <param name="updateAction">The update action. It will be called when entity with the specified key exists.</param>
        /// <param name="createFunc">The create function. It will be called when entity with the specified key not exists.</param>
        public void CreateOrUpdateEntity(TKey entityKey, Action<TEntity> updateAction, Func<TEntity> createFunc)
        {
            VxArgs.NotNull(entityKey, nameof(entityKey));
            VxArgs.NotNull(updateAction, nameof(updateAction));
            VxArgs.NotNull(createFunc, nameof(createFunc));

            lock (_entities)
            {
                if (_entities.TryGetValue(entityKey, out var entity))
                {
                    updateAction(entity);
                }
                else
                {
                    TEntity newEntity = createFunc();
                    Critical.Assert(
                        newEntity.Key.Equals(entityKey),
                        "Created entity should have the same key as was queried to the CreateOrUpdateEntity method.");
                }
            }
        }

        /// <inheritdoc/>
        [CanBeNull]
        public override TR ResolveRepository<TR>(TUnitOfWork uow)
        {
            if (!SupportedRepositoryTypes.Contains(typeof(TR)))
            {
                return null;
            }

            lock (uow.DisposeSyncRoot)
            {
                var repository = uow.GetPluginEntry(this) as TRepository;

                Critical.Assert(repository != null, "Repository should be registered on unit of work init.");

                // ReSharper disable once SuspiciousTypeConversion.Global
                // ReSharper disable once PossibleInvalidCastException
                return (TR)(object)repository;
            }
        }

        /// <summary>
        /// Tries to delete entity with the specified key.
        /// </summary>
        /// <param name="entityKey">The key of the entity to delete.</param>
        /// <returns>true, if the entity was deleted.</returns>
        public bool TryDeleteEntity(TKey entityKey)
        {
            lock (_entities)
            {
                return _entities.Remove(entityKey);
            }
        }

        /// <summary>
        /// Ties to select entity by the specified key.
        /// </summary>
        /// <param name="key">The key of the entity to select.</param>
        /// <param name="entity">The found entity snapshot or null.</param>
        /// <returns>true - if entity was found, false otherwise.</returns>
        public bool TrySelectEntity(TKey key, [CanBeNull] out TEntity entity)
        {
            lock (_entities)
            {
                if (_entities.TryGetValue(key, out var innerEntity))
                {
                    entity = innerEntity.Clone();
                    return true;
                }

                entity = null;
                return false;
            }
        }

        /// <inheritdoc/>
        protected override async Task AsyncDispose()
        {
            // Do nothing.
        }

        /// <summary>
        /// Creates repository on transaction opening.
        /// </summary>
        /// <returns>Created repository.</returns>
        protected abstract TRepository CreateRepository();

        private void UnitOfWorkOpened(TUnitOfWork uow)
        {
            if (uow == null)
            {
                throw new ArgumentNullException(nameof(uow));
            }

            // We are under lock here.
            Debug.Assert(Monitor.IsEntered(uow.DisposeSyncRoot), "Repository creation allowed only under lock");

            uow.SetPluginEntry(this, CreateRepository());
        }
    }
}