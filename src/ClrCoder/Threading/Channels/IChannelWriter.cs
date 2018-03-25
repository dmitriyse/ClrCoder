// <copyright file="IChannelWriter.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1

namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Provides a base class for writing to a channel.
    /// </summary>
    /// <typeparam name="T">Specifies the type of data that may be written to the channel.</typeparam>
    [NoReorder]
    public interface IChannelWriter<T> : IDisposable
    {
        /// <summary>
        /// Mark the channel as being complete, meaning no more items will be written to it.
        /// </summary>
        /// <param name="error">Optional Exception indicating a failure that's causing the channel to complete.</param>
        /// <exception cref="InvalidOperationException">The channel has already been marked as complete.</exception>
        void Complete(Exception error = null);

        /// <summary>
        /// Attempts to mark the channel as being completed, meaning no more data will be written to it.
        /// </summary>
        /// <param name="error">
        /// An <see cref="Exception"/> indicating the failure causing no more data to be written, or
        /// null for success.
        /// </param>
        /// <returns>
        /// true if this operation successfully completes the channel; otherwise, false if the channel could not be marked for
        /// completion,
        /// for example due to having already been marked as such, or due to not supporting completion.
        /// </returns>
        bool TryComplete(Exception error = null);

        /// <summary>
        /// Attempts to write the specified item to the channel.
        /// </summary>
        /// <param name="item">The item to write.</param>
        /// <returns>true if the item was written; otherwise, false if it wasn't written.</returns>
        bool TryWrite(T item);

        /// <summary>
        /// Returns a <see cref="ValueTask{T}"/> that will complete when space is available to write an
        /// item.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the wait operation.</param>
        /// <returns>
        /// A <see cref="ValueTask{T}"/> that will complete with a <c>true</c> result when space is available to
        /// write an item
        /// or with a <c>false</c> result when no further writing will be permitted.
        /// </returns>
        ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default);

        /// <summary>Asynchronously writes an item to the channel.</summary>
        /// <param name="item">The value to write to the channel.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> used to cancel the write
        /// operation.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask{T}"/> that will complete with a <c>true</c> result when item was written
        /// or with a <c>false</c> result when no further writing will be permitted.
        /// </returns>
        ValueTask WriteAsync(T item, CancellationToken cancellationToken = default);

        #region The zero-copy batch mode interface

        /// <summary>
        /// Allocates a buffer slice to write to.
        /// </summary>
        /// <param name="count">The preferred amount of items to write.</param>
        /// <returns>
        /// The buffer slice with to write data to. The length of the slice is zero, if currently there is no room to
        /// write to.
        /// </returns>
        ChannelWriterBufferSlice<T> TryStartWrite(int count);

        /// <summary>
        /// Allocates a buffer slice to write to.
        /// </summary>
        /// <param name="count">The preferred amount of items to write.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> used to cancel the write
        /// operation.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the buffer slice was allocated; otherwise, <see langword="false"/> if it wasn't
        /// allocated.
        /// </returns>
        ValueTask<ChannelWriterBufferSlice<T>> StartWriteAsync(int count, CancellationToken cancellationToken);

        /// <summary>
        /// Releases a part of previously allocated buffer slice.
        /// </summary>
        /// <param name="newCount">The number of items to remain allocated.</param>
        /// <param name="writeSlice">The buffer slice.</param>
        void PartialFree(int newCount, ref ChannelWriterBufferSlice<T> writeSlice);

        /// <summary>
        /// Finalizes write operation.
        /// </summary>
        /// <param name="processedCount">The number of processed items.</param>
        /// <param name="writeSlice">The buffer slice.</param>
        void CompleteWrite(int processedCount, ref ChannelWriterBufferSlice<T> writeSlice);

        #endregion

        #region Copy-based batch mode interface

        /// <summary>
        /// Attempts to write the specified item to the channel.
        /// </summary>
        /// <param name="items">The item to write.</param>
        /// <returns>The amount of items written.</returns>
        int TryWrite(ReadOnlySpan<T> items);

        /// <summary>
        /// Asynchronously writes an item to the channel.
        /// </summary>
        /// <param name="items">The items to write to the channel.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> used to cancel the write
        /// operation.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask{T}"/> that represents the asynchronous write operation and returns the number of items
        /// has been written.
        /// </returns>
        ValueTask<int> WriteAsync(ArraySegment<T> items, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously writes an items to the channel.
        /// </summary>
        /// <param name="items">The items to write to the channel.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> used to cancel the write
        /// operation.
        /// </param>
        /// <returns>
        /// The number of items has been written.
        /// </returns>
        int TryWrite<TItems>(TItems items)
            where TItems : IEnumerable<T>;

        /// <summary>
        /// Asynchronously writes an item to the channel.
        /// </summary>
        /// <param name="items">The items to write to the channel.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> used to cancel the write
        /// operation.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask{T}"/> that represents the asynchronous write operation and returns the number of items
        /// has been written.
        /// </returns>
        ValueTask<int> WriteAsync<TItems>(TItems items, CancellationToken cancellationToken = default)
            where TItems : IEnumerable<T>;

        #endregion

        #region Array pass-through interface

        /// <summary>
        /// Attempts to write the specified items to the channel and gives ownership of the provided array to the channel.
        /// </summary>
        /// <remarks>
        /// This method allows to pass-through array to be read by the reader without changes to the array and any copying.
        /// </remarks>
        /// <param name="items">The item to write.</param>
        /// <returns>
        /// <see langword="true"/> if the items was fully written; otherwise, <see langword="false"/> if it wasn't fully
        /// written.
        /// </returns>
        bool TryWriteWithOwnership(ArraySegment<T> items);

        /// <summary>
        /// Asynchronously writes an item to the channel.
        /// </summary>
        /// <remarks>
        /// This method allows to pass-through array to be read by the reader without changes to the array and any copying.
        /// </remarks>
        /// <param name="items">The value to write to the channel.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> used to cancel the write
        /// operation.
        /// </param>
        /// <returns>A <see cref="ValueTask{T}"/> that represents the asynchronous write operation.</returns>
        ValueTask<bool> WriteWithOwnershipAsync(ArraySegment<T> items, CancellationToken cancellationToken = default);

        #endregion
    }
}

#endif