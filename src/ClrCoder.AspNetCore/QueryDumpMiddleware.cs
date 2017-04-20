// <copyright file="QueryDumpMiddleware.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using IO;

    using JetBrains.Annotations;

    using Logging;
    using Logging.Std;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Http.Features;

    using Net.Http;

    /// <summary>
    /// Midleware for add information about request.
    /// </summary>
    public class QueryDumpMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDumpMiddleware"/> class.
        /// </summary>
        /// <param name="next"> The RequestDelegate. </param>
        /// <param name="logger"> The logger for support team. </param>
        [UsedImplicitly]
        public QueryDumpMiddleware(
            RequestDelegate next,
            IJsonLogger logger)
        {
            _next = next;
            Log = new ClassJsonLogger<QueryDumpMiddleware>(logger);
        }

        private IJsonLogger Log { get; }

        /// <summary>
        /// Handles request.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <returns>Async execution TPL task.</returns>
        [UsedImplicitly]
        public async Task Invoke([NotNull] HttpContext context)
        {
            var requestDump = new RequestDump();
            var isBodyDumpingStarted = false;
            try
            {
                requestDump.Url = context.Request.GetDisplayUrl();
                requestDump.Headers =
                    context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToArray());
                requestDump.Method = context.Request.Method;
                requestDump.HttpProtocolVersion = context.Request.Protocol;

                if (context.Request.Body != null)
                {
                    Log.Debug(
                        _ => _($"Reading request body started: {requestDump.Method} {requestDump.Url}")
                            .Data(
                                new
                                    {
                                        RequestID = context.TraceIdentifier
                                    }));

                    isBodyDumpingStarted = true;
                    requestDump.RequestBody = await context.Request.Body.ReadAllBytesAsync();
                    context.Request.Body = new MemoryStream(requestDump.RequestBody);
                }
            }
            catch (Exception ex)
            {
                if (isBodyDumpingStarted)
                {
                    Log.Error(
                        _ => _($"Error dumping request body: {requestDump.Method} {requestDump.Url}")
                            .Data(
                                new
                                    {
                                        RequestDump = requestDump,
                                        RequestID = context.TraceIdentifier
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
                                        RequestDump = requestDump,
                                        RequestID = context.TraceIdentifier
                                    })
                            .Exception(ex));
                }

                throw;
            }

            Log.Trace(
                _ => _($"Request handling started: {requestDump.Method} {requestDump.Url}")
                    .Data(
                        new
                            {
                                Dump = requestDump,
                                RequestId = context.TraceIdentifier
                            }));

            Stream originalResponseStream = context.Response.Body;
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            Exception handlingError = null;
            var responseDump = new ResponseDump();
            try
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex) when (ex.IsProcessable())
                {
                    handlingError = ex;
                    throw;
                }
                finally
                {
                    try
                    {
                        context.Response.Body = originalResponseStream;
                        responseStream.Seek(0, SeekOrigin.Begin);
                        await responseStream.CopyToAsync(originalResponseStream);
                    }
                    catch (Exception ex) when (ex.IsProcessable())
                    {
                        if (handlingError == null)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        try
                        {
                            responseDump.StatusCode = context.Response?.StatusCode.ToString();
                            responseDump.ReasonPhrase = context.Response.HttpContext.Features
                                ?.Get<IHttpResponseFeature>()
                                ?.ReasonPhrase;
                            responseDump.Headers =
                                context.Response.Headers.ToDictionary(x => x.Key, x => x.Value.ToArray());
                            if (!responseDump.Headers.ContainsKey("Server"))
                            {
                                responseDump.Headers.Add("Server", new[] { "Kestrel" });
                            }

                            responseDump.ResponseBody = responseStream.ToArray();
                        }
                        catch (Exception ex) when (ex.IsProcessable())
                        {
                            Log.Critical(
                                _ => _(
                                        $"Internal dumping error of the request: {requestDump.Method} {requestDump.Url} -> {responseDump.StatusCode} {responseDump.ReasonPhrase}")
                                    .Data(
                                        new
                                            {
                                                ResponseId = context.TraceIdentifier
                                            })
                                    .Exception(ex));
                        }
                    }
                }

                Log.Trace(
                    _ => _(
                            $"Response sent: {requestDump.Method} {requestDump.Url} -> {responseDump.StatusCode} {responseDump.ReasonPhrase}")
                        .Data(
                            new
                                {
                                    Dump = responseDump,
                                    RequestId = context.TraceIdentifier
                                }));
            }
            catch (Exception ex) when (ex.IsProcessable())
            {
                string errorString = handlingError != null
                                         ? "Exception while handling request"
                                         : "Error writing dumped response to the output stream";

                Log.Error(
                    _ => _(
                            $"{errorString}: {requestDump.Method} {requestDump.Url} -> {responseDump.StatusCode} {responseDump.ReasonPhrase}")
                        .Data(
                            new
                                {
                                    ResponseDump = responseDump,
                                    ResponseId = context.TraceIdentifier
                                })
                        .Exception(ex));
                throw;
            }
        }
    }
}