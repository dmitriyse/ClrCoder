// <copyright file="IntervalExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logic
{
    /// <summary>
    /// <see cref="Interval{T}"/> helper extensions.
    /// </summary>
    public static class IntervalExtensions
    {
        /// <summary>
        /// Use int value as a start for <see cref="Interval{T}"/>.
        /// </summary>
        /// <param name="start">Int value to use as start.</param>
        /// <param name="length">Length of the interval</param>
        /// <returns>Interval from the specified value with the specified length.</returns>
        public static Interval<int> WithLength(this int start, int length)
        {
            return new Interval<int>(start, start + length);
        }
    }
}