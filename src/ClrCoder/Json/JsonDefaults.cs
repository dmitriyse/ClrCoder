// <copyright file="JsonDefaults.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using JetBrains.Annotations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    using NodaTime;
    using NodaTime.Serialization.JsonNet;

    /// <summary>
    /// Default Json.Net serializer settings for different cases.
    /// </summary>
    [PublicAPI]
    public static class JsonDefaults
    {
        /// <summary>
        /// Default settings for Json configuration files. Always recreates settings instance.
        /// </summary>
        /// <remarks>
        /// This settings represents convention. Every change should be commented with motivation.
        /// </remarks>
        public static JsonSerializerSettings JsonConfigSerializerSettings
        {
            get
            {
                var settings = new JsonSerializerSettings();

                // Using human readable form.
                settings.Formatting = Formatting.Indented;

                // Using standard javascript naming convention.
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                // Ignoring non specified properties from output.
                settings.NullValueHandling = NullValueHandling.Ignore;

                // Using IANA tokens for time zones for better compatibility with all world (non only ms-*).
                settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

                settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

                return settings;
            }
        }

        /// <summary>
        /// Default settings for REST JSON RPC. Always recreates settings instance.
        /// </summary>
        /// <remarks>
        /// This settings represents convention. Every change should be commented with motivation.
        /// </remarks>
        public static JsonSerializerSettings JsonRestRpcSerializerSettings
        {
            get
            {
                var settings = new JsonSerializerSettings();

                // Using standard javascript naming convention.
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                // Ignoring non specified properties from output, for traffic optimization.
                settings.NullValueHandling = NullValueHandling.Ignore;

                // Using IANA tokens for time zones for better compatibility with all world (non only ms-*).
                settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

                // Forcing all statuses to be transfered as a string.
                settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

                settings.MissingMemberHandling = MissingMemberHandling.Error;

                return settings;
            }
        }
    }
}