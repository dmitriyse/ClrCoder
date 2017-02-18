// <copyright file="InMemoryStorageRepositoryEntry.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel.Impl.InMemory
{
    using System;

    using DomainModel.InMemory;

    using ObjectModel;

    /// <summary>
    /// Entity entry in a in-memory storage repository. Stores state and other options.
    /// </summary>
    /// <typeparam name="TKey">Entity key type.</typeparam>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public class InMemoryStorageRepositoryEntry<TKey, TEntity>
        where TKey : IEntityKey<TKey>
        where TEntity : IKeyed<TKey>
    {
        private TEntity _entity;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryStorageRepositoryEntry{TKey,TEntity}"/> class.
        /// </summary>
        /// <param name="entity">In-memory entity.</param>
        public InMemoryStorageRepositoryEntry(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Entity = entity;
        }

        /// <summary>
        /// In-memory entity.
        /// </summary>
        public TEntity Entity
        {
            get
            {
                return _entity;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _entity = value;
            }
        }

        /// <summary>
        /// <c>Entity</c> state.
        /// </summary>
        public InMemoryStorageEntityState State { get; set; }
    }
}