// <copyright file="InMemoryStorage.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel.Impl.InMemory
{
#pragma warning disable 1998
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using ObjectModel;

    /// <summary>
    /// In-memory entity storage.
    /// </summary>
    /// <typeparam name="TPersistence">Final persistence type.</typeparam>
    /// <typeparam name="TUnitOfWork">Final unit of work type.</typeparam>
    /// <typeparam name="TStorage">Final in-memory storage type.</typeparam>
    /// <typeparam name="TRepository">Final repository type.</typeparam>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public abstract class InMemoryStorage<TPersistence, TUnitOfWork, TStorage, TRepository, TKey,
                                          TEntity>
        : PersistencePluginBase<TPersistence, TUnitOfWork>
        where TPersistence : PersistenceBase<TPersistence, TUnitOfWork>
        where TUnitOfWork : UnitOfWorkBase<TPersistence, TUnitOfWork>
        where TStorage :
        InMemoryStorage<TPersistence, TUnitOfWork, TStorage, TRepository, TKey, TEntity>
        where TRepository :
        InMemoryStorageRepository<TPersistence, TUnitOfWork, TStorage, TRepository, TKey, TEntity>
        where TKey : IEntityKey<TKey>
        where TEntity : class, IKeyed<TKey>, IDeepCloneable<TEntity>
    {
        private readonly IEqualityComparer<TEntity> _mergeComparer;

        private ImmutableDictionary<TKey, TEntity> _data = ImmutableDictionary<TKey, TEntity>.Empty;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="InMemoryStorage{TPersistence,TUnitOfWork,TStorage,TRepository,TKey,TEntity}"/> class.
        /// </summary>
        /// <param name="persistence">Owner persistence.</param>
        /// <param name="mergeComparer">Compares entities in merge algorithms.</param>
        /// <param name="supportedRepositoryTypes">Provides repository types that can be resolved with <c>this</c> plugin.</param>
        public InMemoryStorage(
            TPersistence persistence,
            IEqualityComparer<TEntity> mergeComparer,
            [Immutable] IReadOnlySet<Type> supportedRepositoryTypes)
            : base(persistence, supportedRepositoryTypes, false)
        {
            if (mergeComparer == null)
            {
                throw new ArgumentNullException(nameof(mergeComparer));
            }

            _mergeComparer = mergeComparer;

            persistence.UnitOfWorkOpened += UnitOfWorkOpened;
        }

        /// <inheritdoc/>
        [CanBeNull]
        public override TR ResolveRepository<TR>(TUnitOfWork uow)
        {
            if (!SupportedRepositoryTypes.Contains(typeof(TR)))
            {
                return null;
            }

            var entry = uow.GetPluginEntry(this);
            Debug.Assert(entry != null, "Snapshot reference should be registered on unit of work init.");

            if (entry is ImmutableDictionary<TKey, TEntity> dataSnapshot)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                entry = CreateRepository(dataSnapshot);
                uow.SetPluginEntry(this, entry);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            // ReSharper disable once PossibleInvalidCastException
            return (TR)entry;
        }

        /// <summary>
        /// Performs merge current data state with uow changes.
        /// </summary>
        /// <param name="localData">Uow changed data.</param>
        /// <param name="uowDataSnapshot">Snapshot of data that was captured on UoW creation.</param>
        internal void MergeChanges(
            Dictionary<TKey, InMemoryStorageRepositoryEntry<TKey, TEntity>> localData,
            ImmutableDictionary<TKey, TEntity> uowDataSnapshot)
        {
            lock (DisposeSyncRoot)
            {
                ImmutableDictionary<TKey, TEntity>.Builder newStateBuilder = _data.ToBuilder();

                foreach (InMemoryStorageRepositoryEntry<TKey, TEntity> entry in localData.Values)
                {
                    TKey key = entry.Entity.Key;
                    TEntity original;
                    uowDataSnapshot.TryGetValue(key, out original);

                    TEntity main = entry.State != InMemoryStorageEntityState.Removed ? entry.Entity : null;

                    TEntity theirs;
                    _data.TryGetValue(key, out theirs);

                    TEntity merged = original;
                    var isUpdateRequired = false;

                    if (!_mergeComparer.Equals(original, main))
                    {
                        isUpdateRequired = true;
                        merged = !_mergeComparer.Equals(original, theirs)
                                     ? MergeEntity(original, main, theirs)
                                     : main;
                    }

                    if (isUpdateRequired)
                    {
                        if (merged == null)
                        {
                            newStateBuilder.Remove(key);
                        }
                        else
                        {
                            newStateBuilder[key] = merged;
                        }
                    }
                }

                _data = newStateBuilder.ToImmutable();
            }
        }

        /// <inheritdoc/>
        protected override async Task DisposeAsyncCore()
        {
            // Do nothing.
        }

        /// <summary>
        /// Creates repository on transaction opening.
        /// </summary>
        /// <param name="dataSnapshot">Current state of <c>this</c> storage.</param>
        /// <returns>Created repository.</returns>
        protected abstract TRepository CreateRepository(ImmutableDictionary<TKey, TEntity> dataSnapshot);

        /// <summary>
        /// Merges conflicts.
        /// </summary>
        /// <param name="original">
        /// Original entity <c>state</c> when UoW was opened. <see langword="null"/>, if <c>original</c>
        /// entity not exists.
        /// </param>
        /// <param name="main">Modified in UoW entity. <see langword="null"/>, if entity was removed in this UoW.</param>
        /// <param name="theirs">Modified entity by other UoWs. <see langword="null"/>, if entity was removed by other UoWs.</param>
        /// <returns>Merged entity.</returns>
        [CanBeNull]
        protected virtual TEntity MergeEntity(
            [CanBeNull] TEntity original,
            [CanBeNull] TEntity main,
            [CanBeNull] TEntity theirs)
        {
            return main;
        }

        private void UnitOfWorkOpened(TUnitOfWork uow)
        {
            if (uow == null)
            {
                throw new ArgumentNullException(nameof(uow));
            }

            //// We are under lock here.
            //Debug.Assert(Monitor.IsEntered(uow.DisposeSyncRoot), "Repository creation allowed only under lock");

            uow.SetPluginEntry(this, _data);
        }
    }
#endif
}