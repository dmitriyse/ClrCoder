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
    [PublicAPI]
    public interface IJsonLogger
    {
        /// <summary>
        /// Asynchronous handler, that should be used to generate and process log messages.
        /// </summary>
        [NotNull]
        IAsyncHandler AsyncHandler { get; }

        /// <summary>
        /// Object that represents log <c>entry</c>.
        /// </summary>
        /// <param name="entry">JObject, string with Json, or any other type derived from <see cref="JLogEntry"/>.</param>
        void Log([NotNull] object entry);
    }
}