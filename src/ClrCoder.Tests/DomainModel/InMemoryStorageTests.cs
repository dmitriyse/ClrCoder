// <copyright file="InMemoryStorageTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    using ClrCoder.DomainModel;
    using ClrCoder.DomainModel.Impl;
    using ClrCoder.DomainModel.Impl.InMemory;
    using ClrCoder.Logging;
    using ClrCoder.Threading;

    using FluentAssertions;

    using JetBrains.Annotations;

    using NUnit.Framework;

    using ObjectModel;

    using Testing;

    /// <summary>
    /// Tests related to the
    /// <see cref="InMemoryStorage{TPersistence,TUnitOfWork,TStorage,TRepository,TKey,TEntity}"/>
    /// </summary>
    [TestFixture]
    public class InMemoryStorageTests
    {
        private interface IDummyEntityRepository : IRepository
        {
            DummyEntity CreateNew(string name);

            [CanBeNull]
            DummyEntity GetEntity(DummyEntityKey key);

            void Remove(DummyEntityKey key);

            IEnumerable<DummyEntity> SelectAll();
        }

        /// <summary>
        /// Simple CRUD test.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task SimpleCrudTest()
        {
            await new SimplePersistence(
                    new NUnitJsonLogger(),
                    new List<IPersistenceInitializer<SimplePersistence>>
                        {
                            new SimpleCrudTestPersistenceInitializer()
                        })
                .AsyncUsing(
                    async p =>
                        {
                            DummyEntityKey firstKey = null;
                            DummyEntityKey secondKey = null;

                            await p.OpenUnitOfWork()
                                .AsyncUsing(
                                    uow =>
                                        {
                                            var repository = uow.GetRepository<IDummyEntityRepository>();
                                            firstKey = repository.CreateNew("First").Key;
                                        });

                            await p.OpenUnitOfWork()
                                .AsyncUsing(
                                    uow =>
                                        {
                                            var repository = uow.GetRepository<IDummyEntityRepository>();
                                            string firstName = repository.GetEntity(firstKey)?.Name;
                                            firstName.Should().Be("First");
                                            secondKey = repository.CreateNew("Second").Key;
                                        });

                            await p.OpenUnitOfWork()
                                .AsyncUsing(
                                    uow =>
                                        {
                                            var repository = uow.GetRepository<IDummyEntityRepository>();
                                            Dictionary<DummyEntityKey, DummyEntity> allDummies =
                                                repository.SelectAll().ToDictionary(x => x.Key);
                                            allDummies.Count.Should().Be(2);
                                            allDummies.Should().ContainKey(firstKey);
                                            allDummies.Should().ContainKey(secondKey);
                                            repository.Remove(secondKey);
                                        });

                            await p.OpenUnitOfWork()
                                .AsyncUsing(
                                    uow =>
                                        {
                                            var repository = uow.GetRepository<IDummyEntityRepository>();
                                            Dictionary<DummyEntityKey, DummyEntity> allDummies =
                                                repository.SelectAll().ToDictionary(x => x.Key);
                                            allDummies.Count.Should().Be(1);
                                            allDummies.Should().ContainKey(firstKey);
                                            repository.GetEntity(secondKey).Should().BeNull();
                                        });
                        });
        }

        private class DummyEntity : IDeepCloneable<DummyEntity>, IKeyed<DummyEntityKey>
        {
            public DummyEntity(DummyEntityKey key)
            {
                Key = key;
            }

            public DummyEntityKey Key { get; }

            public string Name { get; set; }

            public DummyEntity Clone()
            {
                return new DummyEntity(Key)
                           {
                               Name = Name
                           };
            }
        }

        private class DummyEntityKey : IEntityKey<DummyEntityKey>
        {
            /// <inheritdoc/>
            public bool Equals([CanBeNull] DummyEntityKey other)
            {
                return ReferenceEquals(this, other);
            }
        }

        private class DummyEntityRepository :
            InMemoryStorageRepository<
                SimplePersistence,
                SimpleUnitOfWork,
                SimpleInMemoryStorage,
                DummyEntityRepository,
                DummyEntityKey,
                DummyEntity>,
            IDummyEntityRepository
        {
            public DummyEntityRepository(
                SimpleInMemoryStorage storage,
                ImmutableDictionary<DummyEntityKey, DummyEntity> dataSnapshot)
                : base(storage, dataSnapshot)
            {
            }

            public DummyEntity CreateNew(string name)
            {
                var created = new DummyEntity(new DummyEntityKey()) { Name = name };
                Add(created);

                return created;
            }

            [CanBeNull]
            public DummyEntity GetEntity(DummyEntityKey key)
            {
                return SelectByKey(key);
            }

            void IDummyEntityRepository.Remove(DummyEntityKey key)
            {
                Remove(key);
            }

            public IEnumerable<DummyEntity> SelectAll()
            {
                return GetAll();
            }
        }

        private class SimpleCrudTestPersistenceInitializer : IPersistenceInitializer<SimplePersistence>
        {
            public void Initialize(SimplePersistence persistence)
            {
                // ReSharper disable once ObjectCreationAsStatement
                new SimpleInMemoryStorage(persistence, ReferenceEqualityComparer<DummyEntity>.Default);
            }
        }

        private class SimpleInMemoryStorage :
            InMemoryStorage<
                SimplePersistence,
                SimpleUnitOfWork,
                SimpleInMemoryStorage,
                DummyEntityRepository,
                DummyEntityKey,
                DummyEntity>
        {
            public SimpleInMemoryStorage(SimplePersistence persistence, IEqualityComparer<DummyEntity> mergeComparer)
                : base(persistence, mergeComparer, new HashSetEx<Type> { typeof(IDummyEntityRepository) })
            {
            }

            /// <inheritdoc/>
            protected override DummyEntityRepository CreateRepository(
                ImmutableDictionary<DummyEntityKey, DummyEntity> dataSnapshot)
            {
                return new DummyEntityRepository(this, dataSnapshot);
            }
        }

        private sealed class SimplePersistence : PersistenceBase<SimplePersistence, SimpleUnitOfWork>
        {
            public SimplePersistence(
                IJsonLogger logger,
                ICollection<IPersistenceInitializer<SimplePersistence>> initializers)
                : base(logger)
            {
                if (initializers == null)
                {
                    throw new ArgumentNullException(nameof(initializers));
                }

                foreach (IPersistenceInitializer<SimplePersistence> initializer in initializers)
                {
                    initializer.Initialize(this);
                }

                Initialize();
            }

            protected override SimpleUnitOfWork CreateUnitOfWork()
            {
                return new SimpleUnitOfWork(this);
            }
        }

        private class SimpleUnitOfWork : UnitOfWorkBase<SimplePersistence, SimpleUnitOfWork>
        {
            public SimpleUnitOfWork(SimplePersistence persistence)
                : base(persistence)
            {
            }
        }
    }
}