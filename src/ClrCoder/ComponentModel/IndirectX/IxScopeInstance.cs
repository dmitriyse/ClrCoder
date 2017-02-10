// <copyright file="IxScopeInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Scope instance. TODO: Scope should be instance less.
    /// </summary>
    public class IxScopeInstance : IxInstance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxScopeInstance"/> class.
        /// </summary>
        /// <param name="providerNode">Owner provider.</param>
        /// <param name="parentInstance">Parent instance.</param>
        public IxScopeInstance(IxProviderNode providerNode, [CanBeNull] IIxInstance parentInstance)
            : base(providerNode, parentInstance)
        {
            Object = providerNode;
        }

        /// <inheritdoc/>
        protected override Task SelfDispose()
        {
            return Task.CompletedTask;
        }
    }
}