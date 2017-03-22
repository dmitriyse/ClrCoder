// <copyright file="FlurlHttpExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Net.Http
{
#if NET46 || NETSTANDARD1_6
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    using Flurl;
    using Flurl.Http;

    using JetBrains.Annotations;

    using Validation;

    /// <summary>
    /// Flurl.Http related extensions.
    /// </summary>
    [PublicAPI]
    public static class FlurlHttpExtensions
    {
        /// <summary>
        /// Posts binary data to the url.
        /// </summary>
        /// <param name="client">Flurl fluent syntax.</param>
        /// <param name="content">Binary content to send.</param>
        /// <param name="setHeadersAction">Action that setups headers.</param>
        /// <param name="cancellationToken">Send cancellation token.</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostBytesAsync(
            this IFlurlClient client,
            byte[] content,
            Action<HttpContentHeaders> setHeadersAction = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VxArgs.NotNull(client, nameof(client));
            VxArgs.NotNull(content, nameof(content));

            var httpContent = new ByteArrayContent(content);

            setHeadersAction?.Invoke(httpContent.Headers);

            return client.PostAsync(httpContent, cancellationToken);
        }

        /// <summary>
        /// Posts binary data to the url.
        /// </summary>
        /// <param name="url">Flurl url.</param>
        /// <param name="content">Binary content to send.</param>
        /// <param name="setHeadersAction">Action that setups headers.</param>
        /// <param name="cancellationToken">Send cancellation token.</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostBytesAsync(
            this Url url,
            byte[] content,
            Action<HttpContentHeaders> setHeadersAction = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VxArgs.NotNull(url, nameof(url));
            VxArgs.NotNull(content, nameof(content));

            return PostBytesAsync(new FlurlClient(url), content, setHeadersAction, cancellationToken);
        }
    }

#endif
}