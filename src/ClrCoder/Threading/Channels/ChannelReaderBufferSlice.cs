// <copyright file="ChannelReaderBufferSlice.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;

    /// <summary>
    /// The channel reader allocated buffer slice.
    /// </summary>
    /// <remarks>
    /// Memory can also be encapsulated in this <see langword="struct"/>.
    /// </remarks>
    /// <typeparam name="T">Specifies the type of data that may be read from the channel.</typeparam>
    public struct ChannelReaderBufferSlice<T>
    {
        private T[] _array;

        private int _start;

        private int _length;

        private int _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelReaderBufferSlice{T}"/> struct.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="start">The slice start in the array.</param>
        /// <param name="length">The slice length.</param>
        /// <param name="id">The slice identifier.</param>
        public ChannelReaderBufferSlice(T[] array, int start, int length, int id)
        {
#if DEBUG

// TODO: checks
#endif
            _array = array;
            _start = start;
            _length = length;
            _id = id;
        }

        /// <summary>
        /// Decreases slice length.
        /// </summary>
        /// <param name="newLength">The new length.</param>
        public void DecreaseLength(int newLength)
        {
            _length = newLength;
        }

        /// <summary>
        /// The span interface for reading.
        /// </summary>
        public ReadOnlySpan<T> Span => new ReadOnlySpan<T>(_array, _start, _length);

        /// <summary>
        /// The slice id.
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// The length of the slice.
        /// </summary>
        public int Length => _length;
    }
}

#endif