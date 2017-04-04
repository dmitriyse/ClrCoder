// <copyright file="JsonExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Newtonsoft.Json;

    /// <summary>
    /// Json.Net ralated extension methods.
    /// </summary>
    [PublicAPI]
    public static class JsonExtensions
    {
        /// <summary>
        /// Deserializes file content to the type <typeparamref name="T"/> with the specifie deserializer.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="serializer">The json serializer.</param>
        /// <param name="fileName">The file name to deserialize from.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Deserialized data.</returns>
        public static async ValueTask<T> DeserializeFile<T>(this JsonSerializer serializer, string fileName, Encoding encoding = null)
        {
            // TODO: Replace to RecyclableMemoeryStream.
            MemoryStream memStream = new MemoryStream();
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                await fileStream.CopyToAsync(memStream);
            }

            memStream.Position = 0;

            using (var sr = encoding != null ? new StreamReader(memStream, encoding) : new StreamReader(memStream))
            {
                using (var jsonReader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<T>(jsonReader);
                }
            }
        }
    }
}