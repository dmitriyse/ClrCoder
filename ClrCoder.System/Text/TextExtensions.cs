using System;
using System.Text.RegularExpressions;

namespace ClrCoder.System.Text
{
    /// <summary>
    /// Text related extension methods.
    /// </summary>
    public static class TextExtensions
    {
        private static readonly Regex LineEndingPattern = new Regex(
            "(\r\n)|\r|\n",
            RegexOptions.CultureInvariant);

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
                throw new ArgumentNullException("text");
            }

            return LineEndingPattern.Replace(text, Environment.NewLine);
        }
    }
}