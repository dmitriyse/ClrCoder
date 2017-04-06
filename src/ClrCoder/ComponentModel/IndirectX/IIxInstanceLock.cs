// <copyright file="IIxInstanceLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Lock on instance. Prevents instance from disposing.
    /// </summary>
    public interface IIxInstanceLock : IDisposable
    {
        /// <summary>
        /// The instance that is locked by this lock.
        /// </summary>
        [NotNull]
        IIxInstance Target { get; }

        /// <summary>
        /// The owner instance of this lock. Usually when owner disposes it releases this lock.
        /// </summary>
        IIxInstance Owner { get; }

        /// <summary>
        /// Signals that locked <c>object</c> goes to disposing state.
        /// </summary>
        void PulseDispose();
    }
}