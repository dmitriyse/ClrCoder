// <copyright file="IImmutableState{T}.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    using JetBrains.Annotations;

    /// <summary>
    /// Provides runtime immutability state information for part/aspect of an instance that implements this abstraction.
    /// </summary>
    /// <typeparam name="T">The types that is used to binds part/aspect of an instance with immutability state.</typeparam>
    [PublicAPI]
    public interface IImmutableState<out T>
    {
        /// <summary>
        /// Shows if part/state of an instance is deeply immutable.
        /// </summary>
        bool IsImmutable { get; }

        /// <summary>
        /// Shows if part/state of an instance is shallow immutable.
        /// </summary>
        bool IsShallowImmutable { get; }
    }
}