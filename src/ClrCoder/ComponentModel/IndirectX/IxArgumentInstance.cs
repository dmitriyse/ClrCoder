// <copyright file="IxArgumentInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
#pragma warning disable 1998
    using System.Threading.Tasks;

    using Annotations;

    using JetBrains.Annotations;

    using Threading;

    /// <summary>
    /// Special instance implementation of <see cref="IIxInstance"/> for passing resolve arguments.
    /// </summary>
    [InvalidUsageIsCritical]
    public class IxArgumentInstance : AsyncDisposableBase, IIxInstance
    {
        ///
        public IxArgumentInstance(IxArgumentProvider providerNode, [CanBeNull] object instanceObj)
        {
            Critical.CheckedAssert(providerNode != null, "Provider node should not be null.");

            Object = instanceObj;
            ProviderNode = providerNode;
        }

        /// <inheritdoc/>
        public IxProviderNode ProviderNode { get; }

        IIxInstance IIxInstance.ParentInstance { get; }

        IIxResolver IIxInstance.Resolver
        {
            get
            {
                Critical.Assert(false, "Unsupported.");
                return null;
            }

            //// ReSharper disable once ValueParameterNotUsed
            set => Critical.Assert(false, "Unsupported");
        }

        Task IIxInstance.ObjectCreationTask => TaskEx.CompletedTask;

        /// <inheritdoc/>
        public object Object { get; }

        /// <inheritdoc/>
        void IIxInstance.AddLock(IIxInstanceLock instanceLock)
        {
            // Do nothing.
        }

        /// <inheritdoc/>
        void IIxInstance.AddOwnedLock(IIxInstanceLock instanceLock)
        {
            // Do nothing.
        }

        /// <inheritdoc/>
        [CanBeNull]
        object IIxInstance.GetData(IxProviderNode providerNode)
        {
            Critical.Assert(false, "Unsupported.");
            return null;
        }

        void IIxInstance.RemoveLock(IIxInstanceLock instanceLock)
        {
            // Do nothing.
        }

        void IIxInstance.RemoveOwnedLock(IIxInstanceLock instanceLock)
        {
            // Do nothing.
        }

        void IIxInstance.SetData(IxProviderNode providerNode, [CanBeNull] object data)
        {
            Critical.Assert(false, "Unsupported.");
        }

        /// <inheritdoc/>
        protected override async Task DisposeAsyncCore()
        {
            // Do nothing.
        }
    }
}