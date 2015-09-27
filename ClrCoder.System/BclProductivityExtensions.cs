using System;
using System.Collections.Generic;

namespace ClrCoder.System
{
    /// <summary>
    /// Extensions for core BCL classes designed to boost development productivity.
    /// </summary>
    public static class BclProductivityExtensions
    {
        /// <summary>
        /// <see cref="string.Format(string, object[])"/> alias.
        /// </summary>
        /// <param name="formatString">Format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <returns>Formatted string.</returns>
        public static string Fmt(this string formatString, params object[] args)
        {
            return string.Format(formatString, args);
        }

        /// <summary>
        /// Gets random item from the <c>collection</c>.
        /// </summary>
        /// <remarks>
        /// This estension method is not thread safe.
        /// </remarks>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="rnd"><see cref="Random"/> <c>object</c>.</param>
        /// <param name="collection">Collection to get item from.</param>
        /// <returns>Random collection item.</returns>
        public static T From<T>(this Random rnd, IReadOnlyList<T> collection)
        {
            if (rnd == null)
            {
                throw new ArgumentNullException(nameof(rnd));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return collection[rnd.Next(collection.Count)];
        }

        /// <summary>
        /// Gets exception short description in form ExceptionType: Message.
        /// </summary>
        /// <param name="ex"><c>Exception</c> to get short description for.</param>
        /// <returns>Short description string.</returns>
        public static string GetShortDescription(this Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            return $"{ex.GetType().Name}: {ex.Message}";
        }

        /// <summary>
        /// Replaces the <paramref name="source"/> value with the <paramref name="substitute"/>, if it is equals to the 
        /// <paramref name="comparand"/>.
        /// </summary>
        /// <typeparam name="T">Type of source value.</typeparam>
        /// <param name="source">Source value.</param>
        /// <param name="comparand">Value to replace.</param>
        /// <param name="substitute">Replacement value.</param>
        /// <returns><paramref name="substitute"/> when <paramref name="source"/> is equals to the 
        /// <paramref name="comparand"/>, or <paramref name="source"/> otherwise.</returns>
        public static T Replace<T>(this T source, T comparand, T substitute)
            where T : IEquatable<T>
        {
            return source.Equals(comparand) ? substitute : source;
        }

        /// <summary>
        /// Converts string to <see langword="int"/>.
        /// </summary>
        /// <param name="str">String to convert to <see langword="int"/>.</param>
        /// <returns>Parsed integer value or null if string is null or invalid integer.</returns>
        public static int? ToInt(this string str)
        {
            int result;
            return str != null && int.TryParse(str, out result) ? (int?)result : null;
        }

        /// <summary>
        /// Converts string to <see langword="decimal"/>.
        /// </summary>
        /// <param name="str">String to convert to <see langword="decimal"/>.</param>
        /// <returns>Parsed integer value or null if string is null or invalid decimal.</returns>
        public static decimal? ToDecimal(this string str)
        {
            decimal result;
            return str != null && decimal.TryParse(str, out result) ? (decimal?)result : null;
        }
    }
}