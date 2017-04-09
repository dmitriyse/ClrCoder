// <copyright file="IKeyed.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ObjectModel
{
    using JetBrains.Annotations;

    /// <summary>
    /// Keyed <c>object</c>.
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    public interface IKeyed<out TKey>
    {
        /// <summary>
        /// Key value. Used to identify <c>object</c>.
        /// </summary>
        [NotNull]
        TKey Key { get; }
    }
}