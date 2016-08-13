// <copyright file="JLogEntry.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Logging
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    //// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global

    /// <summary>
    /// Base class for log entries with additional data.
    /// </summary>
    [PublicAPI]
    public class JLogEntry
    {
        /// <summary>
        /// Extension data contains nen specified in class properties.
        /// </summary>
        [JsonExtensionData]
        public virtual Dictionary<string, JToken> ExtensionData { get; set; }
    }
}