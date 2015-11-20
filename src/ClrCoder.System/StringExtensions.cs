using System;

namespace ClrCoder.System
{
    /// <summary>
    /// Utility methods for working with strings.
    /// </summary>
    public static class StringExtensions
    {
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
                throw new ArgumentNullException("str");
            }

            if (maxLength < 0)
            {
                throw new ArgumentOutOfRangeException("maxLength", "Maximal length should be positive.");
            }
            
            if (str.Length > maxLength)
            {
                return str.Substring(0, maxLength);
            }
            
            return str;
        }
    }
}