// <copyright file="UPathParseMode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    /// <summary>
    /// UPath parse mode.
    /// </summary>
    public enum UPathParseMode
    {
        /// <summary>
        /// Allows only UPath format.
        /// </summary>
        Strict,

        /// <summary>
        /// Allows windows and unix slashes.
        /// </summary>
        AllowAnyPlatform,

        /// <summary>
        /// Allows incorrectly formatted paths (with extra spaces, double slashes, etc.).
        /// </summary>
        AllowIncorrectFormat
    }
}