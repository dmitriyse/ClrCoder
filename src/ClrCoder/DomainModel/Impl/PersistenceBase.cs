// <copyright file="PersistenceBase.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Logging;
    using Logging.Std;

    using Threading;

    /// <summary>
    /// Base implementation of persistence abstraction.
    /// </summary>
    /// <remarks>TODO: Change initialization scheme to AsyncInitializable.</remarks>
    /// <typeparam name="TPersistence">Final persistence type.</typeparam>
    /// <typeparam name="TUnitOfWork">Final unit of work type.</typeparam>
    public abstract class PersistenceBase<TPersistence, TUnitOfWork> : AsyncDisposableBase, IPersistenceImpl
        where TPersistence : PersistenceBase<TPersistence, TUnitOfWork>
        where TUnitOfWork : UnitOfWorkBase<TPersistence, TUnitOfWork>
    {
        private readonly List<PersistencePluginBase<TPersistence, TUnitOfWork>> _plugins =
            new List<PersistencePluginBase<TPersistence, TUnitOfWork>>();

        private readonly HashSet<TUnitOfWork> _openedUoWs = new HashSet<TUnitOfWork>();

        private readonly List<PersistencePluginBase<TPersistence, TUnitOfWork>> _nonDeclaredResolvePlugins =
            new List<PersistencePluginBase<TPersistence, TUnitOfWork>>();

        private Dictionary<Type, PersistencePluginBase<TPersistence, TUnitOfWork>> _repositoryTypeToPlugin =
            new Dictionary<Type, PersistencePluginBase<TPersistence, TUnitOfWork>>();

        private bool _isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceBase{TPersistence,TUnitOfWork}"/> class.
        /// </summary>
        /// <param name="logger">Logger for persistence.</param>
        /// <remarks>
        /// Do not forget to call Initialize method as last operation in the constructor.
        /// </remarks>
        protected PersistenceBase(IJsonLogger logger)
        {
            // Do nothing.
            Log = new ClassJsonLogger<TPersistence>(logger);
        }

        /// <summary>
        /// Raises when new Unit of Work opened.
        /// </summary>
        /// <remarks>
        /// Plugins can subscribe to <c>this</c> event to perform required UoW initializations.
        /// </remarks>
        public event Action<TUnitOfWork> UnitOfWorkOpened;

        private IJsonLogger Log { get; }

        /// <inheritdoc/>
        public IUnitOfWorkImpl OpenUnitOfWork()
        {
            VerifyInitialized();

            // TODO: pass debug information to a unit of work.
            lock (DisposeSyncRoot)
            {
                SetDisposeSuspended(true);
                TUnitOfWork uow = CreateUnitOfWork();

                Critical.Assert(
                    DisposeSyncRoot == uow.DisposeSyncRoot,
                    "Unit of Work and its persistence should be synchronized with the same DisposeSyncRoot.");

                _openedUoWs.Add(uow);

                try
                {
                    // Handler should never rise an exception.
                    OnUnitOfWorkOpened(uow);
                }
                catch (Exception ex)
                {
                    Critical.Assert(
                        false,
                        $"UnitOfWorkOpened handlers should never rise any exception. Error = {ex.Message}");
                }

                return uow;
            }
        }

        /// <summary>
        /// Processes UoW disposed event.
        /// </summary>
        /// <param name="uow">Disposed UoW.</param>
        /// <remarks>SyncRoot <c>lock</c> required here.</remarks>
        internal void OnUowDisposed(TUnitOfWork uow)
        {
            // We need lock here.
            lock (DisposeSyncRoot)
            {
                _openedUoWs.Remove(uow);
                if (_openedUoWs.Count == 0)
                {
                    SetDisposeSuspended(false);
                }
            }
        }

        /// <summary>
        /// Registers persistence <c>plugin</c>.
        /// </summary>
        /// <param name="plugin">Persistence <c>plugin</c>.</param>
        internal void RegisterPlugin(PersistencePluginBase<TPersistence, TUnitOfWork> plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            Critical.Assert(!_isInitialized, "You cannot register plugin after persistence become initialized.");

            _plugins.Add(plugin);
        }

        /// <summary>
        /// Resolves repository with help of registered plugins.
        /// </summary>
        /// <typeparam name="TRepository">Type of repository to resolve.</typeparam>
        /// <param name="uow">Unit of work where resolve should be performed.</param>
        /// <returns>Resolved unit of work.</returns>
        /// <exception cref="NotSupportedException">No any plugin that can resolve asked repository.</exception>
        internal TRepository ResolveRepository<TRepository>(TUnitOfWork uow)
            where TRepository : class, IRepository
        {
            PersistencePluginBase<TPersistence, TUnitOfWork> plugin;

            TRepository repository = default(TRepository);
            if (_repositoryTypeToPlugin.TryGetValue(typeof(TRepository), out plugin))
            {
                repository = plugin.ResolveRepository<TRepository>(uow);
            }
            else
            {
                foreach (PersistencePluginBase<TPersistence, TUnitOfWork> p in _nonDeclaredResolvePlugins)
                {
                    repository = p.ResolveRepository<TRepository>(uow);
                    if (repository != null)
                    {
                        break;
                    }
                }
            }

            if (repository == null)
            {
                throw new NotSupportedException("No any plugin that can resolve asked repository.");
            }

            return repository;
        }

        /// <inheritdoc/>
        protected override async Task AsyncDispose()
        {
            // Waiting all plugin dispose.
            foreach (PersistencePluginBase<TPersistence, TUnitOfWork> plugin in _plugins)
            {
                try
                {
                    await plugin.DisposeAsync();
                }
                catch (Exception ex)
                {
                    Log.Error(
                        ex,
                        plugin.GetType().Name,
                        (_, e, pn) =>
                            _($"Persistence plugin {pn} dispose error: {e.Message}").Exception(e));
                }
            }
        }

        /// <summary>
        /// Creates new unit of work instance.
        /// </summary>
        /// <returns>New unit of work.</returns>
        protected abstract TUnitOfWork CreateUnitOfWork();

        /// <summary>
        /// Performs persistence initialization after all plugins are registered.
        /// </summary>
        protected virtual void Initialize()
        {
            _isInitialized = true;

            _nonDeclaredResolvePlugins.AddRange(_plugins.Where(x => x.AreNonDeclaredRepositoryTypesSupported));
            foreach (KeyValuePair<Type, PersistencePluginBase<TPersistence, TUnitOfWork>> kvp
                in _plugins.SelectMany(
                    x =>
                        x.SupportedRepositoryTypes.Select(
                            y => new KeyValuePair<Type, PersistencePluginBase<TPersistence, TUnitOfWork>>(y, x))))
            {
                _repositoryTypeToPlugin.Add(kvp.Key, kvp.Value);
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposeStarted()
        {
            // We are under lock here.
            // -------------------------------
            lock (DisposeSyncRoot)
            {
                if (_openedUoWs.Any())
                {
                    // TODO: Add debug info here
                    Log.Error(
                        _ =>
                            _("Dispose started, but not all UoW finished. Waiting for all UoW disposed.")
                                .Details(
                                    "Persistence shutdown should begins after all activity finished. You have bad application design."));
                }
            }

            base.OnDisposeStarted();
        }

        /// <summary>
        /// Handles unit of work opening event.
        /// </summary>
        /// <param name="uow">Opened unit of work.</param>
        protected virtual void OnUnitOfWorkOpened(TUnitOfWork uow)
        {
            UnitOfWorkOpened?.Invoke(uow);
        }

        private void VerifyInitialized()
        {
            Critical.Assert(_isInitialized, "You forgot to perform initialization.");
        }
    }
}