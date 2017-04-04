// <copyright file="ILogEntryBuilder.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System;

    using JetBrains.Annotations;

    using Json;

    /// <summary>
    /// Log entry fluent syntax building block.
    /// </summary>
    public interface ILogEntryBuilder
    {
        /// <summary>
        /// Serializer source.
        /// </summary>
        [ReferenceImmutable]
        [NotNull]
        IJsonSerializerSource SerializerSource { get; }

        /// <summary>
        /// Builds log <c>entry</c>.
        /// </summary>
        /// <param name="entry">Current log <c>entry</c> state.</param>
        /// <returns>Modified log <c>entry</c>.</returns>
        [NotNull]
        object Build([NotNull] object entry);
    }
}