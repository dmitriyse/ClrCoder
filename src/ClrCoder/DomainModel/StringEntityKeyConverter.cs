// <copyright file="StringEntityKeyConverter.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.DomainModel
{
    using System;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using Newtonsoft.Json;

    /// <summary>
    /// The json converter for the string entity keys.
    /// </summary>
    public class StringEntityKeyConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert([NotNull] Type objectType)
        {
            // TODO: Speedup me.
            return objectType.GetInterfaces().Contains(typeof(ISimpleEntityKey<string>));
        }

        /// <inheritdoc/>
        public override object ReadJson(
            [NotNull] JsonReader reader,
            [NotNull] Type objectType,
            [CanBeNull] object existingValue,
            [NotNull] JsonSerializer serializer)
        {
            return reader.ReadAsString();
        }

        /// <inheritdoc/>
        public override void WriteJson(
            [NotNull] JsonWriter writer,
            [CanBeNull] object value,
            [NotNull] JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteValue(((ISimpleEntityKey<string>)value).Id);
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}
#endif