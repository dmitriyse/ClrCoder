// <copyright file="IIxInstanceLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    public interface IIxInstanceLock : IDisposable
    {
        IIxInstance Target { get; }

        /// <summary>
        /// Signals that locked <c>object</c> goes to disposing state.
        /// </summary>
        void PulseDispose();
    }
}