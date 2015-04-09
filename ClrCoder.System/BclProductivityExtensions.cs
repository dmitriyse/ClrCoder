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
                throw new ArgumentNullException("rnd");
            }

            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            return collection[rnd.Next(collection.Count)];
        }
    }
}