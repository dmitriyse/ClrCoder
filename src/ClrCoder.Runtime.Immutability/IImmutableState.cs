// <copyright file="IImmutableState.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    /// <summary>
    /// Alias for <see cref="IImmutableState{object}"/> provides immutability state of all parts/aspect of an instance.
    /// </summary>
    public interface IImmutableState : IImmutableState<object>
    {
    }
}