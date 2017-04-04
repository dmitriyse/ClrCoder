// <copyright file="JLogEntry.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Newtonsoft.Json;

    //// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global

    /// <summary>
    /// Base class for log entries with additional data.
    /// </summary>
    [PublicAPI]
    public class JLogEntry
    {
        /// <summary>
        /// Extension data, dictionary contains non specified properties in a derived type.
        /// </summary>
        [JsonExtensionData]
        [CanBeNull]
        public virtual IDictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Sets extension data property.
        /// </summary>
        /// <param name="key">Extension data property name.</param>
        /// <param name="value">Extension data value.</param>
        public virtual void SetExtensionData([NotNull] string key, [CanBeNull] object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (ExtensionData == null)
            {
                ExtensionData = new Dictionary<string, object>();
            }

            ExtensionData[key] = value;
        }
    }
}