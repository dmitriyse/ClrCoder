// <copyright file="TracingDelegatingHandler.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Net.Http
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    //// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global

    /// <summary>
    /// Message handler, with request logging.
    /// </summary>
    [PublicAPI]
    public class TracingDelegatingHandler : DelegatingHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TracingDelegatingHandler"/> class.
        /// </summary>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        public TracingDelegatingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        /// <summary>
        /// Raises when message is processed.
        /// </summary>
        public event EventHandler<HttpMessageEventArgs> MessageProcessed;

        /// <summary>
        /// Raises <see cref="MessageProcessed"/> event.
        /// </summary>
        /// <param name="e">Event argument.</param>
        protected virtual void OnMessageProcessed(HttpMessageEventArgs e)
        {
            this.MessageProcessed?.Invoke(this, e);
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            DateTime startTime = DateTime.UtcNow;

            // Allows to load
            string requestBody = null;

            if (request.Content != null)
            {
                try
                {
                    await request.Content.LoadIntoBufferAsync();
                    requestBody = await request.Content.ReadAsStringAsync();
                }
                catch
                {
                    // Mute error.
                }
            }

            try
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                string responseBody = null;
                try
                {
                    await response.Content.LoadIntoBufferAsync();
                    responseBody = await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    // Mute error.
                }

                TimeSpan duration = DateTime.UtcNow - startTime;
                try
                {
                    this.OnMessageProcessed(
                        new HttpMessageEventArgs(
                            startTime,
                            duration,
                            requestBody,
                            responseBody,
                            request,
                            response,
                            null));
                }
                catch
                {
                    // Mute error.
                }

                return response;
            }
            catch (Exception ex)
            {
                TimeSpan duration = DateTime.UtcNow - startTime;
                try
                {
                    this.OnMessageProcessed(
                        new HttpMessageEventArgs(startTime, duration, requestBody, null, request, null, ex));
                }
                catch
                {
                    // Mute error.
                }

                throw;
            }
        }
    }
}