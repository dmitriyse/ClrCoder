// <copyright file="Container.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.ComponentModel
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Container implementation.
    /// </summary>
    public class Container : IContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="parent">Parent container.</param>
        public Container([CanBeNull] IContainer parent)
        {
            Parent = parent;

            // Empty config.
            Config = new ContainerConfig();
        }

        /// <inheritdoc/>
        public IContainerConfig Config { get; private set; }

        /// <inheritdoc/>
        public IContainer Parent { get; }

        /// <inheritdoc/>
        public Task<IContainerLease> Resolve(ContainerNodeKey key)
        {
            if (!(key != default(ContainerNodeKey)))
            {
                Vx.Throw(key);
            }

            // Do magic.
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void UpdateConfig(IContainerConfig config)
        {
            // Do magic.
            Config = config;
        }
    }
}