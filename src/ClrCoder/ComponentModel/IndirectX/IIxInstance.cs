// <copyright file="IIxInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using JetBrains.Annotations;

    using Threading;

    public interface IIxInstance : IAsyncDisposable
    {
        IxHost Host { get; }

        IxProviderNode ProviderNode { get; }

        object Object { get; }

        IIxInstance ParentInstance { get; }

        IIxResolver Resolver { get; set; }

        [CanBeNull]
        object GetData(IxProviderNode providerNode);

        object DataSyncRoot { get; }

        void SetData(IxProviderNode providerNode, [CanBeNull] object data);
    }
}