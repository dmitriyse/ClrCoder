using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClrCoder.System.Net.Http
{
    /// <summary>
    /// Extensions fro the http client api.
    /// </summary>
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
}