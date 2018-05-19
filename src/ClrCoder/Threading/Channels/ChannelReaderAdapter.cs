// <copyright file="ChannelReaderAdapter.cs" company="ClrCoder project">
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

    using Validation;

    /// <summary>
    /// Adds <see cref="IChannelReader{T}"/> contract to the BCL <see cref="ChannelReader{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the item in the channel.</typeparam>
    public class ChannelReaderAdapter<T> : IChannelReader<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelReaderAdapter{T}"/> class.
        /// </summary>
        /// <param name="innerReader">The BCL channel reader.</param>
        public ChannelReaderAdapter(ChannelReader<T> innerReader)
        {
            VxArgs.NotNull(innerReader, nameof(innerReader));
            InnerReader = innerReader;
            Completion = new ValueTask(innerReader.Completion);
        }

        /// <inheritdoc/>
        public ValueTask Completion { get; }

        /// <summary>
        /// The inner channel reader.
        /// </summary>
        public ChannelReader<T> InnerReader { get; }

        /// <inheritdoc/>
        public void CompleteRead(int processedCount, ref ChannelReaderBufferSlice<T> slice)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do nothing.
        }

        /// <inheritdoc/>
        public void PartialFree(int newCount, ref ChannelReaderBufferSlice<T> slice)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
        {
            return InnerReader.ReadAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public ValueTask<ArraySegment<T>> ReadAsync(int count, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<ChannelReaderBufferSlice<T>> StartReadAsync(
            int count,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryRead(out T item)
        {
            return InnerReader.TryRead(out item);
        }

        /// <inheritdoc/>
        public int TryRead(Span<T> outBuffer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryRead(int count, out ArraySegment<T> items)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ChannelReaderBufferSlice<T> TryStartRead(int count)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
        {
            return InnerReader.WaitToReadAsync(cancellationToken);
        }
    }
}
#endif