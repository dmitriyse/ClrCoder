// <copyright file="RegexFluentExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Text
{
    using System.Text.RegularExpressions;

    using JetBrains.Annotations;

    /// <summary>
    /// Fluent wrapper for regex.
    /// </summary>
    [PublicAPI]
    public static class RegexFluentExtensions
    {
        /// <summary>
        /// <see cref="Regex.Replace(string,string)"/> method wrapper.
        /// </summary>
        /// <param name="str">Source string.</param>
        /// <param name="pattern">Search pattern.</param>
        /// <param name="replacement">Replacement pattern.</param>
        /// <returns>Result string after replacements.</returns>
        public static string ReplaceEx(this string str, [RegexPattern] string pattern, string replacement)
        {
            return Regex.Replace(str, pattern, replacement);
        }
    }
}