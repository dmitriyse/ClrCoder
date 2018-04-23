// <copyright file="IChannelReader.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1

namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Provides a base contract for reading from a channel.
    /// </summary>
    /// <typeparam name="T">
    /// Specifies the type of data that may be read from the channel.
    /// </typeparam>
    [NoReorder]
    public interface IChannelReader<T> : IDisposable
    {
        /// <summary>
        /// Gets a <see cref="Task"/> that completes when no more data will ever
        /// be available to be read from this channel.
        /// </summary>
        /// <remarks>
        /// Task is slow, we needs IValueTaskSource everywhere !
        /// </remarks>
        ValueTask Completion { get; }

        /// <summary>
        /// Asynchronously reads an item from the channel.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the read operation.</param>
        /// <returns>A <see cref="ValueTask{T}"/> that represents the asynchronous read operation.</returns>
        ValueTask<T> ReadAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to read an item to the channel.
        /// </summary>
        /// <param name="item">The read item, or a default value if no item could be read.</param>
        /// <returns><see langword="true"/> if an item was read; otherwise, <see langword="false"/> if no item was read.</returns>
        bool TryRead([CanBeNull] out T item);

        /// <summary>
        /// Returns a <see cref="ValueTask{T}"/> that will complete when data is available to read.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the wait operation.</param>
        /// <returns>
        /// A <see cref="ValueTask{T}"/> that will complete with a <c>true</c> result when data is available to
        /// read
        /// or with a <c>false</c> result when no further data will ever be available to be read.
        /// </returns>
        ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default);

        #region The zero-copy batch mode interface

        /// <summary>
        /// Allocates a buffer slice with data.
        /// </summary>
        /// <remarks>
        /// Resulting slice can has less length than requested.
        /// </remarks>
        /// <param name="count">The preferred amount of items to read.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the read operation.</param>
        /// <returns>The buffer slice with the data. The length of the slice can be zero.</returns>
        ValueTask<ChannelReaderBufferSlice<T>> StartReadAsync(int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// Allocates a buffer slice with data.
        /// </summary>
        /// <remarks>
        /// Resulting slice can has less length than requested.
        /// </remarks>
        /// <param name="count">The preferred amount of items to read.</param>
        /// <returns>The buffer slice with the data. The length of the slice can be zero.</returns>
        ChannelReaderBufferSlice<T> TryStartRead(int count);

        /// <summary>
        /// Finalizes read operation.
        /// </summary>
        /// <param name="processedCount">The number of processed items.</param>
        /// <param name="slice">The buffer slice.</param>
        void CompleteRead(int processedCount, ref ChannelReaderBufferSlice<T> slice);

        /// <summary>
        /// Releases a part of previously allocated buffer slice.
        /// </summary>
        /// <param name="newCount">The number of items to remain allocated.</param>
        /// <param name="slice">The buffer slice.</param>
        void PartialFree(int newCount, ref ChannelReaderBufferSlice<T> slice);

        #endregion

        #region The batch mode interface with copying.

        /// <summary>
        /// Attempts to read with copying the specified amount of items from the channel.
        /// </summary>
        /// <param name="outBuffer">The output buffer to read to.</param>
        /// <returns>The amount of items read (can be less than <see cref="outBuffer"/> length.</returns>
        int TryRead(Span<T> outBuffer);

        /// <summary>
        /// Attempts to read the specified amount of items to an array.
        /// </summary>
        /// <remarks>
        /// This method allows to pass-through pushed array without changes and copying.
        /// You becomes owner of the resulting array segment.
        /// </remarks>
        /// <param name="count">The number of items to read.</param>
        /// <param name="items">The array with read items. (Array can have less items than has been requested.)</param>
        /// <returns><see langword="true"/> if at least one item was read; otherwise, <see langword="false"/> if no items was read.</returns>
        bool TryRead(int count, out ArraySegment<T> items);

        /// <summary>
        /// Asynchronously reads the specified amount of items from the channel.
        /// </summary>
        /// <remarks>
        /// This method allows to pass-through pushed array without changes and copying.
        /// </remarks>
        /// <param name="count">The number of items to read.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the read operation.</param>
        /// <returns>A <see cref="ValueTask{T}"/> that represents the asynchronous read operation.</returns>
        ValueTask<ArraySegment<T>> ReadAsync(int count, CancellationToken cancellationToken);

        #endregion
    }
}

#endif