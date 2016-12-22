// <copyright file="EncodingEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder
{
    using System.Text;

    using JetBrains.Annotations;

    /// <summary>
    /// Additional default encodings.
    /// </summary>
    public static class EncodingEx
    {
        /// <summary>
        /// UTF16 Encoding without BOM.
        /// </summary>
        [PublicAPI]
        public static Encoding UnicodeNoBom = new UnicodeEncoding(false, false);

        //// ReSharper disable once InconsistentNaming

        /// <summary>
        /// UTF8 Encoding without BOM.
        /// </summary>
        [PublicAPI]
        public static Encoding UTF8NoBom = new UTF8Encoding(false);
    }
}