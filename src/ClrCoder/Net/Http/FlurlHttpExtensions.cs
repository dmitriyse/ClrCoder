// <copyright file="FlurlHttpExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Net.Http
{
#if NETSTANDARD2_0
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    using Flurl;
    using Flurl.Http;
    using Flurl.Http.Configuration;

    using JetBrains.Annotations;

    using Logging;
    using Logging.Std;

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
            if ((flurlException.Call.Response == null)
                && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
                && (flurlException.InnerException?.InnerException?.Message ==
                    "A security error occurred"))
            {
                return true;
            }

            // net 4.6
            if ((flurlException.Call.Response == null)
                && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
                && (flurlException.InnerException?.InnerException?.Message?.Contains("A security error occurred")
                    ?? false)
                && (flurlException.InnerException?.InnerException?.InnerException?.Message?.Contains("handshake failed")
                    ?? false))
            {
                return true;
            }

            // Linux
            if ((flurlException.Call.Response == null)
                && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
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
            if ((flurlException.Call.Response == null)
                && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
                && (flurlException.InnerException.InnerException.Message != null)
                && flurlException.InnerException.InnerException.Message.Contains(
                    "remote name could not be resolved"))
            {
                return true;
            }

            // core
            if ((flurlException.Call.Response == null)
                && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
                && (flurlException.InnerException?.InnerException?.Message?.Contains(
                        "server name or address could not be resolved") ?? false))
            {
                return true;
            }

            // Linux
            return (flurlException.Call.Response == null)
                   && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
                   && (flurlException.InnerException?.InnerException?.Message != null)
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
            if ((flurlException.Call.Response == null)
                && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
                && (flurlException.InnerException.InnerException.InnerException != null)
                && (flurlException.InnerException?.InnerException?.Message == "Unable to connect to the remote server")
                && flurlException.InnerException.InnerException.InnerException.Message.StartsWith(
                    "A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond")
            )
            {
                return true;
            }

            // net 4.6 locked IP
            if ((flurlException.Call.Response == null)
                && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
                && (flurlException.InnerException.InnerException.InnerException != null)
                && (flurlException.InnerException?.InnerException?.Message == "Unable to connect to the remote server")
                && flurlException.InnerException.InnerException.InnerException.Message.StartsWith(
                    "No connection could be made because the target machine actively refused it"))
            {
                return true;
            }

            // core does not exist IP
            if ((flurlException.Call.Response == null)
                && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
                && (flurlException.InnerException?.InnerException?.Message ==
                    "The operation timed out"))
            {
                return true;
            }

            // core locked IP
            if ((flurlException.Call.Response == null)
                && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
                && (flurlException.InnerException?.InnerException?.Message ==
                    "A connection with the server could not be established"))
            {
                return true;
            }

            // Linux does not exist IP
            // FlurlHttpException
            // Request to http://10.110.110.110/ failed. A task was canceled.
            // TaskCanceledException
            // A task was canceled.

            // Linux locked IP
            if ((flurlException.Call.Response == null)
                && (flurlException.InnerException?.GetType() == typeof(HttpRequestException))
                && (flurlException.InnerException?.InnerException?.Message ==
                    "Couldn't connect to server"))
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
            this IFlurlRequest client,
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

            return PostBytesAsync(new FlurlRequest(url), content, setHeadersAction, cancellationToken);
        }

        /// <summary>
        /// Enables request/response dumping to the provided logger.
        /// </summary>
        /// <param name="flurlRequest">Flurl fluent syntax.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static IFlurlClient WithDump(this IFlurlClient flurlRequest, IJsonLogger logger)
        {
            return flurlRequest.Configure(
                client => client.HttpClientFactory = new HttpClientWithDumpingFactory(logger));
        }

        /// <summary>
        /// Enables request/response dumping to the provided logger.
        /// </summary>
        /// <param name="url">Flurl url.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static IFlurlRequest WithDump(this Url url, IJsonLogger logger)
        {
            return new FlurlRequest(url).WithClient(
                new FlurlClient().Configure(
                    client => client.HttpClientFactory = new HttpClientWithDumpingFactory(logger)));
        }

        private class DumpHandler : DelegatingHandler
        {
            public DumpHandler(HttpMessageHandler inner, IJsonLogger logger)
                : base(inner)
            {
                Log = new ClassJsonLogger<DumpHandler>(logger);
            }

            private IJsonLogger Log { get; }

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Guid id = Guid.NewGuid();
                var wasContentDumpingStarted = false;
                var requestDump = new RequestDump();
                try
                {
                    requestDump.Method = request.Method?.ToString() ?? "<null>";
                    requestDump.HttpProtocolVersion = request.Version?.ToString() ?? "<null>";
                    requestDump.Url = request.RequestUri?.AbsoluteUri ?? "<null>";
                    requestDump.Headers = request.Headers.ToDictionary(x => x.Key, x => x.Value.ToArray());

                    if (request.Content != null)
                    {
                        wasContentDumpingStarted = true;
                        await request.Content.LoadIntoBufferAsync();
                        requestDump.RequestBody = await request.Content.ReadAsByteArrayAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (wasContentDumpingStarted)
                    {
                        Log.Error(
                            _ => _($"Error dumping request content: {requestDump.Method} {requestDump.Url}")
                                .Data(
                                    new
                                        {
                                            RequestDump = requestDump
                                        })
                                .Exception(ex));
                    }
                    else
                    {
                        Log.Error(
                            _ => _($"Error dumping request: {requestDump.Method} {requestDump.Url}")
                                .Data(
                                    new
                                        {
                                            RequestDump = requestDump
                                        })
                                .Exception(ex));
                    }

                    throw;
                }

                Log.Trace(
                    _ => _($"Client http request started: {requestDump.Method} {requestDump.Url}")
                        .Data(
                            new
                                {
                                    RequestDump = requestDump,
                                    ClientRequestId = id
                                }));

                HttpResponseMessage response;
                try
                {
                    response = await base.SendAsync(request, cancellationToken);
                    Log.Debug(
                        _ => _(
                            $"Response received, starting read: {requestDump.Method} {requestDump.Url} -> {(int)response.StatusCode} {response.ReasonPhrase}"));
                }
                catch (Exception ex)
                {
                    Log.Error(
                        _ => _($"Client http request error. ({requestDump.Method} {requestDump.Url})")
                            .Data(
                                new
                                    {
                                        ClientRequestId = id
                                    })
                            .Exception(ex));
                    throw;
                }

                var responseDump = new ResponseDump();
                wasContentDumpingStarted = false;
                try
                {
                    responseDump.StatusCode = ((int)response.StatusCode).ToString();
                    responseDump.ReasonPhrase = response.ReasonPhrase ?? "<null>";
                    responseDump.Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.ToArray());

                    if (response.Content != null)
                    {
                        wasContentDumpingStarted = true;
                        await response.Content.LoadIntoBufferAsync();
                        responseDump.ResponseBody = await response.Content.ReadAsByteArrayAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (wasContentDumpingStarted)
                    {
                        Log.Error(
                            _ => _(
                                    $"Error dumping response content: {requestDump.Method} {requestDump.Url} -> {response.StatusCode} {response.ReasonPhrase}")
                                .Data(
                                    new
                                        {
                                            ResponseDump = responseDump,
                                            ClientResponseId = id
                                        })
                                .Exception(ex));
                    }
                    else
                    {
                        Log.Error(
                            _ => _(
                                    $"Error dumping response: {requestDump.Method} {requestDump.Url} -> {response.StatusCode} {response.ReasonPhrase}")
                                .Data(
                                    new
                                        {
                                            ResponseDump = responseDump,
                                            ClientResponseId = id
                                        })
                                .Exception(ex));
                    }

                    throw;
                }

                Log.Trace(
                    _ => _(
                            $"Response received: {requestDump.Method} {requestDump.Url} -> {responseDump.StatusCode} {responseDump.ReasonPhrase}")
                        .Data(
                            new
                                {
                                    ResponseDump = responseDump,
                                    ClientResponseId = id
                                }));

                return response;
            }
        }

        /// <summary>
        /// Factory for add request/response dump.
        /// </summary>
        private class HttpClientWithDumpingFactory : IHttpClientFactory
        {
            private readonly IJsonLogger _logger;

            public HttpClientWithDumpingFactory(IJsonLogger logger)
            {
                _logger = logger;
            }

            public HttpClient CreateHttpClient(HttpMessageHandler handler)
            {
                // Default implementation.
                var httpClient = new HttpClient(handler);
                TimeSpan infiniteTimeSpan = Timeout.InfiniteTimeSpan;
                httpClient.Timeout = infiniteTimeSpan;
                return httpClient;
            }

            public HttpMessageHandler CreateMessageHandler()
            {
                return new DumpHandler(new HttpClientHandler(), _logger);
            }
        }
    }

#endif
}