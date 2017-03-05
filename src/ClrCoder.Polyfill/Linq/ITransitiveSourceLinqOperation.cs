// <copyright file="ITransitiveSourceLinqOperation.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Linq
{
    using Collections;

    using JetBrains.Annotations;

    /// <summary>
    /// Operation that performs items transformation and transit source of operation.
    /// </summary>
    public interface ITransitiveSourceLinqOperation
    {
        /// <summary>
        /// Source operation that is transited to next operation.
        /// </summary>
        [NotNull]
        IEnumerable Source { get; }
    }
}