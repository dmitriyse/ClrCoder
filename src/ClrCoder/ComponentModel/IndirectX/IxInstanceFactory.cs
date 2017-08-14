// <copyright file="IxInstanceFactory.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Instance factory.
    /// </summary>
    /// <remarks>
    /// Instance Builder - constructs some how instance, may require dependencies, do some proxying.
    /// Instance Factory - is just async method that performs instantiation using already prepared instance prerequisites.
    /// (It's not true).
    /// TODO: Make conception clear.
    /// </remarks>
    public class IxInstanceFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxInstanceFactory"/> class.
        /// </summary>
        /// <param name="factory">Function that performs actual instantiation.</param>
        /// <param name="instanceBaseType">Base type of instantiated object. Can be null in a case where it can be different.</param>
        public IxInstanceFactory(IxInstanceFactoryDelegate factory, [CanBeNull] Type instanceBaseType)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Factory = factory;
            InstanceBaseType = instanceBaseType;
        }

        /// <summary>
        /// Function that performs actual instantiation.
        /// </summary>
        public IxInstanceFactoryDelegate Factory { get; }

        /// <summary>
        /// Base type of instantiated <c>object</c>. Can be <see langword="null"/> in a case where it can be different.
        /// </summary>
        [CanBeNull]
        public Type InstanceBaseType { get; }
    }

    /// <summary>
    /// Raw <c>instance</c> factory <c>delegate</c>. No any registrations just obtain <c>instance</c> according to config.
    /// </summary>
    /// <param name="instance">Instance that will <c>finally</c> receive created <c>object</c>.</param>
    /// <param name="parentInstance">Parent <c>instance</c>.</param>
    /// <param name="context"><c>Resolve</c> <c>context</c>.</param>
    /// <param name="frame">The resolve frame in the dependency sequence.</param>
    /// <returns>Async execution task result.</returns>
    public delegate ValueTask<object> IxInstanceFactoryDelegate(
        IIxInstance instance,
        IIxInstance parentInstance,
        IxHost.IxResolveContext context,
        [CanBeNull] IxResolveFrame frame);
}