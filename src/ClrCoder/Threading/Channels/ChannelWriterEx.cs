// <copyright file="ChannelWriterEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base class for writing to a channel in the batch mode possibly with the zero-copy.
    /// </summary>
    /// <typeparam name="T">Specifies the type of data that may be written to the channel.</typeparam>
    public abstract class ChannelWriterEx<T> : ChannelWriter<T>, IChannelWriter<T>
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="ChannelWriterEx{T}"/> class.
        /// </summary>
        ~ChannelWriterEx()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public abstract void CompleteWrite(int processedCount, ref ChannelWriterBufferSlice<T> writeSlice);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public abstract void PartialFree(int newCount, ref ChannelWriterBufferSlice<T> writeSlice);

        /// <inheritdoc/>
        public abstract bool TryStartWrite(int count, out ChannelWriterBufferSlice<T> slice);

        /// <inheritdoc/>
        public abstract bool TryWrite(ReadOnlySpan<T> items, out int written);

        /// <inheritdoc/>
        public abstract bool TryWriteWithOwnership(T[] items);

        /// <inheritdoc/>
        public abstract ValueTask<bool> WaitToWriteValueTaskAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract ValueTask<int> WriteAsync(ReadOnlySpan<T> items, CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract ValueTask<ValueVoid> WriteAsyncWithOwnership(
            T[] items,
            CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract ValueTask<ValueVoid> WriteValueTaskAsync(T item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Releases the unmanaged resources used by the Component and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/>
        /// to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}

#endif