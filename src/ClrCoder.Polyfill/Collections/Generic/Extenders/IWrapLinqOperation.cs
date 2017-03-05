// <copyright file="IWrapLinqOperation.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    using Linq;

    /// <summary>
    /// Contract that marks object as an extender.
    /// </summary>
    internal interface IWrapLinqOperation : ITransitiveSourceLinqOperation
    {
        /// <summary>
        /// Extender assumes that collection is immutable anyway.
        /// </summary>
        bool AssumeImmutable { get; }

        /// <summary>
        /// Extender assumes that collection is read only anyway.
        /// </summary>
        bool AssumeIsReadOnly { get; }

        /// <summary>
        /// Extender assumes that this comparer is used.
        /// </summary>
        [CanBeNull]
        object Comparer { get; }

        /// <summary>
        /// Allows to override is immutable.
        /// </summary>
        [CanBeNull]
        Func<bool> IsImmutableOverride { get; set; }
    }
}