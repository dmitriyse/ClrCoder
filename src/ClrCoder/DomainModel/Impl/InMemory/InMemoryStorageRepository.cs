// <copyright file="InMemoryStorageRepository.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>


#pragma warning disable 1998

namespace ClrCoder.DomainModel.Impl.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;

    using DomainModel.InMemory;

    using JetBrains.Annotations;

    using ObjectModel;

    /// <summary>
    /// Repository for in-memory storage.
    /// </summary>
    /// <typeparam name="TPersistence">Final persistence type.</typeparam>
    /// <typeparam name="TUnitOfWork">Final unit of work type.</typeparam>
    /// <typeparam name="TStorage">Final in-memory storage type.</typeparam>
    /// <typeparam name="TRepository">Final repository type.</typeparam>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TEntity">entity type.</typeparam>
    [PublicAPI]
    public class InMemoryStorageRepository<TPersistence, TUnitOfWork, TStorage, TRepository, TKey,
                                           TEntity> : IDisposablePluginEntry<TPersistence, TUnitOfWork>
        where TPersistence : PersistenceBase<TPersistence, TUnitOfWork>
        where TUnitOfWork : UnitOfWorkBase<TPersistence, TUnitOfWork>
        where TStorage :
        InMemoryStorage<TPersistence, TUnitOfWork, TStorage, TRepository, TKey, TEntity>
        where TRepository :
        InMemoryStorageRepository<TPersistence, TUnitOfWork, TStorage, TRepository, TKey, TEntity>
        where TKey : IEntityKey<TKey>
        where TEntity : class, IKeyed<TKey>, IDeepCloneable<TEntity>
    {
        private readonly ImmutableDictionary<TKey, TEntity> _dataSnapshot;

        private readonly Dictionary<TKey, InMemoryStorageRepositoryEntry<TKey, TEntity>> _localData =
            new Dictionary<TKey, InMemoryStorageRepositoryEntry<TKey, TEntity>>();

        private int _count;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="InMemoryStorageRepository{TPersistence,TUnitOfWork,TStorage,TRepository,TKey,TEntity}"/>
        /// class.
        /// </summary>
        /// <param name="storage">Storage that is serviced by <c>this</c> repository.</param>
        /// <param name="dataSnapshot">Current state of the storage.</param>
        public InMemoryStorageRepository(TStorage storage, ImmutableDictionary<TKey, TEntity> dataSnapshot)
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            if (dataSnapshot == null)
            {
                throw new ArgumentNullException(nameof(dataSnapshot));
            }

            Storage = storage;
            _dataSnapshot = dataSnapshot;
            _count = _dataSnapshot.Count;
        }

        /// <summary>
        /// Storage that is serviced by <c>this</c> repository.
        /// </summary>
        public TStorage Storage { get; }

        /// <summary>
        /// Current count in the storage visible by uow.
        /// </summary>
        protected int Count
        {
            get
            {
                lock (_localData)
                {
                    return _count;
                }
            }
        }

        /// <inheritdoc/>
        public async Task EnsureDisposed(
            PersistencePluginBase<TPersistence, TUnitOfWork> plugin,
            TUnitOfWork uow,
            bool isDiscardRequested)
        {
            if (!isDiscardRequested)
            {
                Storage.MergeChanges(_localData, _dataSnapshot);
            }
        }

        /// <summary>
        /// Adds new <c>entity</c>.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        /// <exception cref="ArgumentException">Entity with the same key already exists.</exception>
        protected void Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var raiseError = false;

            lock (_localData)
            {
                InMemoryStorageRepositoryEntry<TKey, TEntity> entry;
                if (_localData.TryGetValue(entity.Key, out entry))
                {
                    if (entry.State == InMemoryStorageEntityState.Removed)
                    {
                        entry.Entity = entity;
                        entry.State = InMemoryStorageEntityState.Inserted;
                        _count++;
                    }
                    else
                    {
                        raiseError = true;
                    }
                }
                else
                {
                    TEntity existingEntity;
                    if (_dataSnapshot.TryGetValue(entity.Key, out existingEntity))
                    {
                        raiseError = true;
                    }
                    else
                    {
                        _count++;
                        _localData.Add(
                            entity.Key,
                            new InMemoryStorageRepositoryEntry<TKey, TEntity>(entity)
                                {
                                    State = InMemoryStorageEntityState.Inserted
                                });
                    }
                }
            }

            if (raiseError)
            {
                throw new ArgumentException("Entity with the same key already exists.");
            }
        }

        /// <summary>
        /// Verifies that UoW contains entities with the specified <c>key</c>.
        /// </summary>
        /// <param name="key">Key to verify existence.</param>
        /// <returns><see langword="true"/>, if UoW contains entity with the specified key.</returns>
        protected bool Contains(TKey key)
        {
            bool contains;

            lock (_localData)
            {
                InMemoryStorageRepositoryEntry<TKey, TEntity> entry;

                if (_localData.TryGetValue(key, out entry))
                {
                    contains = entry.State != InMemoryStorageEntityState.Removed;
                }
                else
                {
                    contains = _dataSnapshot.ContainsKey(key);
                }
            }

            return contains;
        }

        /// <summary>
        /// Selects entities with editable copies creation.
        /// </summary>
        /// <returns>Entities enumeration.</returns>
        protected IEnumerable<TEntity> GetAll()
        {
            lock (_localData)
            {
                foreach (KeyValuePair<TKey, InMemoryStorageRepositoryEntry<TKey, TEntity>> kvp in _localData)
                {
                    if (kvp.Value.State != InMemoryStorageEntityState.Removed)
                    {
                        yield return kvp.Value.Entity;
                    }
                }

                foreach (KeyValuePair<TKey, TEntity> kvp in _dataSnapshot)
                {
                    InMemoryStorageRepositoryEntry<TKey, TEntity> entry;

                    if (_localData.TryGetValue(kvp.Key, out entry))
                    {
                        if (entry.State != InMemoryStorageEntityState.Removed)
                        {
                            yield return entry.Entity;
                        }
                    }
                    else
                    {
                        var clone = kvp.Value.Clone();
                        _localData.Add(
                            kvp.Key,
                            new InMemoryStorageRepositoryEntry<TKey, TEntity>(clone)
                                {
                                    State = InMemoryStorageEntityState.Modified
                                });

                        yield return clone;
                    }
                }
            }
        }

        /// <summary>
        /// Selects entities but avoid to create editable copies.
        /// </summary>
        /// <returns>Entities enumeration.</returns>
        protected IEnumerable<TEntity> GetAllWithNoTracking()
        {
            lock (_localData)
            {
                foreach (KeyValuePair<TKey, InMemoryStorageRepositoryEntry<TKey, TEntity>> kvp in _localData)
                {
                    if (kvp.Value.State != InMemoryStorageEntityState.Removed)
                    {
                        yield return kvp.Value.Entity;
                    }
                }

                foreach (KeyValuePair<TKey, TEntity> kvp in _dataSnapshot)
                {
                    InMemoryStorageRepositoryEntry<TKey, TEntity> entry;

                    if (_localData.TryGetValue(kvp.Key, out entry))
                    {
                        if (entry.State != InMemoryStorageEntityState.Removed)
                        {
                            yield return entry.Entity;
                        }
                    }
                    else
                    {
                        yield return kvp.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Removes entity by the specified <c>key</c>.
        /// </summary>
        /// <param name="key">Key of an entity to remove.</param>
        /// <returns>
        /// <see langword="true"/>, if entity was removed, <see langword="false"/> if entity with the specified <c>key</c>
        /// does not exists.
        /// </returns>
        protected bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var deleted = false;
            lock (_localData)
            {
                InMemoryStorageRepositoryEntry<TKey, TEntity> entry;

                if (_localData.TryGetValue(key, out entry))
                {
                    if (entry.State != InMemoryStorageEntityState.Removed)
                    {
                        entry.State = InMemoryStorageEntityState.Removed;
                        deleted = true;
                    }
                }
                else
                {
                    TEntity entityToRemove;

                    if (_dataSnapshot.TryGetValue(key, out entityToRemove))
                    {
                        _localData.Add(
                            key,
                            new InMemoryStorageRepositoryEntry<TKey, TEntity>(entityToRemove)
                                {
                                    State = InMemoryStorageEntityState.Removed
                                });
                        deleted = true;
                    }
                }

                if (deleted)
                {
                    _count--;
                }
            }

            return deleted;
        }

        /// <summary>
        /// Gets entity by <c>key</c>.
        /// </summary>
        /// <param name="key">Key of entity to get to.</param>
        /// <returns>Queried entity or <c>null</c>, if entity not found.</returns>
        [CanBeNull]
        protected TEntity SelectByKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            TEntity result = null;
            lock (_localData)
            {
                InMemoryStorageRepositoryEntry<TKey, TEntity> entry;
                if (_localData.TryGetValue(key, out entry))
                {
                    if (entry.State != InMemoryStorageEntityState.Removed)
                    {
                        result = entry.Entity;
                    }
                }
                else
                {
                    TEntity immutableEntity;
                    if (_dataSnapshot.TryGetValue(key, out immutableEntity))
                    {
                        result = immutableEntity.Clone();

                        _localData.Add(
                            key,
                            new InMemoryStorageRepositoryEntry<TKey, TEntity>(result)
                                {
                                    State = InMemoryStorageEntityState.Modified
                                });
                    }
                }
            }

            return result;
        }
    }
}