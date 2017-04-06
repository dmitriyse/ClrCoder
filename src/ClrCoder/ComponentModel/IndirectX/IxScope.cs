// <copyright file="IxScope.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public class IxScope : IxProviderNode
    {
        [CanBeNull]
        private IxScopeInstance _rootInstance;

        public IxScope(
            IxHost host,
            [CanBeNull] IxProviderNode parentNode,
            IxScopeConfig config,
            IxVisibilityFilter exportFilter,
            IxVisibilityFilter exportToParentFilter,
            IxVisibilityFilter importFilter)
            : base(
                host,
                parentNode,
                config,
                null,
                exportFilter,
                exportToParentFilter,
                importFilter,
                host.ScopeBinderBuilder.Delegate(new IxRegistrationScopeBindingConfig()),
                obj => Task.CompletedTask)
        {
            // Adding self provided as default for children.
            VisibleNodes.Add(new IxIdentifier(Identifier.Type), new IxResolvePath(this, new IxProviderNode[] { }));
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        /// <inheritdoc/>
        public override async Task<IIxInstanceLock> GetInstance(
            IIxInstance parentInstance,
            IxIdentifier identifier,
            IxHost.IxResolveContext context,
            [CanBeNull] IxResolveFrame frame)
        {
            if (parentInstance == null)
            {
                throw new ArgumentNullException(nameof(parentInstance));
            }

            lock (Host.InstanceTreeSyncRoot)
            {
                IIxInstanceLock scopeLock;
                IxScopeInstance singleton;
                object data = parentInstance.GetData(this);
                if (data == null)
                {
                    singleton = new IxScopeInstance(this, parentInstance, out scopeLock);
                    parentInstance.SetData(this, singleton);
                }
                else
                {
                    singleton = (IxScopeInstance)data;
                    scopeLock = new IxInstanceTempLock(singleton);
                }

                return scopeLock;
            }
        }

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public IxScopeInstance GetRootInstance()
        {
            if (ParentNode != null)
            {
                throw new InvalidOperationException("Only root scope can produce root instance.");
            }

            if (_rootInstance == null)
            {
                lock (Host.InstanceTreeSyncRoot)
                {
                    if (_rootInstance == null)
                    {
                        IIxInstanceLock creatorLock;
                        _rootInstance = new IxScopeInstance(this, null, out creatorLock);
                        creatorLock.Dispose();
                    }
                }
            }

            return _rootInstance;
        }
    }
}