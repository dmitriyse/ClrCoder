// <copyright file="IDeepCloneable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ObjectModel
{
    using JetBrains.Annotations;

    /// <summary>
    /// Typed cloneable contract.
    /// </summary>
    /// <typeparam name="T">Type of clones.</typeparam>
    [PublicAPI]
    public interface IDeepCloneable<out T>
        where T : IDeepCloneable<T>
    {
        /// <summary>
        /// Creates a new <c>object</c> that is a deep copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new <c>object</c> that is a deep copy of <see langword="this"/> instance.
        /// </returns>
        T Clone();
    }
}