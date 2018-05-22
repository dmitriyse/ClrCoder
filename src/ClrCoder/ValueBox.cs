// <copyright file="ValueBox.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Makes any value to be wrapped as value type.
    /// </summary>
    /// <typeparam name="T">The value to be stored in the box.</typeparam>
    public struct ValueBox<T> : IEquatable<ValueBox<T>>
    {
        /// <inheritdoc/>
        public bool Equals(ValueBox<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return EqualityComparer<T>.Default.Equals(Value, ((ValueBox<T>)obj).Value);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueBox{T}"/> struct.
        /// </summary>
        /// <param name="value">The box content.</param>
        public ValueBox(T value)
        {
            Value = value;
        }

        /// <summary>
        /// The box content.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Implicitly converts box to value.
        /// </summary>
        /// <param name="box">The box to convert.</param>
        public static implicit operator T(ValueBox<T> box)
        {
            return box.Value;
        }

        /// <summary>
        /// Implicitly converts value to box.
        /// </summary>
        /// <param name="value">The value </param>
        public static implicit operator ValueBox<T>(T value)
        {
            return new ValueBox<T>(value);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Value != null ? Value.ToString() : "null";
        }
    }
}