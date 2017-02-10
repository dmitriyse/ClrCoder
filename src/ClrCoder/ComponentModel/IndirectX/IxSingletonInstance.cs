// <copyright file="IxSingletonInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Instance controlled by <see cref="IxSingletonProvider"/>.
    /// </summary>
    public class IxSingletonInstance : IxInstance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxSingletonInstance"/> class.
        /// </summary>
        /// <param name="providerNode">Singleton provider.</param>
        /// <param name="parentInstance">Parent instance.</param>
        public IxSingletonInstance(IxProviderNode providerNode, IIxInstance parentInstance)
            : base(providerNode, parentInstance)
        {
            if (parentInstance == null)
            {
                throw new ArgumentNullException(nameof(parentInstance));
            }
        }

        /// <inheritdoc/>
        protected override Task SelfDispose()
        {
            return ProviderNode.DisposeHandler(Object);
        }
    }
}