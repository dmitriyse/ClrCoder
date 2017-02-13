// <copyright file="IIxResolver.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Threading.Tasks;

    /// <summary>
    /// Resolver API, the only point where hosted <c>object</c> can be instantiated.
    /// </summary>
    public interface IIxResolver : IIxInstance
    {
        /// <summary>
        /// Resolves <c>object</c> with the specified type and <c>name</c>.
        /// </summary>
        /// <param name="identifier">Dependency <c>identifier</c>.</param>
        /// <returns>Required <c>object</c>.</returns>
        Task<IIxInstanceLock> Resolve(IxIdentifier identifier);
    }
}