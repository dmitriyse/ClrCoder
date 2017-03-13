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
        private readonly IIxInstance _parentInstance;

        private readonly ProcessableSet<IIxInstanceLock> _locks;

        private readonly ProcessableSet<IIxInstanceLock> _ownedLocks;

        private readonly AwaitableEvent _selfDisposeCompleted;

        [CanBeNull]
        private Dictionary<IxProviderNode, object> _childrenData;

        [CanBeNull]
        private object _object;

        /// <summary>
        /// Initializes a new instance of the <see cref="IxInstance"/> class.
        /// </summary>
        /// <param name="providerNode">Node that produces instance.</param>
        /// <param name="parentInstance">Direct parent instance.</param>
        public IxInstance(
            IxProviderNode providerNode,
            [CanBeNull] IIxInstance parentInstance)
            : base(providerNode.Host.InstanceTreeSyncRoot)
        {
            ProviderNode = providerNode;
            _parentInstance = parentInstance;
            _ownedLocks = new ProcessableSet<IIxInstanceLock>();
            _locks = new ProcessableSet<IIxInstanceLock>();
            _selfDisposeCompleted = new AwaitableEvent();
        }

        /// <inheritdoc/>
        public IxProviderNode ProviderNode { get; }

        /// <inheritdoc/>
        [DebuggerHidden]
        public object Object
        {
            get
            {
                // If everything fine this method will be called from one thread and only once.
                // We can skip thread safety check for this method.

                // This should never happened even if schema have errors. Cycle detector should save us.
                Critical.Assert(_object != null, "You cannot use half initialized dependency.");

                return _object;
            }

            set
            {
                // If everything fine this method will be called from one thread and only once.
                // We can skip thread safety check for this method.
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                Critical.Assert(_object == null, "Instance already initialized, you cannot init Object property twice.");

                _object = value;
            }
        }

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CanBeNull]
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
        public object DataSyncRoot => ChildrenData;

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
            EnsureTreeLocked();

            bool inserted = _locks.Add(instanceLock);
            Critical.Assert(inserted, "Lock already registered in the target instance.");

            UpdateDisposeSuspendState();

            UpdateSelfDisposeCompleteSuspendState();
        }

        /// <inheritdoc/>
        public void AddOwnedLock(IIxInstanceLock instanceLock)
        {
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

            if (!Monitor.IsEntered(DataSyncRoot))
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
            EnsureTreeLocked();

            // --------- We are under tree lock --------------
            if (!_locks.Remove(instanceLock))
            {
                Critical.Assert(false, "Lock was not registered in the target or already removed.");
            }

            UpdateDisposeSuspendState();

            UpdateSelfDisposeCompleteSuspendState();
        }

        /// <inheritdoc/>
        public void RemoveOwnedLock(IIxInstanceLock instanceLock)
        {
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

            if (!Monitor.IsEntered(DataSyncRoot))
            {
                Critical.Assert(false, "Data manipulations should be performed under lock.");
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
                // Here we are possibly under lock.
                lock (ProviderNode.Host.InstanceTreeSyncRoot)
                {
                    if (_locks.Any(x => !(x is IxInstanceMasterLock)))
                    {
                        Critical.Assert(
                            false,
                            "Dependencies schema problem, after instance object disposed no any lock are allowed.");
                    }
                }

                // Here we possibly deinitializing half-initialized instance.
                if (_object != null)
                {
                    // Only fully-initialized instance should be self-disposed.
                    await SelfDispose();
                }

                UpdateSelfDisposeCompleteSuspendState();

                _selfDisposeCompleted.Set();

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

                    // If some master locks stayed here, we need to init some awaiter.
                }

                await _selfDisposeCompleted;
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

            if (Locks.All(x => x is IxInstanceMasterLock))
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
        protected abstract Task SelfDispose();

        private void UpdateDisposeSuspendState()
        {
            try
            {
                SetDisposeSuspended(!Locks.All(x => x is IxInstanceMasterLock));
            }
            catch (InvalidOperationException)
            {
                Critical.Assert(false, "Cannot set lock, self dispose was started.");
            }
        }

        private void UpdateSelfDisposeCompleteSuspendState()
        {
            try
            {
                _selfDisposeCompleted.SuspendTrigger(_locks.Any());
            }
            catch (InvalidOperationException)
            {
                Critical.Assert(false, "Cannot set master lock, self dispose was completed.");
            }
        }
    }
}