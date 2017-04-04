// <copyright file="JsonSerializerSource.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;

    using Json;

    using Newtonsoft.Json;

    /// <summary>
    /// Base serializer source with caching.
    /// </summary>
    [Immutable]
    public class JsonSerializerSource : IJsonSerializerSource
    {
        private readonly Func<JsonSerializerSettings> _createSettingsFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializerSource"/> class.
        /// </summary>
        /// <param name="createSettingsFunc">Factory for serializer settings.</param>
        public JsonSerializerSource(Func<JsonSerializerSettings> createSettingsFunc)
        {
            _createSettingsFunc = createSettingsFunc;
            if (createSettingsFunc == null)
            {
                throw new ArgumentNullException(nameof(createSettingsFunc));
            }
        }

        /// <inheritdoc/>
        public JsonSerializerSettings Settings => _createSettingsFunc();

        /// <inheritdoc/>
        public JsonSerializer Serializer => JsonSerializer.Create(_createSettingsFunc());

        /// <inheritdoc/>
        public JsonSerializerSettings CreateSettings()
        {
            return _createSettingsFunc();
        }
    }
}