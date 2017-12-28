// <copyright file="IImmutableState.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    /// <summary>
    /// Contract that controls immutability of hole instance.
    /// </summary>
    public interface IImmutableState
    {
        /// <summary>
        /// State of the object or the aspect of the object.
        /// </summary>
        ImmutableStates State { get; }

        /// <summary>
        /// On validation assumes that part/aspect of object is immutable.
        /// </summary>
        /// <param name="onlyShallowImmutable">
        /// If the argument is true, than assumption is made only on shallow immutability,
        /// otherwise assumption will been maden on full immutability.
        /// </param>
        void AssumeImmutable(bool onlyShallowImmutable = false);
    }
}