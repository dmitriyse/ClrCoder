// <copyright file="IIxInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Threading;

    public interface IIxInstance : IAsyncDisposable
    {
        [NotNull]
        IxProviderNode ProviderNode { get; }

        [CanBeNull]
        IIxInstance ParentInstance { get; }

        [CanBeNull]
        IIxResolver Resolver { get; set; }

        /// <summary>
        /// Object instantiation task.
        /// </summary>
        [NotNull]
        Task ObjectCreationTask { get; }

        /// <summary>
        /// Gets resolved object.
        /// </summary>
        [NotNull]
        object Object { get; }

        void AddLock(IIxInstanceLock instanceLock);

        /// <summary>
        /// Adds lock, that is owned by this instance. So when instance is disposed, all owned locks also released.
        /// </summary>
        /// <param name="instanceLock">The lock that is owned by this instance.</param>
        void AddOwnedLock(IIxInstanceLock instanceLock);

        [CanBeNull]
        object GetData(IxProviderNode providerNode);

        void RemoveLock(IIxInstanceLock instanceLock);

        void RemoveOwnedLock(IIxInstanceLock instanceLock);

        void SetData(IxProviderNode providerNode, [CanBeNull] object data);
    }
}