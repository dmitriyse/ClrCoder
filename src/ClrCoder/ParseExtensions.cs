// <copyright file="ParseExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder
{
    using System;
    using System.Globalization;

    using JetBrains.Annotations;

    /// <summary>
    /// Parsing related extensions.
    /// </summary>
    [PublicAPI]
    public static class ParseExtensions
    {
        /// <summary>
        /// Parses <see langword="decimal"/> value encoded in any culture.
        /// </summary>
        /// <remarks>
        /// Full information about decimal mark can be found here http://en.wikipedia.org/wiki/Decimal_mark.
        /// </remarks>
        /// <param name="decimalString">String with decimal value.</param>
        /// <returns>Parsed value.</returns>
        public static decimal ParseAnyDecimal(this string decimalString)
        {
            if (decimalString == null)
            {
                throw new ArgumentNullException(nameof(decimalString));
            }

            return decimal.Parse(decimalString.Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses <see langword="double"/> value encoded in any culture.
        /// </summary>
        /// <remarks>
        /// Full information about decimal mark can be found here http://en.wikipedia.org/wiki/Decimal_mark.
        /// </remarks>
        /// <param name="doubleString">String with double value.</param>
        /// <returns>Parsed value.</returns>
        public static double ParseAnyDouble(this string doubleString)
        {
            if (doubleString == null)
            {
                throw new ArgumentNullException(nameof(doubleString));
            }

            return double.Parse(doubleString.Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Tries to parse <see langword="decimal"/> value from the specified string.
        /// </summary>
        /// <param name="decimalString">String to parse value from.</param>
        /// <param name="parsedValue">Parsed value or default decimal value.</param>
        /// <returns>true, if value was successfully parsed, false otherwise.</returns>
        public static bool TryParseAnyDecimal(this string decimalString, out decimal parsedValue)
        {
            if (decimalString == null)
            {
                throw new ArgumentNullException(nameof(decimalString));
            }

            return decimal.TryParse(
                decimalString.Replace(',', '.'),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out parsedValue);
        }

        /// <summary>
        /// Tries to parse <see langword="double"/> value from the specified string.
        /// </summary>
        /// <param name="doubleString">String to parse value from.</param>
        /// <param name="parsedValue">Parsed value or default double value.</param>
        /// <returns>true, if value was successfully parsed, false otherwise.</returns>
        public static bool TryParseAnyDouble(this string doubleString, out double parsedValue)
        {
            if (doubleString == null)
            {
                throw new ArgumentNullException(nameof(doubleString));
            }

            return double.TryParse(
                doubleString.Replace(',', '.'),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out parsedValue);
        }
    }
}