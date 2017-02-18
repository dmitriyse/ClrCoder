// <copyright file="IUnitOfWorkImpl.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel
{
    using Threading;

    /// <summary>
    /// Unit of work implementation. Classes <see cref="IUnitOfWork"/> are implemented usually by transparent proxy to this
    /// contract.
    /// </summary>
    public interface IUnitOfWorkImpl : IExceptionalAsyncDisposable
    {
        /// <summary>
        /// Gets repository of the specified type.
        /// </summary>
        /// <typeparam name="T">Repository type.</typeparam>
        /// <returns>Repository instance of asked type.</returns>
        T GetRepository<T>() where T : class, IRepository;
    }
}