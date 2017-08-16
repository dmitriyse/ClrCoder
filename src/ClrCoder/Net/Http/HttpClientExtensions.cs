// <copyright file="HttpClientExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Net.Http
{
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Extensions fro the http client API.
    /// </summary>
    [PublicAPI]
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Clones request <c>message</c>.
        /// </summary>
        /// <param name="message">Message to clone.</param>
        /// <returns>Message clone.</returns>
        public static async Task<HttpRequestMessage> Clone(this HttpRequestMessage message)
        {
            var newMessage = new HttpRequestMessage(message.Method, message.RequestUri);
            foreach (KeyValuePair<string, IEnumerable<string>> header in message.Headers)
            {
                newMessage.Headers.Add(header.Key, header.Value);
            }

            if (message.Content != null)
            {
                newMessage.Content = await message.Content.Clone();
            }

            foreach (KeyValuePair<string, object> property in message.Properties)
            {
                newMessage.Properties.Add(property.Key, property.Value);
            }

            return newMessage;
        }

        /// <summary>
        /// Clones http <c>content</c>.
        /// </summary>
        /// <param name="content">Content to clone.</param>
        /// <returns>Cloned <c>content</c>.</returns>
        public static async Task<HttpContent> Clone(this HttpContent content)
        {
            var clonedContent = new ByteArrayContent(await content.ReadAsByteArrayAsync());
            foreach (KeyValuePair<string, IEnumerable<string>> httpContentHeader in content.Headers)
            {
                clonedContent.Headers.Add(httpContentHeader.Key, httpContentHeader.Value);
            }

            return clonedContent;
        }
    }
#endif
}