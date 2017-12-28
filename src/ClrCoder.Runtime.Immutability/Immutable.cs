// <copyright file="Immutable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    /// <summary>
    /// Value wrapper that helps to validate immutabiliy.
    /// This is a polifill structure, this feature should be implemented as an extension to C# language.
    /// With the similar approach as non-nullable references.
    /// </summary>
    /// <remarks>
    /// TODO: Describe proposal on github.
    /// </remarks>
    /// <typeparam name="T">The type of the value.</typeparam>
    public struct Immutable<T>
    {
        public T Value { get; }

        private Immutable(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Implicitly converts immutable value to an original type.
        /// </summary>
        /// <param name="immutable">The immutable variable to unwrap.</param>
        public static explicit operator T(Immutable<T> immutable)
        {
            return immutable.Value;
        }

        /// <summary>
        /// Wraps value to an immutable variable.
        /// </summary>
        /// <param name="value">The value to wrap into immutable variable.</param>
        public static implicit operator Immutable<T>(T value)
        {
            ImmutabilityRuntime.AssumeImmutable<T, T>(value);
            return new Immutable<T>(value);
        }
    }
}