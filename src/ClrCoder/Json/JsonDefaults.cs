// <copyright file="JsonDefaults.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Json
{
    using System;

    using Annotations;

    using JetBrains.Annotations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    using NodaTime;
    using NodaTime.Serialization.JsonNet;

    using Validation;

    /// <summary>
    /// Default Json.Net serializer settings for different cases.
    /// </summary>
    [PublicAPI]
    public static class JsonDefaults
    {
        private static readonly Func<JsonSerializerSettings> CreateBaseSettingsFunc = () =>
            {
                var settings = new JsonSerializerSettings();

                // Using standard javascript naming convention.
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                // Ignoring non specified properties from output.
                settings.NullValueHandling = NullValueHandling.Ignore;

                // Using IANA tokens for time zones for better compatibility with all world (non only ms-*).
                settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

                settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

                return settings;
            };

        private static IJsonSerializerSource _baseSerializerSource = new JsonSerializerSource(CreateBaseSettingsFunc);

        private static IJsonSerializerSource _jsonConfigSerializerSource = new JsonSerializerSource(
            () =>
                {
                    JsonSerializerSettings settings = _baseSerializerSource.CreateSettings();

                    // Using human readable formatting.
                    settings.Formatting = Formatting.Indented;

                    return settings;
                });

        private static IJsonSerializerSource _restRpcSerializerSource = new JsonSerializerSource(
            () =>
                {
                    JsonSerializerSettings settings = _baseSerializerSource.CreateSettings();

                    settings.MissingMemberHandling = MissingMemberHandling.Error;

                    return settings;
                });

        /// <summary>
        /// Creates base settings. This member is different from <see cref="JsonConvert.DefaultSettings"/>.
        /// </summary>
        /// <remarks>
        /// This settings represents convention. Every change should be commented with motivation.
        /// </remarks>
        [NotNull]
        [ThreadUnsafe]
        public static IJsonSerializerSource BaseSerializerSource
        {
            get => _baseSerializerSource;
            set
            {
                JsonConvert.DefaultSettings();
                VxArgs.NotNull(value, nameof(value));
                _baseSerializerSource = value;
            }
        }

        /// <summary>
        /// Creates default settings for Json configuration files.
        /// </summary>
        /// <remarks>
        /// This settings represents convention. Every change should be commented with motivation.
        /// </remarks>
        [NotNull]
        [ThreadUnsafe]
        public static IJsonSerializerSource JsonConfigSerializerSource
        {
            get => _jsonConfigSerializerSource;
            set
            {
                VxArgs.NotNull(value, nameof(value));
                _jsonConfigSerializerSource = value;
            }
        }

        /// <summary>
        /// Default serializer source for the REST JSON RPC.
        /// </summary>
        /// <remarks>
        /// This settings represents convention. Every change should be commented with motivation.
        /// </remarks>
        [NotNull]
        [ThreadUnsafe]
        public static IJsonSerializerSource RestRpcSerializerSource
        {
            get => _restRpcSerializerSource;
            set
            {
                VxArgs.NotNull(value, nameof(value));
                _restRpcSerializerSource = value;
            }
        }
    }
}