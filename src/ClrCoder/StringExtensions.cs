// <copyright file="StringExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
    using System.Text;

    using JetBrains.Annotations;

    /// <summary>
    /// Utility methods for working with strings.
    /// </summary>
    [PublicAPI]
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces first charter to lower case.
        /// </summary>
        /// <param name="str">Original string.</param>
        /// <returns>String with first char lowered.</returns>
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            var strBuilder = new StringBuilder(str);
            strBuilder[0] = char.ToLowerInvariant(strBuilder[0]);
            return strBuilder.ToString();
        }

        //// TODO: Write exceptions description.

        /// <summary>
        /// Limits string length.
        /// </summary>
        /// <param name="str"><c>String</c> to truncate.</param>
        /// <param name="maxLength">Maximal allowed length.</param>
        /// <returns>Truncated string.</returns>
        public static string Truncate(this string str, int maxLength)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (maxLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength), "Maximal length should be positive.");
            }

            if (str.Length > maxLength)
            {
                return str.Substring(0, maxLength);
            }

            return str;
        }
    }
}