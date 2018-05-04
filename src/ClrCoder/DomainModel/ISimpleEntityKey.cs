// <copyright file="ISimpleEntityKey.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel
{
    /// <summary>
    /// The one-field simple entity key.
    /// </summary>
    /// <typeparam name="T">The type of the key.</typeparam>
    public interface ISimpleEntityKey<out T>
    {
        /// <summary>
        /// The key data field.
        /// </summary>
        T Id { get; }
    }
}