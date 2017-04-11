// <copyright file="PersistenceProxy.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel
{
    /// <summary>
    /// Proxy that wraps <see cref="IPersistenceImpl"/> in generic form.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    public class PersistenceProxy<TR1> : IPersistence<TR1>
        where TR1 : class, IRepository
    {
        private readonly IPersistenceImpl _impl;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceProxy{TR1}"/> class.
        /// </summary>
        /// <param name="impl">Persistence implementation.</param>
        public PersistenceProxy(IPersistenceImpl impl)
        {
            _impl = impl;
        }

        /// <inheritdoc/>
        public IUnitOfWork<TR1> OpenUnitOfWork()
        {
            return new UnitOfWorkProxy<TR1>(_impl.OpenUnitOfWork());
        }
    }

    /// <summary>
    /// Proxy that wraps <see cref="IPersistenceImpl"/> in generic form.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    public class PersistenceProxy<TR1, TR2> : IPersistence<TR1, TR2>
        where TR1 : class, IRepository
        where TR2 : class, IRepository
    {
        private readonly IPersistenceImpl _impl;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceProxy{TR1, TR2}"/> class.
        /// </summary>
        /// <param name="impl">Persistence implementation.</param>
        public PersistenceProxy(IPersistenceImpl impl)
        {
            _impl = impl;
        }

        /// <inheritdoc/>
        public IUnitOfWork<TR1, TR2> OpenUnitOfWork()
        {
            return new UnitOfWorkProxy<TR1, TR2>(_impl.OpenUnitOfWork());
        }
    }

    /// <summary>
    /// Proxy that wraps <see cref="IPersistenceImpl"/> in generic form.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    /// <typeparam name="TR3">Type of repository 3.</typeparam>
    public class PersistenceProxy<TR1, TR2, TR3> : IPersistence<TR1, TR2, TR3>
        where TR1 : class, IRepository
        where TR2 : class, IRepository
        where TR3 : class, IRepository
    {
        private readonly IPersistenceImpl _impl;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceProxy{TR1, TR2, TR3}"/> class.
        /// </summary>
        /// <param name="impl">Persistence implementation.</param>
        public PersistenceProxy(IPersistenceImpl impl)
        {
            _impl = impl;
        }

        /// <inheritdoc/>
        public IUnitOfWork<TR1, TR2, TR3> OpenUnitOfWork()
        {
            return new UnitOfWorkProxy<TR1, TR2, TR3>(_impl.OpenUnitOfWork());
        }
    }

    /// <summary>
    /// Proxy that wraps <see cref="IPersistenceImpl"/> in generic form.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    /// <typeparam name="TR3">Type of repository 3.</typeparam>
    /// <typeparam name="TR4">Type of repository 4.</typeparam>
    public class PersistenceProxy<TR1, TR2, TR3, TR4> : IPersistence<TR1, TR2, TR3, TR4>
        where TR1 : class, IRepository
        where TR2 : class, IRepository
        where TR3 : class, IRepository
        where TR4 : class, IRepository
    {
        private readonly IPersistenceImpl _impl;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceProxy{TR1, TR2, TR3, TR4}"/> class.
        /// </summary>
        /// <param name="impl">Persistence implementation.</param>
        public PersistenceProxy(IPersistenceImpl impl)
        {
            _impl = impl;
        }

        /// <inheritdoc/>
        public IUnitOfWork<TR1, TR2, TR3, TR4> OpenUnitOfWork()
        {
            return new UnitOfWorkProxy<TR1, TR2, TR3, TR4>(_impl.OpenUnitOfWork());
        }
    }

}