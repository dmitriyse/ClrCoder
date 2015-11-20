﻿using System;
using System.Collections.Generic;

namespace ClrCoder.System
{
    /// <summary>
    /// Valued alternative to the <see cref="Tuple{T1,T2}"/> class.<br/>
    /// TODO: Optimize performance. 
    /// </summary>
    /// <typeparam name="T1">First item type.</typeparam>
    /// <typeparam name="T2">Second item type.</typeparam>
    public struct ValuedTuple<T1, T2> : IEquatable<ValuedTuple<T1, T2>>
    {
        /// <summary>
        /// Item1 value.
        /// </summary>
        public readonly T1 Item1;

        /// <summary>
        /// Item2 value.
        /// </summary>
        public readonly T2 Item2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValuedTuple{T1, T2}"/> struct.
        /// </summary>
        /// <param name="item1">Item1 value.</param>
        /// <param name="item2">Item2 value.</param>
        public ValuedTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        /// <summary>
        /// Equality <see langword="operator"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>true, if operands are equal, false otherwise.</returns>
        public static bool operator ==(ValuedTuple<T1, T2> left, ValuedTuple<T1, T2> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality <see langword="operator"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>true, if operands are different, false otherwise.</returns>
        public static bool operator !=(ValuedTuple<T1, T2> left, ValuedTuple<T1, T2> right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(ValuedTuple<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                   && EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ValuedTuple<T1, T2> && Equals((ValuedTuple<T1, T2>)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T1>.Default.GetHashCode(Item1) * 397)
                       ^ EqualityComparer<T2>.Default.GetHashCode(Item2);
            }
        }
    }
}