// <copyright file="BclProductivityExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

#if NET46
    using System.Threading;    
#endif

    /// <summary>
    /// Extensions for core BCL classes designed to boost development productivity.
    /// </summary>
    [PublicAPI]
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
        /// Gets value from <c>dictionary</c> and creates value if the specified <c>key</c> is not found.
        /// </summary>
        /// <typeparam name="TKey"><c>Type</c> of the <c>dictionary</c> <c>key</c>.</typeparam>
        /// <typeparam name="TValue"><c>Type</c> of the <c>dictionary</c> value</typeparam>
        /// <param name="dictionary"><c>Dictionary</c> to get value from.</param>
        /// <param name="key">Key to search value.</param>
        /// <param name="createFunc">Value factory.</param>
        /// <returns>Value for the specified key or default value.</returns>
        public static TValue GetOrCreate<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            TKey key,
            [NotNull] Func<TKey, TValue> createFunc)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            if (createFunc == null)
            {
                throw new ArgumentNullException(nameof(createFunc));
            }

            TValue result;

            if (dictionary.TryGetValue(key, out result))
            {
                return result;
            }

            TValue newValue = createFunc(key);
            dictionary.Add(key, newValue);

            return newValue;
        }

        /// <summary>
        /// Gets value from <c>dictionary</c> and creates value if the specified <c>key</c> is not found.
        /// </summary>
        /// <typeparam name="TKey"><c>Type</c> of the <c>dictionary</c> <c>key</c>.</typeparam>
        /// <typeparam name="TValue"><c>Type</c> of the <c>dictionary</c> value</typeparam>
        /// <param name="dictionary"><c>Dictionary</c> to get value from.</param>
        /// <param name="key">Key to search value.</param>
        /// <param name="createFunc">Value factory.</param>
        /// <returns>Value for the specified key or default value.</returns>
        public static TValue GetOrCreate<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            TKey key,
            [NotNull] Func<TValue> createFunc)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            if (createFunc == null)
            {
                throw new ArgumentNullException(nameof(createFunc));
            }

            TValue result;

            if (dictionary.TryGetValue(key, out result))
            {
                return result;
            }

            TValue newValue = createFunc();
            dictionary.Add(key, newValue);

            return newValue;
        }

        /// <summary>
        /// Gets value from <c>dictionary</c> and returns default value if the specified <c>key</c> is not found.
        /// </summary>
        /// <typeparam name="TKey"><c>Type</c> of the <c>dictionary</c> <c>key</c>.</typeparam>
        /// <typeparam name="TValue"><c>Type</c> of the <c>dictionary</c> value</typeparam>
        /// <param name="dictionary"><c>Dictionary</c> to get value from.</param>
        /// <param name="key">Key to search value.</param>
        /// <returns>Value for the specified key or default value.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            TValue result;

            if (dictionary.TryGetValue(key, out result))
            {
                return result;
            }

            return default(TValue);
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
        /// Verifies that <c>sequence</c> is empty or even <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">Sequence element type.</typeparam>
        /// <param name="sequence">Sequence to test.</param>
        /// <returns>true, if sequence is null or empty.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence)
        {
            return sequence == null || !sequence.Any();
        }

        /// <summary>
        /// Checks that exception is non critical and can be muted/handled.
        /// </summary>
        /// <param name="ex">Exception to check.</param>
        /// <remarks>
        /// Next exceptions cannot be processed:
        /// <see cref="OutOfMemoryException"/>,
#if NET46
/// <see cref="StackOverflowException"/>,
/// <see cref="ThreadAbortException"/>.
#endif
        /// Also we add <see cref="NotImplementedException"/> to this list in DEBUG mode.
        /// </remarks>
        /// <returns><see langword="true"/>, if exception can be muted/handled.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsProcessable([NotNull] this Exception ex)
        {
#if DEBUG
#if NET46
            return
                !(ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException
                  || ex is NotImplementedException);
#else
            return
                !(ex is OutOfMemoryException || ex is NotImplementedException);
#endif
#else
#if NET46
            return !(ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException);
#else
            return !(ex is OutOfMemoryException);
#endif
#endif
        }

        /// <summary>
        /// Generates random <see langword="double"/> value from the specified range.
        /// </summary>
        /// <param name="rnd"><see cref="Random"/> <c>object</c>.</param>
        /// <param name="maxValueExclusive">Range max boundary (exclusive).</param>
        /// <returns>Random double value grater than zero and less than <see cref="maxValueExclusive"/>.</returns>
        public static double NextDouble(this Random rnd, double maxValueExclusive)
        {
            return NextDouble(rnd, 0.0, maxValueExclusive);
        }

        /// <summary>
        /// Generates random <see langword="double"/> value from the specified range.
        /// </summary>
        /// <param name="rnd"><see cref="Random"/> <c>object</c>.</param>
        /// <param name="minValue">Range min bounday.</param>
        /// <param name="maxValueExclusive">Range max boundary (exclusive).</param>
        /// <returns>Random double value grater <see cref="minValue"/> and less than <see cref="maxValueExclusive"/>.</returns>
        public static double NextDouble(this Random rnd, double minValue, double maxValueExclusive)
        {
            if (minValue > maxValueExclusive)
            {
                throw new ArgumentException("MinValue should be less or equal to maxValueExclusive", nameof(minValue));
            }

            return minValue + (rnd.NextDouble() * (maxValueExclusive - minValue));
        }

        /// <summary>
        /// Generates list with random unique integer values.
        /// </summary>
        /// <param name="rnd">Random values generator.</param>
        /// <param name="minValue">Minimal value.</param>
        /// <param name="maxValueExclusive">Maximal value exclusive.</param>
        /// <param name="size">Result <c>size</c>.</param>
        /// <returns>List with specified <c>size</c> that contains unique random values from the specified range.</returns>
        public static List<int> RandomUniqueSet(this Random rnd, int minValue, int maxValueExclusive, int size)
        {
            if (rnd == null)
            {
                throw new ArgumentNullException();
            }

            if (minValue > maxValueExclusive)
            {
                throw new ArgumentException("Minimal value should be less or equal to maximal value.");
            }

            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Size should be grater or equal to zero");
            }

            if (size > maxValueExclusive - minValue)
            {
                throw new AggregateException("Size should be less or equal to range size between min and max values.");
            }

            int maxValueFromZero = maxValueExclusive - minValue;

            // Fast scenario.
            if (size < maxValueFromZero / 3)
            {
                var uniqueSubset = new HashSet<int>();
                while (uniqueSubset.Count < size)
                {
                    int a = rnd.Next(minValue, maxValueExclusive);
                    uniqueSubset.Add(a);
                }

                return uniqueSubset.ToList();
            }

            var resultSubset = new List<int>(maxValueFromZero);

            for (int i = minValue; i < maxValueExclusive; i++)
            {
                resultSubset.Add(i);
            }

            for (int validElementsCount = maxValueFromZero; validElementsCount > size; validElementsCount--)
            {
                int indexToRemove = rnd.Next(0, validElementsCount);
                resultSubset[indexToRemove] = resultSubset[validElementsCount - 1];
            }

            resultSubset.RemoveRange(size, maxValueFromZero - size);
            return resultSubset;
        }

        /// <summary>
        /// Replaces the <paramref name="source"/> value with the <paramref name="substitute"/>, if it is equals to the
        /// <paramref name="comparand"/>.
        /// </summary>
        /// <typeparam name="T">Type of source value.</typeparam>
        /// <param name="source">Source value.</param>
        /// <param name="comparand">Value to replace.</param>
        /// <param name="substitute">Replacement value.</param>
        /// <returns>
        /// <paramref name="substitute"/> when <paramref name="source"/> is equals to the
        /// <paramref name="comparand"/>, or <paramref name="source"/> otherwise.
        /// </returns>
        public static T Replace<T>(this T source, T comparand, T substitute) where T : IEquatable<T>
        {
            return source.Equals(comparand) ? substitute : source;
        }

        /// <summary>
        /// Rethrows critical exceptions that usually should not be processed.
        /// </summary>
        /// <param name="ex">Exception to check.</param>
        /// <remarks>
        /// Next exceptions cannot be processed:
        /// <see cref="OutOfMemoryException"/>,
#if NET46
/// <see cref="StackOverflowException"/>,
/// <see cref="ThreadAbortException"/>.
#endif
        /// Also we add <see cref="NotImplementedException"/> to this list in DEBUG mode.
        /// </remarks>
        public static void RethrowUnprocessable([NotNull] this Exception ex)
        {
#if DEBUG
#if NET46
            if (ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException
                || ex is NotImplementedException)
            {
                throw ex;
            }
#else
            if (ex is OutOfMemoryException || ex is NotImplementedException)
            {
                throw ex;
            }
#endif
#else
#if NET46
            if (ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException)
            {
                throw ex;
            }
#else
            if (ex is OutOfMemoryException)
            {
                throw ex;
            }
#endif
#endif
        }

        /// <summary>
        /// Calls <c>action</c> if it is not <c>null</c>.
        /// </summary>
        /// <param name="action"><c>Action</c> to call.</param>
        public static void SafeInvoke(this Action action)
        {
            action?.Invoke();
        }

        /// <summary>
        /// Calls <c>action</c> if it is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T1"><c>Type</c> of the argument 1.</typeparam>
        /// <param name="action"><c>Action</c> to call.</param>
        /// <param name="arg1"><c>Action</c> argument 1.</param>
        public static void SafeInvoke<T1>(this Action<T1> action, T1 arg1)
        {
            action?.Invoke(arg1);
        }

        /// <summary>
        /// Calls <c>action</c> if it is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T1"><c>Type</c> of the argument 1.</typeparam>
        /// <typeparam name="T2"><c>Type</c> of the argument 2.</typeparam>
        /// <param name="action"><c>Action</c> to call.</param>
        /// <param name="arg1"><c>Action</c> argument 1.</param>
        /// <param name="arg2"><c>Action</c> argument 2.</param>
        public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            action?.Invoke(arg1, arg2);
        }

        /// <summary>
        /// Calls <c>action</c> if it is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T1"><c>Type</c> of the argument 1.</typeparam>
        /// <typeparam name="T2"><c>Type</c> of the argument 2.</typeparam>
        /// <typeparam name="T3"><c>Type</c> of the argument 3.</typeparam>
        /// <param name="action"><c>Action</c> to call.</param>
        /// <param name="arg1"><c>Action</c> argument 1.</param>
        /// <param name="arg2"><c>Action</c> argument 2.</param>
        /// <param name="arg3"><c>Action</c> argument 3.</param>
        public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            action?.Invoke(arg1, arg2, arg3);
        }

        /// <summary>
        /// Calls <c>action</c> if it is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T1"><c>Type</c> of the argument 1.</typeparam>
        /// <typeparam name="T2"><c>Type</c> of the argument 2.</typeparam>
        /// <typeparam name="T3"><c>Type</c> of the argument 3.</typeparam>
        /// <typeparam name="T4"><c>Type</c> of the argument 4.</typeparam>
        /// <param name="action"><c>Action</c> to call.</param>
        /// <param name="arg1"><c>Action</c> argument 1.</param>
        /// <param name="arg2"><c>Action</c> argument 2.</param>
        /// <param name="arg3"><c>Action</c> argument 3.</param>
        /// <param name="arg4"><c>Action</c> argument 4.</param>
        public static void SafeInvoke<T1, T2, T3, T4>(
            this Action<T1, T2, T3, T4> action,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4)
        {
            action?.Invoke(arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Calls <c>action</c> if it is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T1"><c>Type</c> of the argument 1.</typeparam>
        /// <typeparam name="T2"><c>Type</c> of the argument 2.</typeparam>
        /// <typeparam name="T3"><c>Type</c> of the argument 3.</typeparam>
        /// <typeparam name="T4"><c>Type</c> of the argument 4.</typeparam>
        /// <typeparam name="T5"><c>Type</c> of the argument 5.</typeparam>
        /// <param name="action"><c>Action</c> to call.</param>
        /// <param name="arg1"><c>Action</c> argument 1.</param>
        /// <param name="arg2"><c>Action</c> argument 2.</param>
        /// <param name="arg3"><c>Action</c> argument 3.</param>
        /// <param name="arg4"><c>Action</c> argument 4.</param>
        /// <param name="arg5"><c>Action</c> argument 5.</param>
        public static void SafeInvoke<T1, T2, T3, T4, T5>(
            this Action<T1, T2, T3, T4, T5> action,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Calls event handler if it is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Type of event args.</typeparam>
        /// <param name="eventHandler">Event handler <see langword="delegate"/>.</param>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="eventArgs">Event arguments.</param>
        public static void SafeInvoke<T>(this EventHandler<T> eventHandler, object sender, T eventArgs)
        {
            eventHandler?.Invoke(sender, eventArgs);
        }

        /// <summary>
        /// Converts string to <see langword="bool"/>.
        /// </summary>
        /// <param name="str">String to convert to <see langword="bool"/>.</param>
        /// <returns>Parsed bool value or null if string is null or invalid decimal.</returns>
        public static bool? ToBool(this string str)
        {
            bool result;
            return str != null && bool.TryParse(str, out result) ? (bool?)result : null;
        }

        /// <summary>
        /// Converts string to <see langword="decimal"/>.
        /// </summary>
        /// <param name="str">String to convert to <see langword="decimal"/>.</param>
        /// <returns>Parsed decimal value or null if string is null or invalid decimal.</returns>
        public static decimal? ToDecimal(this string str)
        {
            decimal result;
            return str != null
                   && decimal.TryParse(
                       str.Replace(",", "."),
                       NumberStyles.Any,
                       CultureInfo.InvariantCulture,
                       out result)
                       ? (decimal?)result
                       : null;
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
        /// Converts timespan to <see langword="double"/> seconds value;
        /// </summary>
        /// <param name="timeSpan">Time span to convert.</param>
        /// <returns>Time span in seconds.</returns>
        public static double ToSeconds(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks / (double)TimeSpan.TicksPerSecond;
        }
    }
}