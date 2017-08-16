﻿// <copyright file="IxScopeInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Threading;

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
        /// <param name="creatorTempLock">First temp lock for the creator of a new instance.</param>
        public IxScopeInstance(
            IxProviderNode providerNode,
            [CanBeNull] IIxInstance parentInstance,
            out IIxInstanceLock creatorTempLock)
            : base(providerNode, parentInstance, out creatorTempLock)
        {
            SetObjectCreationTask(new ValueTask<object>(providerNode));
        }

        /// <inheritdoc/>
        protected override Task SelfDispose()
        {
            return TaskEx.CompletedTask;
        }
    }
}