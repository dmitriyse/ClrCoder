// <copyright file="IFreezable{T}.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    /// <summary>
    /// Allows to move part of a type into immutable state.
    /// </summary>
    /// <typeparam name="T">Part of a object that can be frozen.</typeparam>
    public interface IFreezable<out T> : IImmutableState<T>
    {
        /// <summary>
        /// Deeply freezes part/aspect of an instance.
        /// </summary>
        void Freeze(bool shallowOnly);
    }
}