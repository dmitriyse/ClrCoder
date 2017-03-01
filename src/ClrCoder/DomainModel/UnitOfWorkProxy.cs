// <copyright file="UnitOfWorkProxy.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    /// <summary>
    /// Base implementation of unit of work.
    /// </summary>
    public abstract class UnitOfWorkProxy : IUnitOfWork
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkProxy"/> class.
        /// </summary>
        /// <param name="impl">Unit of work implementation.</param>
        protected UnitOfWorkProxy(IUnitOfWorkImpl impl)
        {
            if (impl == null)
            {
                throw new ArgumentNullException(nameof(impl));
            }

            Impl = impl;
        }

        /// <inheritdoc/>
        public Task DisposeTask => Impl.DisposeTask;

        /// <summary>
        /// Actual unit of work implementation.
        /// </summary>
        protected IUnitOfWorkImpl Impl { get; }

        /// <inheritdoc/>
        public Task Commit()
        {
            return Impl.Commit();
        }

        /// <inheritdoc/>
        public void HandleException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Impl.HandleException(exception);
        }

        /// <inheritdoc/>
        public void StartDispose()
        {
            Impl.StartDispose();
        }
    }

    /// <summary>
    /// Proxy for Unit of Work with 1 repository.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Reviewed. Suppression is OK here.")]
    public class UnitOfWorkProxy<TR1> : UnitOfWorkProxy, IUnitOfWork<TR1>
        where TR1 : class, IRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkProxy{TR1}"/> class.
        /// </summary>
        /// <param name="impl">Unit of work implementation.</param>
        internal UnitOfWorkProxy(IUnitOfWorkImpl impl)
            : base(impl)
        {
        }

        /// <inheritdoc/>
        public TR1 R1 => Impl.GetRepository<TR1>();
    }

    /// <summary>
    /// Proxy for Unit of Work with 2 repositories.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    public class UnitOfWorkProxy<TR1, TR2> : UnitOfWorkProxy, IUnitOfWork<TR1, TR2>
        where TR1 : class, IRepository
        where TR2 : class, IRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkProxy{TR1, TR2}"/> class.
        /// </summary>
        /// <param name="impl">Unit of work implementation.</param>
        internal UnitOfWorkProxy(IUnitOfWorkImpl impl)
            : base(impl)
        {
        }

        /// <inheritdoc/>
        public TR1 R1 => Impl.GetRepository<TR1>();

        /// <inheritdoc/>
        public TR2 R2 => Impl.GetRepository<TR2>();
    }

    /// <summary>
    /// Proxy for Unit of Work with 3 repositories.
    /// </summary>
    /// <typeparam name="TR1">Type of repository 1.</typeparam>
    /// <typeparam name="TR2">Type of repository 2.</typeparam>
    /// <typeparam name="TR3">Type of repository 3.</typeparam>
    public class UnitOfWorkProxy<TR1, TR2, TR3> : UnitOfWorkProxy, IUnitOfWork<TR1, TR2, TR3>
        where TR1 : class, IRepository
        where TR2 : class, IRepository
        where TR3 : class, IRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkProxy{TR1, TR2, TR3}"/> class.
        /// </summary>
        /// <param name="impl">Unit of work implementation.</param>
        internal UnitOfWorkProxy(IUnitOfWorkImpl impl)
            : base(impl)
        {
        }

        /// <inheritdoc/>
        public TR1 R1 => Impl.GetRepository<TR1>();

        /// <inheritdoc/>
        public TR2 R2 => Impl.GetRepository<TR2>();

        /// <inheritdoc/>
        public TR3 R3 => Impl.GetRepository<TR3>();
    }
}