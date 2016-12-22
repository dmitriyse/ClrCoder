// <copyright file="TextExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Text
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Text related extension methods.
    /// </summary>
    public static class TextExtensions
    {
        private static readonly Regex LineEndingPattern = new Regex("(\r\n)|\r|\n", RegexOptions.CultureInvariant);

        /// <summary>
        /// Normalizes line endings inside string.
        /// </summary>
        /// <remarks>
        /// Current implementation is not so fast ~ 8.0 Mb/s on a fast machine.
        /// </remarks>
        /// <param name="text">Text to normalize line endings.</param>
        /// <returns>Text with all line ending transformed to <see cref="Environment.NewLine"/>.</returns>
        public static string NormalizeLineEndings(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return LineEndingPattern.Replace(text, Environment.NewLine);
        }

        /// <summary>
        /// Iterates through lines from the <paramref name="reader"/>.
        /// </summary>
        /// <remarks>
        /// This method "eats" last empty line.
        /// </remarks>
        /// <param name="reader">Text reader.</param>
        /// <returns>Lines enumeration.</returns>
        /// <example>
        /// Text "a\n" will return only one line "a".
        /// </example>
        public static IEnumerable<string> ReadLines(this TextReader reader)
        {
            for (;;)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    yield return line;
                }
                else
                {
                    break;
                }
            }
        }
    }
}