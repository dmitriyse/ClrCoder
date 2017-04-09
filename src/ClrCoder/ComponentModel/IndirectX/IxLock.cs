// <copyright file="IxLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Locks <c>object</c> resolved from IndirectX container.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    public class IxLock<T> : IDisposable
    {
        /// <summary>
        /// Can be <see langword="null"/> in a case where it is default value.
        /// </summary>
        [CanBeNull]
        private readonly IIxInstanceLock _instanceLock;

        private readonly T _target;

        /// <summary>
        /// Initializes a new instance of the <see cref="IxLock{T}"/> class.
        /// </summary>
        /// <param name="instanceLock">IndirectX instance lock.</param>
        public IxLock(IIxInstanceLock instanceLock)
        {
            if (instanceLock == null)
            {
                throw new ArgumentNullException(nameof(instanceLock));
            }

            _instanceLock = instanceLock;
            _target = (T)_instanceLock.Target.Object;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IxLock{T}"/> class.
        /// </summary>
        /// <remarks>
        /// HACK: Remove me please!
        /// </remarks>
        /// <param name="target">The lock target.</param>
        public IxLock(T target)
        {
            EnsureNotDefault();
            _target = target;
        }

        /// <summary>
        /// Locked <c>object</c>.
        /// </summary>
        public T Target
        {
            get
            {
                EnsureNotDefault();
                return _target;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            EnsureNotDefault();
            _instanceLock?.Dispose();
        }

        /// <inheritdoc/>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((IxLock<T>)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _instanceLock?.GetHashCode() ?? 0;
        }

        private void EnsureNotDefault()
        {
            if (_instanceLock == null)
            {
                throw new InvalidOperationException("Cannot use default value.");
            }
        }

        /// <summary>
        /// Checks equality.
        /// </summary>
        /// <param name="other">Other value.</param>
        /// <returns><see langword="true"/>, if equal.</returns>
        private bool Equals(IxLock<T> other)
        {
            return Equals(_instanceLock, other._instanceLock);
        }
    }
}