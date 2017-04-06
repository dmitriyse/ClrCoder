// <copyright file="IxInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Collections;

    using JetBrains.Annotations;

    using Threading;

    /// <summary>
    /// IndirectX controlled instance placeholder <c>base</c> implementation.
    /// </summary>
    /// <remarks>Should be implemented as proxy in the future.</remarks>
    public abstract class IxInstance : AsyncDisposableBase, IIxInstance
    {
        [CanBeNull]
        private readonly IIxInstance _parentInstance;

        private readonly ProcessableSet<IIxInstanceLock> _locks;

        private readonly ProcessableSet<IIxInstanceLock> _ownedLocks;

        private readonly AwaitableEvent _childrenDisposeCompleted;

        [CanBeNull]
        private Dictionary<IxProviderNode, object> _childrenData;

        [CanBeNull]
        private Task<object> _objectCreationTask;

        [CanBeNull]
        private IIxInstanceLock _initTempLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="IxInstance"/> class.
        /// </summary>
        /// <remarks>
        /// Instance creates in the "Half-instantiated state".
        /// </remarks>
        /// <param name="providerNode">Node that produces instance.</param>
        /// <param name="parentInstance">Direct parent instance.</param>
        /// <param name="creatorTempLock">First temp lock for the creator of a new instance.</param>
        public IxInstance(
            IxProviderNode providerNode,
            [CanBeNull] IIxInstance parentInstance,
            out IIxInstanceLock creatorTempLock)
            : base(providerNode.Host.InstanceTreeSyncRoot)
        {
            ProviderNode = providerNode;
            _parentInstance = parentInstance;
            _ownedLocks = new ProcessableSet<IIxInstanceLock>();
            _locks = new ProcessableSet<IIxInstanceLock>();
            _childrenDisposeCompleted = new AwaitableEvent();
            if (parentInstance != null)
            {
                new IxInstanceChildLock(parentInstance, this);
            }

            creatorTempLock = new IxInstanceTempLock(this);
            _initTempLock = creatorTempLock;
        }

        /// <inheritdoc/>
        public IxProviderNode ProviderNode { get; }

        //// TODO: Refactor to just task.

        /// <inheritdoc/>
        [DebuggerHidden]
        public Task ObjectCreationTask
        {
            get
            {
                // If everything fine this method will be called from one thread and only once.
                // We can skip thread safety check for this method.

                // This should never happened even if schema have errors. Cycle detector should save us.
                Critical.Assert(_objectCreationTask != null, "You cannot use not properly initialized object.");

                return _objectCreationTask;
            }
        }

        /// <inheritdoc/>
        public object Object
        {
            get
            {
                Critical.Assert(_objectCreationTask != null, "Object creation task was not initialized.");
                Critical.Assert(
                    _objectCreationTask.IsCompleted,
                    "You cannot get instance object if instantiation is not yet completed.");
                return _objectCreationTask.Result;
            }
        }

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IIxInstance ParentInstance
        {
            get
            {
                if (!Monitor.IsEntered(ProviderNode.Host.InstanceTreeSyncRoot))
                {
                    Critical.Assert(
                        false,
                        "Inspecting instance parent should be performed under InstanceTreeLock.");
                }

                return _parentInstance;
            }
        }

        /// <inheritdoc/>
        public IIxResolver Resolver { get; set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IIxInstanceLock> OwnedLocks => _ownedLocks;

        /// <inheritdoc/>
        public IReadOnlyCollection<IIxInstanceLock> Locks => _locks;

        private Dictionary<IxProviderNode, object> ChildrenData
        {
            get
            {
                if (_childrenData == null)
                {
                    Interlocked.CompareExchange(ref _childrenData, new Dictionary<IxProviderNode, object>(), null);
                }

                return _childrenData;
            }
        }

        /// <inheritdoc/>
        public void AddLock(IIxInstanceLock instanceLock)
        {
            if (instanceLock == null)
            {
                throw new ArgumentNullException(nameof(instanceLock));
            }

            EnsureTreeLocked();

            bool inserted = _locks.Add(instanceLock);
            Critical.Assert(inserted, "Lock already registered in the target instance.");

            UpdateDisposeSuspendState();

            UpdateChildrenDisposeCompleteSuspendState();
        }

        /// <inheritdoc/>
        public void AddOwnedLock(IIxInstanceLock instanceLock)
        {
            if (instanceLock == null)
            {
                throw new ArgumentNullException(nameof(instanceLock));
            }

            EnsureTreeLocked();
            if (!_ownedLocks.Add(instanceLock))
            {
                Critical.Assert(false, "Owned Lock already registered in the owner.");
            }
        }

        /// <inheritdoc/>
        [CanBeNull]
        public object GetData(IxProviderNode providerNode)
        {
            if (providerNode == null)
            {
                throw new ArgumentNullException(nameof(providerNode));
            }

            if (!Monitor.IsEntered(ProviderNode.Host.InstanceTreeSyncRoot))
            {
                Critical.Assert(false, "Data manipulations should be performed under lock.");
            }

            object result;

            ChildrenData.TryGetValue(providerNode, out result);

            return result;
        }

        /// <inheritdoc/>
        public void RemoveLock(IIxInstanceLock instanceLock)
        {
            if (instanceLock == null)
            {
                throw new ArgumentNullException(nameof(instanceLock));
            }

            EnsureTreeLocked();

            if (ReferenceEquals(instanceLock, _initTempLock))
            {
                Critical.Assert(
                    _objectCreationTask != null,
                    "You need to setup instance object factory before releasing creator lock.");
                Critical.Assert(
                    _objectCreationTask.IsCompleted,
                    "Creator lock should be removed only after object instantiation completes.");
            }

            _initTempLock = null;

            // --------- We are under tree lock --------------
            if (!_locks.Remove(instanceLock))
            {
                Critical.Assert(false, "Lock was not registered in the target or already removed.");
            }

            UpdateDisposeSuspendState();

            UpdateChildrenDisposeCompleteSuspendState();
        }

        /// <inheritdoc/>
        public void RemoveOwnedLock(IIxInstanceLock instanceLock)
        {
            if (instanceLock == null)
            {
                throw new ArgumentNullException(nameof(instanceLock));
            }

            EnsureTreeLocked();

            // --------- We are under tree lock --------------
            if (!_ownedLocks.Remove(instanceLock))
            {
                Critical.Assert(false, "Owned lock was not registered in the owner or already removed.");
            }
        }

        /// <inheritdoc/>
        public void SetData(IxProviderNode providerNode, [CanBeNull] object data)
        {
            if (providerNode == null)
            {
                throw new ArgumentNullException(nameof(providerNode));
            }

            if (!Monitor.IsEntered(ProviderNode.Host.InstanceTreeSyncRoot))
            {
                Critical.Assert(false, "Data manipulations should be performed under tree lock.");
            }

            if (data == null)
            {
                ChildrenData.Remove(providerNode);
            }
            else
            {
                ChildrenData[providerNode] = data;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{ProviderNode}({GetHashCode()})";
        }

        /// <inheritdoc/>
        protected override async Task AsyncDispose()
        {
            // Under tree lock here, until first await.
            EnsureTreeLocked();

            try
            {
                if (_locks.Any(x => !(x is IxInstanceChildLock)))
                {
                    Critical.Assert(
                        false,
                        "Dependencies schema problem, after instance object disposed no any lock are allowed.");
                }

                if (!ObjectCreationTask.IsFaulted)
                {
                    // Only fully-initialized instance should be self-disposed.
                    await SelfDispose();
                }

                // This call is not required.
                ////UpdateChildrenDisposeCompleteSuspendState();
                _childrenDisposeCompleted.Set();

                // Sync or async execution here.
            }
            finally
            {
                // Sync or async execution here.
                // ------------------------------------
                lock (ProviderNode.Host.InstanceTreeSyncRoot)
                {
                    // Freeing all owned locks.
                    while (OwnedLocks.Any())
                    {
                        // Here other objects can dispose synchronously.
                        OwnedLocks.First().Dispose();
                    }
                }

                await _childrenDisposeCompleted;
            }
        }

        /// <summary>
        /// Ensures that tree <c>lock</c> obtained by current thread.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnsureTreeLocked()
        {
            Critical.Assert(
                Monitor.IsEntered(ProviderNode.Host.InstanceTreeSyncRoot),
                "Lock related operation should be performed under global tree lock.");
        }

        /// <inheritdoc/>
        protected override void OnDisposeStarted()
        {
            // We are under tree lock here
            // ---------------------------------------------------------

            // This method is not reenterant.
            // ---------------------------------------------------------
            _locks.ForEach(x => x.PulseDispose());

            if (Locks.All(x => x is IxInstanceChildLock))
            {
                SetDisposeSuspended(false);
            }

            base.OnDisposeStarted();
        }

        /// <summary>
        /// Performs dispose related to this instance itself. This method is called after all locks on <c>this</c> objects are
        /// removed.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        protected virtual Task SelfDispose()
        {
            return ProviderNode.DisposeHandler(Object);
        }

        /// <summary>
        /// Initializes object creation task.
        /// </summary>
        /// <param name="objectCreateTask">The object creation task.</param>
        protected void SetObjectCreationTask(Task<object> objectCreateTask)
        {
            // If everything fine this method will be called from one thread and only once.
            // We can skip thread safety check for this method.
            if (objectCreateTask == null)
            {
                throw new ArgumentNullException(nameof(objectCreateTask));
            }

            EnsureTreeLocked();

            Critical.Assert(
                _objectCreationTask == null,
                "Instance already initialized, you cannot init Object property twice.");

            _objectCreationTask = ObjectFactoryProxy(objectCreateTask).EnsureStarted();
        }

        private async Task<object> ObjectFactoryProxy(Task<object> objectFactory)
        {
            try
            {
                return await objectFactory;
            }
            finally
            {
                // TODO: Re-implement me gracefully without lock.
                lock (this)
                {
                    if (Resolver == null)
                    {
                        Resolver = new IxHost.IxResolver(ProviderNode.Host, this, null, null);
                    }
                    else
                    {
                        var resolver = (IxHost.IxResolver)Resolver;
                        resolver.ClearParentResolveContext();
                    }
                }
            }
        }

        private void UpdateChildrenDisposeCompleteSuspendState()
        {
            try
            {
                _childrenDisposeCompleted.SuspendTrigger(_locks.Any());
            }
            catch (InvalidOperationException)
            {
                Critical.Assert(false, "Cannot set child lock, full dispose was completed.");
            }
        }

        private void UpdateDisposeSuspendState()
        {
            try
            {
                SetDisposeSuspended(!Locks.All(x => x is IxInstanceChildLock));
            }
            catch (InvalidOperationException)
            {
                Critical.Assert(false, "Cannot set lock, self dispose was started.");
            }
        }
    }
}