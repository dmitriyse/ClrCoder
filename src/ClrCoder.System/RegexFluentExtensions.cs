using System.Text.RegularExpressions;

namespace ClrCoder.System
{
    /// <summary>
    /// Fluent wrapper for regex.
    /// </summary>
    public static class RegexFluentExtensions
    {
        /// <summary>
        /// <see cref="Regex.Replace(string,string)"/> method wrapper.
        /// </summary>
        /// <param name="str">Source string.</param>
        /// <param name="pattern">Search pattern.</param>
        /// <param name="replacement">Replacement pattern.</param>
        /// <returns>Result string after replacements.</returns>
        public static string ReplaceEx(this string str, string pattern, string replacement)
        {
            return Regex.Replace(str, pattern, replacement);
        }
    }
}