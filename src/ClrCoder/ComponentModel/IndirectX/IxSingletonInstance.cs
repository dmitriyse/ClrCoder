// <copyright file="IxSingletonInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

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
        /// <param name="context">The resolve context.</param>
        /// <param name="frame">The resolution frame in the resolve sequence.</param>
        /// <param name="creatorTempLock">First temp lock for the creator of a new instance.</param>
        public IxSingletonInstance(
            IxSingletonProvider providerNode,
            IIxInstance parentInstance,
            IxHost.IxResolveContext context,
            [CanBeNull] IxResolveFrame frame,
            out IIxInstanceLock creatorTempLock)
            : base(providerNode, parentInstance, out creatorTempLock)
        {
            if (parentInstance == null)
            {
                throw new ArgumentNullException(nameof(parentInstance));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var newFrame = new IxResolveFrame(frame, this);

            Debug.Assert(ProviderNode.InstanceFactory != null, "IxSingletonProvider always have instance factory.");

            SetObjectCreationTask(
                ProviderNode.InstanceFactory.Factory(
                    this,
                    parentInstance,
                    context,
                    newFrame));
        }

        /// <inheritdoc/>
        protected override Task SelfDispose()
        {
            return ProviderNode.DisposeHandler(Object);
        }
    }
}