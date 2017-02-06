// <copyright file="IxInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public abstract class IxInstance : IIxInstance
    {
        private readonly IIxInstance _parentInstance;

        private readonly HashSet<IIxInstanceLock> _locks;

        private readonly HashSet<IIxInstanceLock> _ownedLocks;

        private Task _disposeTask;

        private TaskCompletionSource<bool> _disposeCompletionSource;

        private bool _selfDisposingStarted;

        [CanBeNull]
        private Dictionary<IxProviderNode, object> _childrenData;

        public IxInstance(
            IxHost host,
            IxProviderNode providerNode,
            [CanBeNull] IIxInstance parentInstance,
            object @object)
        {
            Host = host;
            ProviderNode = providerNode;
            _parentInstance = parentInstance;
            Object = @object;
            _ownedLocks = new HashSet<IIxInstanceLock>();
            _locks = new HashSet<IIxInstanceLock>();
        }

        public IxHost Host { get; }

        public IxProviderNode ProviderNode { get; }

        public object Object { get; }

        [CanBeNull]
        public IIxInstance ParentInstance
        {
            get
            {
                if (!Monitor.IsEntered(Host.InstanceTreeSyncRoot))
                {
                    Contract.Assert(
                        false,
                        "Inspecting instance parent should be performed under InstanceTreeLock.");
                }

                return _parentInstance;
            }
        }

        public IIxResolver Resolver { get; set; }

        public object DataSyncRoot => ChildrenData;

        public Task DisposeTask
        {
            get
            {
                if (_disposeTask == null && _disposeCompletionSource == null)
                {
                    lock (Host.InstanceTreeSyncRoot)
                    {
                        if (_disposeTask == null && _disposeCompletionSource == null)
                        {
                            _disposeCompletionSource = new TaskCompletionSource<bool>();
                            _disposeTask = _disposeCompletionSource.Task;
                        }
                    }
                }

                return _disposeTask;
            }
        }

        public IReadOnlyCollection<IIxInstanceLock> OwnedLocks => _ownedLocks;

        public IReadOnlyCollection<IIxInstanceLock> Locks => _locks;

        public bool IsDisposing { get; protected set; }

        public int LocksVersion { get; set; }

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

        public void AddLock(IIxInstanceLock instanceLock)
        {
            EnsureTreeLocked();

            if (!_locks.Add(instanceLock))
            {
                Contract.Assert(false, "Lock already registered in the target instance.");
            }

            if (_selfDisposingStarted)
            {
                Contract.Assert(false, "You cannot use instance while in object self-disposing state");
            }

            LocksVersion++;
        }

        public void AddOwnedLock(IIxInstanceLock instanceLock)
        {
            EnsureTreeLocked();
            if (!_ownedLocks.Add(instanceLock))
            {
                Contract.Assert(false, "Owned Lock already registered in the owner.");
            }
        }

        [CanBeNull]
        public object GetData(IxProviderNode providerNode)
        {
            if (providerNode == null)
            {
                throw new ArgumentNullException(nameof(providerNode));
            }

            if (!Monitor.IsEntered(Host.InstanceTreeSyncRoot))
            {
                Contract.Assert(
                    false,
                    "Inspecting instance parent should be performed under InstanceTreeLock.");
            }

            if (!Monitor.IsEntered(DataSyncRoot))
            {
                Contract.Assert(false, "Data manipulations should be performed under lock.");
            }

            object result;

            ChildrenData.TryGetValue(providerNode, out result);

            return result;
        }

        /// <inheritdoc/>
        public void RemoveLock(IIxInstanceLock instanceLock)
        {
            EnsureTreeLocked();
            if (!_locks.Remove(instanceLock))
            {
                Contract.Assert(false, "Lock was not registered in the target or already removed.");
            }

            LocksVersion++;
            if (IsDisposing)
            {
                TryStartSelfDispose();
            }
        }

        /// <inheritdoc/>
        public void RemoveOwnedLock(IIxInstanceLock instanceLock)
        {
            EnsureTreeLocked();
            if (!_ownedLocks.Remove(instanceLock))
            {
                Contract.Assert(false, "Owned lock was not registered in the owner or already removed.");
            }
        }

        public void SetData(IxProviderNode providerNode, [CanBeNull] object data)
        {
            if (providerNode == null)
            {
                throw new ArgumentNullException(nameof(providerNode));
            }

            if (!Monitor.IsEntered(Host.InstanceTreeSyncRoot))
            {
                Contract.Assert(
                    false,
                    "Inspecting instance parent should be performed under InstanceTreeLock.");
            }

            if (!Monitor.IsEntered(DataSyncRoot))
            {
                Contract.Assert(false, "Data manipulations should be performed under lock.");
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

        public virtual void StartDispose()
        {
            lock (Host.InstanceTreeSyncRoot)
            {
                IsDisposing = true;

                var pulsedInstances = new HashSet<IIxInstanceLock>();

                // Collection enumeration retries loop.
                while (true)
                {
                    int currentLocksVersion = LocksVersion;
                    foreach (IIxInstanceLock lockOfMe in Locks)
                    {
                        if (pulsedInstances.Add(lockOfMe))
                        {
                            // Collection can be changed in nested calls of this method.
                            lockOfMe.PulseDispose();
                        }

                        // In a case of collection change we need to perform re-enumerate collection.
                        if (LocksVersion != currentLocksVersion)
                        {
                            break;
                        }
                    }

                    // Once we enumerated lock collection to the end we can exit retries loop.
                    if (LocksVersion == currentLocksVersion)
                    {
                        break;
                    }
                }

                TryStartSelfDispose();
            }
        }

        protected abstract Task SelfDispose();

        private void EnsureTreeLocked()
        {
            Contract.Assert(
                Monitor.IsEntered(Host.InstanceTreeSyncRoot),
                "Lock related operation should be performed under global tree lock.");
        }

        private void TryStartSelfDispose()
        {
            if (!Locks.Any())
            {
                _selfDisposingStarted = true;
                Task selfDisposeTask = SelfDispose();

                // Synchronous version.
                if (selfDisposeTask.IsCompleted)
                {
                    if (Locks.Any())
                    {
                        Contract.Assert(
                            false,
                            "Dependencies schema problem, after instance object disposed no any lock are allowed.");
                    }

                    // Freeing all owned locks.
                    while (OwnedLocks.Any())
                    {
                        // Here other objcts can dispose synchronously.
                        OwnedLocks.First().Dispose();
                    }

                    if (_disposeTask == null)
                    {
                        _disposeTask = Task.CompletedTask;
                    }
                    else if (_disposeCompletionSource != null)
                    {
                        _disposeCompletionSource.SetResult(true);
                    }
                    else
                    {
                        Contract.Assert(false, "Dispose logic error. State should be unreachable.");
                    }
                }
                else
                {
                    Func<Task> disposeAsyncFinish = async () =>
                        {
                            await selfDisposeTask;
                            if (Locks.Any())
                            {
                                Contract.Assert(
                                    false,
                                    "Dependencies schema problem, after instance object disposed no any lock are allowed.");
                            }

                            lock (Host.InstanceTreeSyncRoot)
                            {
                                // Freeing all owned locks.
                                while (OwnedLocks.Any())
                                {
                                    // Here other objcts can dispose synchronously.
                                    OwnedLocks.First().Dispose();
                                }

                                _disposeCompletionSource?.SetResult(true);
                            }
                        };

                    if (_disposeTask == null)
                    {
                        _disposeTask = disposeAsyncFinish();
                    }
                }
            }
        }
    }
}