// <copyright file="ChannelWriterAdapter.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1

namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Adds <see cref="IChannelWriter{T}"/> contract to the <see cref="ChannelWriter{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the channel.</typeparam>
    [PublicAPI]
    public class ChannelWriterAdapter<T> : IChannelWriter<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelReaderAdapter{T}"/> class.
        /// </summary>
        /// <param name="innerWriter">The inner BCL channel writer.</param>
        public ChannelWriterAdapter(ChannelWriter<T> innerWriter)
        {
            InnerWriter = innerWriter;
        }

        /// <inheritdoc/>
        public ValueTask Completion => throw new NotSupportedException();

        /// <summary>
        /// The inner BCL channel writer.
        /// </summary>
        public ChannelWriter<T> InnerWriter { get; }

        /// <inheritdoc/>
        public void Complete(Exception error = null)
        {
            InnerWriter.Complete(error);
        }

        /// <inheritdoc/>
        public void CompleteWrite(int processedCount, ref ChannelWriterBufferSlice<T> writeSlice)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do nothing.
        }

        /// <inheritdoc/>
        public void PartialFree(int newCount, ref ChannelWriterBufferSlice<T> writeSlice)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<ChannelWriterBufferSlice<T>> StartWriteAsync(int count, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryComplete(Exception error = null)
        {
            return InnerWriter.TryComplete(error);
        }

        /// <inheritdoc/>
        public ChannelWriterBufferSlice<T> TryStartWrite(int count)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryWrite(T item)
        {
            return InnerWriter.TryWrite(item);
        }

        /// <inheritdoc/>
        public int TryWrite(ReadOnlySpan<T> items)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int TryWrite<TItems>(TItems items)
            where TItems : IEnumerable<T>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryWriteWithOwnership(ArraySegment<T> items)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default)
        {
            return InnerWriter.WaitToWriteAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public ValueTask WriteAsync(T item, CancellationToken cancellationToken = default)
        {
            return InnerWriter.WriteAsync(item, cancellationToken);
        }

        /// <inheritdoc/>
        public ValueTask<int> WriteAsync(ArraySegment<T> items, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<int> WriteAsync<TItems>(TItems items, CancellationToken cancellationToken = default)
            where TItems : IEnumerable<T>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask<bool> WriteWithOwnershipAsync(
            ArraySegment<T> items,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
#endif