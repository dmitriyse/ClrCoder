// <copyright file="IFreezable{T}.cs" company="PixSee">
// Copyright (c) PixSee. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    /// <summary>
    /// Freezable object, that can freezes to the specified type.
    /// </summary>
    /// <typeparam name="T">Frozen type.</typeparam>
    public interface IFreezable<out T>
    {
        /// <summary>
        /// Current state, frozen or not.
        /// </summary>
        bool IsFrozen { get; }
    }
}