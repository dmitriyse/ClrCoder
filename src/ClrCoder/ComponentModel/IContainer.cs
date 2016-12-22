// <copyright file="IContainer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.ComponentModel
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// <c>Container</c> contract.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Actual container config.
        /// </summary>
        IContainerConfig Config { get; }

        /// <summary>
        /// Parent container.
        /// </summary>
        [CanBeNull]
        IContainer Parent { get; }

        /// <summary>
        /// Resolves dependency by <c>key</c>.
        /// </summary>
        /// <param name="key">Node <c>key</c> to resolve.</param>
        /// <returns>Lease to container node.</returns>
        Task<IContainerLease> Resolve(ContainerNodeKey key);

        /// <summary>
        /// Updates container <c>config</c>.
        /// </summary>
        /// <param name="config">New container <c>config</c>.</param>
        void UpdateConfig(IContainerConfig config);
    }
}