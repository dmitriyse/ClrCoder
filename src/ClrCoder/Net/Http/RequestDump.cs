// <copyright file="RequestDump.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Net.Http
{
    using System.Collections.Generic;

    /// <summary>
    /// The http request dump.
    /// </summary>
    public class RequestDump
    {
        /// <summary>
        /// The http request method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The http request body.
        /// </summary>
        public byte[] RequestBody { get; set; }

        /// <summary>
        /// The http protocol version.
        /// </summary>
        public string HttpProtocolVersion { get; set; }

        /// <summary>
        /// The request url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The request headers.
        /// </summary>
        public Dictionary<string, string[]> Headers { get; set; }
    }
}