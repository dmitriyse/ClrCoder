// <copyright file="ImmutableStates.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    /// <summary>
    /// Immutable state flags.
    /// </summary>
    [Flags]
    public enum ImmutableStates
    {
        /// <summary>
        /// An object or aspect of an object is shallow immutable.
        /// </summary>
        ShallowImmutable = 0b0001,

        /// <summary>
        /// An object or an aspect of an object is immutable.
        /// </summary>
        Immutable = 0b0010 | ShallowImmutable,

        /// <summary>
        /// Assumption was made that an object or an aspect of an object is immutable.
        /// </summary>
        AssumedShallowImmutable = 0x0100,

        /// <summary>
        /// Assumption was made that an object or an aspect of an object is shallow immutable.
        /// </summary>
        AssumedImmutable = 0x1000 | AssumedShallowImmutable
    }
}