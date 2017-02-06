// <copyright file="IxScopeInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public class IxScopeInstance : IxInstance
    {
        public IxScopeInstance(IxHost host, IxProviderNode providerNode, [CanBeNull] IIxInstance parentInstance)
            : base(host, providerNode, parentInstance, providerNode)
        {
        }

        /// <inheritdoc/>
        protected override Task SelfDispose()
        {
            return Task.CompletedTask;
        }
    }
}