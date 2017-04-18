// <copyright file="ResponseDump.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Net.Http
{
    using System.Collections.Generic;

    /// <summary>
    /// The http response dump.
    /// </summary>
    public class ResponseDump
    {
        /// <summary>
        /// The http response status code.
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// THe http response body.
        /// </summary>
        public byte[] ResponseBody { get; set; }

        /// <summary>
        /// The http response reason phrase.
        /// </summary>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// The http response headers.
        /// </summary>
        public Dictionary<string, string[]> Headers { get; set; }
    }
}