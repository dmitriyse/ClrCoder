// <copyright file="IIxResolver.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Resolver API, the only point where hosted <c>object</c> can be instantiated.
    /// </summary>
    /// <remarks>
    /// If you use IIxResolve inside constructor you should wait result also in the constructor.
    /// For better usage you should avoid using resolver inside the constructor.
    /// </remarks>
    public interface IIxResolver
    {
        /// <summary>
        /// Resolves <c>object</c> with the specified type and <c>name</c>.
        /// </summary>
        /// <param name="identifier">Dependency <c>identifier</c>.</param>
        /// <param name="arguments">Resolve arguments.</param>
        /// <returns>Required <c>object</c>.</returns>
        ValueTask<IIxInstanceLock> Resolve(
            IxIdentifier identifier,
            IReadOnlyDictionary<IxIdentifier, object> arguments = null);
    }
}