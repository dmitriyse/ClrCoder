// <copyright file="JsonConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder
{
#if !PCL
    using System.IO;
#endif

    using JetBrains.Annotations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using NodaTime;
    using NodaTime.Serialization.JsonNet;

    /// <summary>
    /// Utils for json configuration files.
    /// </summary>
    [PublicAPI]
    public static class JsonConfig
    {
        static JsonConfig()
        {
#if !PCL
            Serializer.ConfigureForNodaTime(DateTimeZoneProviders.Bcl);
#else
            Serializer.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
#endif
        }

        /// <summary>
        /// Default serializer for json configuration files.
        /// </summary>
        public static JsonSerializer Serializer { get; set; } =
            JsonSerializer.Create(
                new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented, 
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

#if !PCL

        /// <summary>
        /// Loads configuration from json file.
        /// </summary>
        /// <typeparam name="T">C# class that corresponding to file content.</typeparam>
        /// <param name="fileName">File name with absolute or relative path.</param>
        /// <returns>Parsed configuration content.</returns>
        public static T Load<T>(string fileName)
        {
            using (var sr = new StreamReader(File.OpenRead(fileName)))
            using (var jsonReader = new JsonTextReader(sr))
            {
                return Serializer.Deserialize<T>(jsonReader);
            }
        }

#endif
    }
}