// <copyright file="IJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Logging
{
    using JetBrains.Annotations;

    using Threading;

    /// <summary>
    /// Main logging abstraction.
    /// </summary>
    /// <remarks>
    /// This logger is designed to be used with <see cref="DeferredHandler"/> and not contains
    /// </remarks>
    [PublicAPI]
    public interface IJsonLogger
    {
        /// <summary>
        /// Object that represents log entry.
        /// </summary>
        /// <param name="obj">JObject, string with Json, or any other type that will be serialized to json string.</param>
        void Log([NotNull] object obj);
    }
}