// <copyright file="VxArgs.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Validation
{
    using System;
    using System.Collections.Generic;
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System.Drawing;
#endif
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// Simple argument validation utils.
    /// </summary>
    [PublicAPI]
    public static class VxArgs
    {
        /// <summary>
        /// Validates that argument is positive and finite.
        /// </summary>
        /// <param name="value">Argument <c>value</c>.</param>
        /// <param name="name">Argument <c>name</c>.</param>
        public static void FinitPositive(double value, [InvokerParameterName] string name)
        {
            if (!(value > 0.0) && double.IsPositiveInfinity(value))
            {
                throw new ArgumentOutOfRangeException(name, $"{name} should be finite positive.");
            }
        }

        /// <summary>
        /// Validates that argument is positive and finite.
        /// </summary>
        /// <param name="value">Argument <c>value</c>.</param>
        /// <param name="name">Argument <c>name</c>.</param>
        public static void FinitPositive(double? value, [InvokerParameterName] string name)
        {
            if ((value != null) && !(value > 0.0) && double.IsPositiveInfinity(value.Value))
            {
                throw new ArgumentOutOfRangeException(name, $"{name} should be finite positive.");
            }
        }

        /// <summary>
        /// Validates that <c>value</c> fall in the specified range.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="start">Range <c>start</c> (inclusive).</param>
        /// <param name="end">Range <c>end</c> (inclusive).</param>
        /// <param name="name">Argument <c>name</c>.</param>
        public static void InRange(double value, double start, double end, [InvokerParameterName] string name)
        {
            if (!(value >= start) || !(value <= end))
            {
                throw new ArgumentOutOfRangeException($"{name} should fall in range [{start}, {end}]", name);
            }
        }

        /// <summary>
        /// Validates that value typed argument is initialized (not equals to the default value).
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="name">The argument name.</param>
        public static void NonDefault<T>(T value, [InvokerParameterName] string name)
            where T : struct, IEquatable<T>
        {
            if (value.Equals(default(T)))
            {
                throw new ArgumentException("Argument should not be equal to the default value.", name);
            }
        }

        /// <summary>
        /// Validates that argument is non negative.
        /// </summary>
        /// <param name="value">Argument <c>value</c>.</param>
        /// <param name="name">Argument <c>name</c>.</param>
        public static void NonNegative(int value, [InvokerParameterName] string name)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException($"{name} should not be negative.", name);
            }
        }

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0

        /// <summary>
        /// Validates that Width and Height are both non negative.
        /// </summary>
        /// <param name="size">Size to validate.</param>
        /// <param name="name">Argument <c>name</c>.</param>
        public static void NonNegativeSize(Size? size, [InvokerParameterName] string name)
        {
            if ((size != null) && ((size.Value.Width < 0) || (size.Value.Height < 0)))
            {
                throw new ArgumentOutOfRangeException(name, "Both Width and Height should be non negative");
            }
        }
#endif

        /// <summary>
        /// Validates argument string is not empty.
        /// </summary>
        /// <param name="str">String to validate.</param>
        /// <param name="name">Name of argument.</param>
        public static void NonNullOrWhiteSpace(string str, [InvokerParameterName] string name)
        {
            if (str == null)
            {
                throw new ArgumentNullException(name);
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException($"{name} should not be an empty or whitespace string.", name);
            }
        }

        /// <summary>
        /// Validates that the specified value not equals to the default value.
        /// </summary>
        /// <typeparam name="T">The type of the value to validate.</typeparam>
        /// <param name="value">Value to validate.</param>
        /// <param name="name">Argument <c>name</c>.</param>
        public static void NotDefault<T>(T value, [InvokerParameterName] string name)
            where T : struct
        {
            throw new ArgumentException(nameof(name), $"{name} cannot be equal to the default value.");
        }

        /// <summary>
        /// Validates that <c>collection</c> is not empty. Also validates items not <c>null</c> in debug mode.
        /// </summary>
        /// <typeparam name="T">Type of <c>collection</c> item.</typeparam>
        /// <param name="collection">Collection to validate.</param>
        /// <param name="name">Name of an argument.</param>
        [Pure]
        public static void NotEmpty<T>(ICollectionEx<T> collection, string name)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(name);
            }

            if (collection.Count == 0)
            {
                throw new ArgumentException($"{name} collection should not be empty.", name);
            }

#if DEBUG
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
                foreach (T item in collection)
                {
                    if (item == null)
                    {
                        throw new ArgumentException($"{name} collection element is null.", name);
                    }
                }
            }

#endif
        }

        /// <summary>
        /// Validates that <c>collection</c> is not empty. Also validates items not <c>null</c> in debug mode.
        /// </summary>
        /// <typeparam name="T">Type of <c>collection</c> item.</typeparam>
        /// <param name="collection">Collection to validate.</param>
        /// <param name="name">Name of an argument.</param>
        public static void NotEmptyReadOnly<T>(IReadOnlyCollection<T> collection, string name)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(name);
            }

            if (collection.Count == 0)
            {
                throw new ArgumentException($"{name} collection should not be empty.", name);
            }

#if DEBUG
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
                foreach (T item in collection)
                {
                    if (item == null)
                    {
                        throw new ArgumentException($"{name} collection element is null.", name);
                    }
                }
            }

#endif
        }

        /// <summary>
        /// Validates that passed parameter value is not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to validate.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="errorName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">Argument is null.</exception>
        [ContractAnnotation("value:null=>halt")]
        public static void NotNull<T>([CanBeNull] T value, string errorName)
            where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(errorName);
            }
        }

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0 /// <summary>
/// Validates that Width and Height are both non zero positive.
/// </summary>
/// <param name="size">Size to validate.</param>
/// <param name="name">Argument <c>name</c>.</param>
        public static void PositiveSize(Size size, [InvokerParameterName] string name)
        {
            if ((size.Width <= 0) || (size.Height <= 0))
            {
                throw new ArgumentOutOfRangeException(name, "Both Width and Height should be non-zero positive");
            }
        }

        /// <summary>
        /// Validates that Width and Height are both non zero positive.
        /// </summary>
        /// <param name="size">Size to validate.</param>
        /// <param name="name">Argument <c>name</c>.</param>
        public static void PositiveSize(Size? size, [InvokerParameterName] string name)
        {
            if ((size != null) && ((size.Value.Width <= 0) || (size.Value.Height <= 0)))
            {
                throw new ArgumentOutOfRangeException(name, "Both Width and Height should be non-zero positive");
            }
        }
#endif

        /// <summary>
        /// Validates that the specified value fits into TypeChoice. Validation success for <see langword="null"/> value.
        /// </summary>
        /// <typeparam name="T">The type of the value to validate.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="name">The parameter name.</param>
        /// <returns>Fluent syntax to choice validation.</returns>
        public static VxValidateTypeChoice<T> TypeChoice<T>([CanBeNull] T value, [InvokerParameterName] string name)
        {
            return new VxValidateTypeChoice<T>(value, name);
        }

        /// <summary>
        /// Validates that provided Uri is valid http and absolute.
        /// </summary>
        /// <param name="uri">Uri to validate.</param>
        /// <param name="name">Argument <c>name</c>.</param>
        public static void ValidAbsoluteHttpUri(string uri, [InvokerParameterName] string name)
        {
            NonNullOrWhiteSpace(uri, name);

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                var uriObj = new Uri(uri, UriKind.Absolute);

                if ((uriObj.Scheme != "http") && (uriObj.Scheme != "https"))
                {
                    throw new ArgumentException($"{name} should use http/https", name);
                }
            }
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
            catch (UriFormatException ex)
            {
                throw new ArgumentException(ex.Message, name, ex);
            }
#else
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message, name, ex);
            }
#endif
        }

        /// <summary>
        /// Validates that provided Uri is valid and absolute.
        /// </summary>
        /// <param name="uri">Uri to validate.</param>
        /// <param name="name">Argument <c>name</c>.</param>
        public static void ValidAbsoluteUri(string uri, [InvokerParameterName] string name)
        {
            NonNullOrWhiteSpace(uri, name);

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Uri(uri, UriKind.Absolute);
            }
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
            catch (UriFormatException ex)
            {
                throw new ArgumentException(ex.Message, name, ex);
            }
#else
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message, name, ex);
            }
#endif
        }
    }
}