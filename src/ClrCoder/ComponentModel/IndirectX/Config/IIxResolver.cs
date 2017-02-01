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
    public interface IIxResolver
    {
        /// <summary>
        /// Resolves <c>object</c> with the specified type and <c>name</c>.
        /// </summary>
        /// <typeparam name="TContract">Required <c>object</c> contract.</typeparam>
        /// <param name="name">Additional distinguish <c>name</c>.</param>
        /// <returns>Required <c>object</c>.</returns>
        Task<TContract> Resolve<TContract>(string name = null);
    }
}