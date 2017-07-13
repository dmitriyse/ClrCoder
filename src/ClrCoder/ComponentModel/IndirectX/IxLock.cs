// <copyright file="IxLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Reflection;

    using Annotations;

    using JetBrains.Annotations;

    /// <summary>
    /// Locks <c>object</c> resolved from IndirectX container.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    public class IxLock<T> : IDisposable
    {
        [CanBeNull]
        private readonly T _target;

        /// <summary>
        /// Can be <see langword="null"/> in a case where it is default value.
        /// </summary>
        [CanBeNull]
        private readonly IIxInstanceLock _instanceLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="IxLock{T}"/> class.
        /// </summary>
        /// <param name="instanceLock">IndirectX instance lock.</param>
        [InvalidUsageIsCritical]
        public IxLock(IIxInstanceLock instanceLock)
        {
            Critical.Assert(instanceLock != null, "Lock target should not be null.");

            _instanceLock = instanceLock;
            _target = (T)_instanceLock.Target.Object;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IxLock{T}"/> class.
        /// </summary>
        /// <remarks>
        /// HACK: Remove me please!
        /// Currently only used to get Cluster reference to pseudo remote object.
        /// </remarks>
        /// <param name="target">The lock target.</param>
        [InvalidUsageIsCritical]
        public IxLock(T target)
        {
            Critical.CheckedAssert(
                typeof(T).GetTypeInfo().IsClass,
                "Cluster reference hack can be created only for reference types.");
            _target = target;
        }

        /// <summary>
        /// Locked <c>object</c> with not-null assurance.
        /// </summary>
        [NotNull]
        public T Target
        {
            get
            {
                if (_target == null)
                {
                    throw new NullReferenceException(
                        "You cannot use null-resolved object in the non-null expressions.");
                }

                return _target;
            }
        }

        /// <summary>
        /// Locked <c>object</c> allowed to be null.
        /// </summary>
        [CanBeNull]
        public T TargetUnsafe { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            try
            {
                try
                {
                    _instanceLock?.Dispose();
                }
                catch (Exception ex) when (ex.IsProcessable())
                {
                    Critical.Assert(false, "Dispose should not raise any error.");
                }
            }
            catch
            {
                // Dispose should decouple errors propagation.
                // Usually dispose errors relates only to disposing component, but not to it's caller.
            }
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
            return _instanceLock?.GetHashCode() ?? typeof(T).GetHashCode();
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