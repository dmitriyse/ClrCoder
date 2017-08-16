// <copyright file="ConvertEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using JetBrains.Annotations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using NodaTime;
    using NodaTime.Serialization.JsonNet;
#endif
    /// <summary>
    /// Conversion utilities.
    /// </summary>
    [PublicAPI]
    public static class ConvertEx
    {
        static ConvertEx()
        {
            DefaultTraceSettings = new JsonSerializerSettings
                                       {
                                           PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                                           Formatting = Formatting.Indented,
                                           ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                       };
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
            DefaultTraceSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
#endif
            DefaultTraceSettings.Converters.Add(new StringEnumConverter { CamelCaseText = false });
        }

        /// <summary>
        /// Default serializer settings designed to used for <c>object</c> tracing;
        /// </summary>
        public static JsonSerializerSettings DefaultTraceSettings { get; private set; }

        /// <summary>
        /// Serializes <c>object</c> to tracing json string.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>Object serialized to json string.</returns>
        [NotNull]
        public static string ToTraceJson([CanBeNull] object obj)
        {
            return JsonConvert.SerializeObject(obj, DefaultTraceSettings);
        }
    }
}