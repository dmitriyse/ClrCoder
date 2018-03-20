// <copyright file="ChannelWriterBufferSlice.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading.Channels
{
    using System;

    /// <summary>
    /// The channel writer allocated buffer slice.
    /// </summary>
    /// <remarks>
    /// Memory can also be encapsulated in this <see langword="struct"/>.
    /// </remarks>
    /// <typeparam name="T">Specifies the type of data that may be written to the channel.</typeparam>
    public struct ChannelWriterBufferSlice<T>
    {
        private T[] _array;

        private int _start;

        private int _length;

        private int _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelWriterBufferSlice{T}"/> struct.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="start">The slice start in the array.</param>
        /// <param name="length">The slice length.</param>
        /// <param name="id">The slice identifier.</param>
        public ChannelWriterBufferSlice(T[] array, int start, int length, int id)
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
        /// The span interface for writing.
        /// </summary>
        public Span<T> Span => new Span<T>(_array, _start, _length);

        /// <summary>
        /// The slice id.
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// The length of the slice.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// Decreases slice length.
        /// </summary>
        /// <param name="newLength">The new length.</param>
        public void DecreaseLength(int newLength)
        {
            _length = newLength;
        }
    }
}