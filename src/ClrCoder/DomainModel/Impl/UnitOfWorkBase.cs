// <copyright file="UnitOfWorkBase.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>


#pragma warning disable 1998

namespace ClrCoder.DomainModel.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Threading;

    /// <summary>
    /// Base implementation of unit of work abstraction.
    /// </summary>
    /// <typeparam name="TPersistence">Final type of persistence.</typeparam>
    /// <typeparam name="TUnitOfWork">Final type of unit of work.</typeparam>
    public abstract class UnitOfWorkBase<TPersistence, TUnitOfWork> : AsyncDisposableBase, IUnitOfWorkImpl
        where TPersistence : PersistenceBase<TPersistence, TUnitOfWork>
        where TUnitOfWork : UnitOfWorkBase<TPersistence, TUnitOfWork>
    {
        private readonly Dictionary<PersistencePluginBase<TPersistence, TUnitOfWork>, object> _pluginEntries =
            new Dictionary<PersistencePluginBase<TPersistence, TUnitOfWork>, object>();

        private bool _isDiscardRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkBase{TPersistence,TUnitOfWork}"/> class.
        /// </summary>
        /// <param name="persistence">Persistence to which <c>this</c> UoW belongs.</param>
        protected UnitOfWorkBase(TPersistence persistence)
            : base(persistence.DisposeSyncRoot)
        {
            if (persistence == null)
            {
                throw new ArgumentNullException(nameof(persistence));
            }

            Persistence = persistence;
        }

        /// <summary>
        /// Persistence to which <c>this</c> UoW belongs.
        /// </summary>
        public TPersistence Persistence { get; }

        /// <inheritdoc/>
        public T GetRepository<T>() where T : class, IRepository
        {
            return Persistence.ResolveRepository<T>((TUnitOfWork)this);
        }

        /// <inheritdoc/>
        public void HandleException(Exception exception)
        {
            // This variable should not be concurrent, so lock is not needed.
            _isDiscardRequested = true;
        }

        /// <summary>
        /// Gets <c>plugin</c> entry associated with UoW.
        /// </summary>
        /// <param name="plugin">Plugin to get entry for.</param>
        /// <returns>Plugin entry.</returns>
        [CanBeNull]
        public object GetPluginEntry(PersistencePluginBase<TPersistence, TUnitOfWork> plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            if (!ReferenceEquals(plugin.Persistence, Persistence))
            {
                throw new ArgumentException("Plugin and UoW should belongs to the same persistence.", nameof(plugin));
            }

            lock (DisposeSyncRoot)
            {
                object data;
                _pluginEntries.TryGetValue(plugin, out data);

                return data;
            }
        }

        /// <summary>
        /// Sets <c>plugin</c> <c>entry</c> to <c>this</c> UoW.
        /// </summary>
        /// <param name="plugin">Plugin to which associate specified <c>entry</c>.</param>
        /// <param name="entry">
        /// Entry associated with the specified <c>plugin</c>. Use <see langword="null"/> to delete
        /// <c>entry</c>.
        /// </param>
        public void SetPluginEntry(PersistencePluginBase<TPersistence, TUnitOfWork> plugin, [CanBeNull] object entry)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            if (!ReferenceEquals(plugin.Persistence, Persistence))
            {
                throw new ArgumentException("Plugin and UoW should belongs to the same persistence.", nameof(plugin));
            }

            lock (DisposeSyncRoot)
            {
                if (entry == null)
                {
                    _pluginEntries.Remove(plugin);
                }
                else
                {
                    _pluginEntries[plugin] = entry;
                }
            }
        }

        /// <inheritdoc/>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1110:OpeningParenthesisMustBeOnDeclarationLine",
            Justification = "Reviewed. Suppression is OK here.")]
        protected override async Task AsyncDispose()
        {
            List<ValueTuple<PersistencePluginBase<TPersistence, TUnitOfWork>, IDisposablePluginEntry<TPersistence, TUnitOfWork>>> entriesToDispose;
            lock (DisposeSyncRoot)
            {
                entriesToDispose =
                    _pluginEntries.Where(x => x.Value is IDisposablePluginEntry<TPersistence, TUnitOfWork>)
                        .Select(
                            x =>
                                new ValueTuple
                                <PersistencePluginBase<TPersistence, TUnitOfWork>,
                                    IDisposablePluginEntry<TPersistence, TUnitOfWork>>(
                                    x.Key,
                                    x.Value as IDisposablePluginEntry<TPersistence, TUnitOfWork>)).ToList();
            }

            foreach (
                ValueTuple<PersistencePluginBase<TPersistence, TUnitOfWork>, IDisposablePluginEntry<TPersistence, TUnitOfWork>> tuple 
                in entriesToDispose)
            {
                try
                {
                    await tuple.Item2.EnsureDisposed(tuple.Item1, (TUnitOfWork)this, _isDiscardRequested);
                }
                catch (Exception ex)
                {
                    if (!ex.IsProcessable())
                    {
                        throw;
                    }

                    // TODO: Log problem
                }
            }

            Persistence.OnUowDisposed((TUnitOfWork)this);
        }
    }
}