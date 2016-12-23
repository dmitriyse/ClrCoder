// <copyright file="IFreezable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ObjectModel
{
    /// <summary>
    /// <c>Freezable</c> <c>object</c>.
    /// </summary>
    /// <remarks>
    /// Once <c>object</c> is frozen, it cannot be not frozen any time later.
    /// </remarks>
    public interface IFreezable
    {
        /// <summary>
        /// Current state, frozen or not.
        /// </summary>
        bool IsFrozen { get; }

        /// <summary>
        /// Turns <c>object</c> into frozen state.
        /// </summary>
        void Freeze();
    }
}