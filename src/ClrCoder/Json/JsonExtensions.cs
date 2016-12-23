// <copyright file="JsonExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;

    using JetBrains.Annotations;

    using Newtonsoft.Json;

    /// <summary>
    /// Json.Net ralated extension methods.
    /// </summary>
    [PublicAPI]
    public static class JsonExtensions
    {
        /// <summary>
        /// Clones <see cref="JsonSerializer"/>.
        /// </summary>
        /// <param name="serializer">Original <c>serializer</c>.</param>
        /// <returns>Cloned serializer.</returns>
        [NotNull]
        public static JsonSerializer Clone([NotNull] this JsonSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            var clonedSerializer = new JsonSerializer
                                       {
                                           Binder = serializer.Binder,
                                           CheckAdditionalContent = serializer.CheckAdditionalContent,
                                           ConstructorHandling = serializer.ConstructorHandling,
                                           Context = serializer.Context,
                                           ContractResolver = serializer.ContractResolver,
                                           Culture = serializer.Culture,
                                           DateFormatHandling = serializer.DateFormatHandling,
                                           DateFormatString = serializer.DateFormatString,
                                           DateParseHandling = serializer.DateParseHandling,
                                           DateTimeZoneHandling = serializer.DateTimeZoneHandling,
                                           DefaultValueHandling = serializer.DefaultValueHandling,
                                           EqualityComparer = serializer.EqualityComparer,
                                           FloatFormatHandling = serializer.FloatFormatHandling,
                                           FloatParseHandling = serializer.FloatParseHandling,
                                           Formatting = serializer.Formatting,
                                           MaxDepth = serializer.MaxDepth,
                                           MetadataPropertyHandling = serializer.MetadataPropertyHandling,
                                           MissingMemberHandling = serializer.MissingMemberHandling,
                                           NullValueHandling = serializer.NullValueHandling,
                                           ObjectCreationHandling = serializer.ObjectCreationHandling,
                                           PreserveReferencesHandling = serializer.PreserveReferencesHandling,
                                           ReferenceLoopHandling = serializer.ReferenceLoopHandling,
                                           ReferenceResolver = serializer.ReferenceResolver,
                                           StringEscapeHandling = serializer.StringEscapeHandling,
                                           TraceWriter = serializer.TraceWriter,
                                           TypeNameAssemblyFormat = serializer.TypeNameAssemblyFormat,
                                           TypeNameHandling = serializer.TypeNameHandling
                                       };

            foreach (var converter in serializer.Converters)
            {
                clonedSerializer.Converters.Add(converter);
            }

            return clonedSerializer;
        }
    }
}