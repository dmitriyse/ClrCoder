// <copyright file="FlurlHttpExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Net.Http
{
#if NET46 || NETSTANDARD1_6
    using System;
    using System.Net;
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
        /// TODO: Document me.
        /// </summary>
        /// <param name="flurlException"></param>
        /// <returns></returns>
        public static bool IsCertificateError(this FlurlHttpException flurlException)
        {
            // core
            if (flurlException.Call.Response == null
                && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                && flurlException.InnerException?.InnerException?.Message ==
                "A security error occurred")
            {
                return true;
            }

            // net 4.6
            if (flurlException.Call.Response == null
                && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                && (flurlException.InnerException?.InnerException?.Message?.Contains("A security error occurred")
                    ?? false)
                && (flurlException.InnerException?.InnerException?.InnerException?.Message?.Contains("handshake failed")
                    ?? false))
            {
                return true;
            }

            // Linux
            if (flurlException.Call.Response == null
                && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                && (flurlException.InnerException?.InnerException?.Message?.Contains("SSL") ?? false))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// TODO: Document me.
        /// </summary>
        /// <param name="flurlException"></param>
        /// <returns></returns>
        public static bool IsDnsResolveError(this FlurlHttpException flurlException)
        {
            if (flurlException == null)
            {
                throw new ArgumentNullException(nameof(flurlException));
            }

            // net 4.6
            if (flurlException.Call.Response == null
                && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                && flurlException.InnerException.InnerException.Message != null
                && flurlException.InnerException.InnerException.Message.StartsWith(
                    "remote name could not be resolved"))
            {
                return true;
            }

            // core
            if (flurlException.Call.Response == null
                && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                && (flurlException.InnerException?.InnerException?.Message?.Contains(
                        "server name or address could not be resolved") ?? false))
            {
                return true;
            }

            // Linux
            return flurlException.Call.Response == null
                   && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                   && flurlException.InnerException?.InnerException?.Message != null
                   && flurlException.InnerException.InnerException.Message.Contains("resolve host name");
        }

        /// <summary>
        /// TODO: Document me.
        /// </summary>
        /// <param name="flurlException"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIpNotAvailable(this FlurlHttpException flurlException)
        {
            if (flurlException == null)
            {
                throw new ArgumentNullException(nameof(flurlException));
            }

            // net 4.6 does not exist IP
            if (flurlException.Call.Response == null
                && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                && flurlException.InnerException.InnerException.InnerException != null
                && flurlException.InnerException?.InnerException?.Message == "Unable to connect to the remote server"
                && flurlException.InnerException.InnerException.InnerException.Message.StartsWith(
                    "A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond")
            )
            {
                return true;
            }

            // net 4.6 locked IP
            if (flurlException.Call.Response == null
                && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                && flurlException.InnerException.InnerException.InnerException != null
                && flurlException.InnerException?.InnerException?.Message == "Unable to connect to the remote server"
                && flurlException.InnerException.InnerException.InnerException.Message.StartsWith(
                    "No connection could be made because the target machine actively refused it"))
            {
                return true;
            }

            // core does not exist IP
            if (flurlException.Call.Response == null
                && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                && flurlException.InnerException?.InnerException?.Message ==
                "The operation timed out")
            {
                return true;
            }

            // core locked IP
            if (flurlException.Call.Response == null
                && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                && flurlException.InnerException?.InnerException?.Message ==
                "A connection with the server could not be established")
            {
                return true;
            }

            // Linux does not exist IP
            // FlurlHttpException
            // Request to http://10.110.110.110/ failed. A task was canceled.
            // TaskCanceledException
            // A task was canceled.

            // Linux locked IP
            if (flurlException.Call.Response == null
                && flurlException.InnerException?.GetType() == typeof(HttpRequestException)
                && flurlException.InnerException?.InnerException?.Message ==
                "Couldn't connect to server")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// TODO: Document me.
        /// </summary>
        /// <param name="flurlException"></param>
        /// <param name="exceptionToken"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsTimeOutError(
            this FlurlHttpException flurlException,
            CancellationToken exceptionToken,
            CancellationToken token)
        {
            if (flurlException.Call.HttpStatus == HttpStatusCode.RequestTimeout)
            {
                return true;
            }

            // 4.6 and core and Linux
            // if (flurlException.InnerException?.Message == "A task was canceled.")
            // {
            // return true;
            // }
            if (!(exceptionToken == token))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Posts binary data to the flurl client.
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