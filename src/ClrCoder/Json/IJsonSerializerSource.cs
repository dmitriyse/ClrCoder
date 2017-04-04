// <copyright file="IJsonSerializerSource.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Json
{
    using JetBrains.Annotations;

    using Newtonsoft.Json;

    /// <summary>
    /// Helps to work with serializers and settings.
    /// </summary>
    [PublicAPI]
    public interface IJsonSerializerSource
    {
        /// <summary>
        /// Returns cached serializer settings.
        /// </summary>
        [NotNull]
        JsonSerializerSettings Settings { get; }

        /// <summary>
        /// Returns serializer for based on current settings.
        /// </summary>
        [NotNull]
        JsonSerializer Serializer { get; }

        /// <summary>
        /// Creates new instance of the serializer settings.
        /// </summary>
        /// <returns>New instance of the <see cref="JsonSerializerSettings"/>.</returns>
        JsonSerializerSettings CreateSettings();
    }
}