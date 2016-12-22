// <copyright file="HttpMessageEventArgs.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Net.Http
{
    using System;
    using System.Net.Http;

    using JetBrains.Annotations;

    /// <summary>
    /// Request log entry.
    /// </summary>
    [PublicAPI]
    public class HttpMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageEventArgs"/> class.
        /// </summary>
        /// <param name="timeStamp">Request start time in UTC.</param>
        /// <param name="duration">Request duration.</param>
        /// <param name="requestBody">Request content.</param>
        /// <param name="responseBody">Response body.</param>
        /// <param name="requestMessage">Http request message.</param>
        /// <param name="responseMessage">Http response message.</param>
        /// <param name="exception">Request error exception. </param>
        public HttpMessageEventArgs(
            DateTime timeStamp,
            TimeSpan duration,
            string requestBody,
            string responseBody,
            HttpRequestMessage requestMessage,
            HttpResponseMessage responseMessage,
            Exception exception)
        {
            this.TimeStamp = timeStamp;
            this.Duration = duration;
            this.RequestBody = requestBody;
            this.ResponseBody = responseBody;
            this.RequestMessage = requestMessage;
            this.ResponseMessage = responseMessage;
            this.Exception = exception;
        }

        /// <summary>
        /// Request duration.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Request error exception.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Request content.
        /// </summary>
        public string RequestBody { get; private set; }

        /// <summary>
        /// Http request message.
        /// </summary>
        public HttpRequestMessage RequestMessage { get; private set; }

        /// <summary>
        /// Response body.
        /// </summary>
        public string ResponseBody { get; private set; }

        /// <summary>
        /// Http response message.
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; set; }

        /// <summary>
        /// Request start time in UTC.
        /// </summary>
        public DateTime TimeStamp { get; private set; }
    }
}