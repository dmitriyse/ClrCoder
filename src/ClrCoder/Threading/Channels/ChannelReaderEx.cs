// <copyright file="ChannelBatchReader.cs" company="ClrCoder project">
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
    /// Provides a base class for reading from a channel in the batch mode possibly with the zero-copy.
    /// </summary>
    /// <typeparam name="T">Specifies the type of data that may be read from the channel.</typeparam>
    public abstract class ChannelReaderEx<T> : ChannelReader<T>, IChannelReader<T>
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="ChannelReaderEx{T}"/> class.
        /// </summary>
        ~ChannelReaderEx()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public abstract ValueTask<ValueVoid> ValueTaskCompletion { get; }

        /// <summary>
        /// Finalizes read operation.
        /// </summary>
        /// <param name="processedCount">The number of processed items.</param>
        /// <param name="slice">The buffer slice.</param>
        public abstract void CompleteRead(int processedCount, ref ChannelReaderBufferSlice<T> slice);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases a part of previously allocated buffer slice.
        /// </summary>
        /// <param name="newCount">The number of items to remain allocated.</param>
        /// <param name="slice">The buffer slice.</param>
        public abstract void PartialFree(int newCount, ref ChannelReaderBufferSlice<T> slice);

        /// <inheritdoc/>
        public abstract bool TryRead(int count, out T[] items);

        /// <inheritdoc/>
        public abstract ValueTask<T[]> ReadAsync(int count, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract bool TryRead(Span<T> outBuffer, out int readCount);

        /// <inheritdoc/>
        public abstract bool TryStartRead(int count, out ChannelReaderBufferSlice<T> slice);

        /// <inheritdoc/>
        public abstract ValueTask<bool> WaitToReadValueTaskAsync(CancellationToken cancellationToken = default);

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