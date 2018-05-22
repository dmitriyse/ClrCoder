// <copyright file="ChannelAdapter.cs" company="ClrCoder project">
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

    using Validation;

    /// <summary>
    /// The adapter that wraps <see cref="System.Threading.Channels.Channel{T}"/> and provides <see cref="IChannelReader{T}"/>
    /// and <see cref="IChannelWriter{T}"/> interfaces.
    /// </summary>
    /// <remarks>TODO: Implement Channel adapter with different input and output item types.</remarks>
    /// <typeparam name="T">The types of the items in the channel.</typeparam>
    [NoReorder]
    [PublicAPI]
    public class ChannelAdapter<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelAdapter{T}"/> class.
        /// </summary>
        /// <param name="innerChannel">The inner channel.</param>
        public ChannelAdapter(Channel<T> innerChannel)
        {
            VxArgs.NotNull(innerChannel, nameof(innerChannel));

            InnerChannel = innerChannel;
            Reader = new ChannelAdapterReader(this);
            Writer = new ChannelAdapterWriter(this);
        }

        /// <summary>
        /// THe inner channel that was wrapped.
        /// </summary>
        public Channel<T> InnerChannel { get; }

        /// <summary>
        /// The channel reader.
        /// </summary>
        public IChannelReader<T> Reader { get; }

        /// <summary>
        /// The channel writer.
        /// </summary>
        public IChannelWriter<T> Writer { get; }

        private class ChannelAdapterReader : IChannelReader<T>
        {
            private readonly ChannelAdapter<T> _parent;

            private readonly ChannelReader<T> _innerReader;

            public ChannelAdapterReader(ChannelAdapter<T> parent)
            {
                _parent = parent;
                _innerReader = _parent.InnerChannel.Reader;
            }

            /// <inheritdoc/>
            public ValueTask Completion => new ValueTask(_innerReader.Completion);

            /// <inheritdoc/>
            public void Complete(Exception error = null)
            {
                _parent.Writer.Complete(error);
            }

            /// <inheritdoc/>
            public void CompleteRead(int processedCount, ref ChannelReaderBufferSlice<T> slice) =>
                throw new NotImplementedException();

            /// <inheritdoc/>
            public void Dispose()
            {
                // Do nothing.
            }

            /// <inheritdoc/>
            public void PartialFree(int newCount, ref ChannelReaderBufferSlice<T> slice) =>
                throw new NotImplementedException();

            /// <inheritdoc/>
            public ValueTask<T> ReadAsync(CancellationToken cancellationToken = default) =>
                _innerReader.ReadAsync(cancellationToken);

            /// <inheritdoc/>
            public ValueTask<ArraySegment<T>> ReadAsync(int count, CancellationToken cancellationToken) =>
                throw new NotImplementedException();

            public ValueTask<ChannelReaderBufferSlice<T>> StartReadAsync(
                int count,
                CancellationToken cancellationToken = default) =>
                throw new NotImplementedException();

            /// <inheritdoc/>
            public bool TryComplete(Exception error = null)
            {
                return _parent.InnerChannel.Writer.TryComplete(error);
            }

            /// <inheritdoc/>
            public bool TryRead(out T item) => _innerReader.TryRead(out item);

            /// <inheritdoc/>
            public int TryRead(Span<T> outBuffer) => throw new NotImplementedException();

            /// <exception cref="NotImplementedException"></exception>
            public bool TryRead(int count, out ArraySegment<T> items) => throw new NotImplementedException();

            /// <inheritdoc/>
            public ChannelReaderBufferSlice<T> TryStartRead(int count) => throw new NotImplementedException();

            /// <inheritdoc/>
            public ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default) =>
                _innerReader.WaitToReadAsync(cancellationToken);
        }

        private class ChannelAdapterWriter : IChannelWriter<T>
        {
            private readonly ChannelAdapter<T> _parent;

            private readonly ChannelWriter<T> _innerWriter;

            public ChannelAdapterWriter(ChannelAdapter<T> parent)
            {
                _parent = parent;
                _innerWriter = parent.InnerChannel.Writer;
            }

            /// <inheritdoc/>
            public ValueTask Completion => new ValueTask(_parent.InnerChannel.Reader.Completion);

            /// <inheritdoc/>
            public void Complete(Exception error = null)
            {
                _innerWriter.Complete(error);
            }

            /// <inheritdoc/>
            public void CompleteWrite(int processedCount, ref ChannelWriterBufferSlice<T> writeSlice) =>
                throw new NotImplementedException();

            public void Dispose()
            {
                // Do nothing.
            }

            /// <inheritdoc/>
            public void PartialFree(int newCount, ref ChannelWriterBufferSlice<T> writeSlice) =>
                throw new NotImplementedException();

            /// <inheritdoc/>
            public ValueTask<ChannelWriterBufferSlice<T>> StartWriteAsync(
                int count,
                CancellationToken cancellationToken) =>
                throw new NotImplementedException();

            /// <inheritdoc/>
            public bool TryComplete(Exception error = null) => _innerWriter.TryComplete(error);

            /// <inheritdoc/>
            public ChannelWriterBufferSlice<T> TryStartWrite(int count) => throw new NotImplementedException();

            /// <inheritdoc/>
            public bool TryWrite(T item) => _innerWriter.TryWrite(item);

            /// <inheritdoc/>
            public int TryWrite(ReadOnlySpan<T> items) => throw new NotImplementedException();

            /// <inheritdoc/>
            public int TryWrite<TItems>(TItems items)
                where TItems : IEnumerable<T> =>
                throw new NotImplementedException();

            /// <inheritdoc/>
            public bool TryWriteWithOwnership(ArraySegment<T> items) => throw new NotImplementedException();

            /// <inheritdoc/>
            public ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default) =>
                _innerWriter.WaitToWriteAsync(cancellationToken);

            /// <inheritdoc/>
            public ValueTask WriteAsync(T item, CancellationToken cancellationToken = default) =>
                _innerWriter.WriteAsync(item, cancellationToken);

            /// <inheritdoc/>
            public ValueTask<int> WriteAsync(ArraySegment<T> items, CancellationToken cancellationToken = default) =>
                throw new NotImplementedException();

            /// <inheritdoc/>
            public ValueTask<int> WriteAsync<TItems>(TItems items, CancellationToken cancellationToken = default)
                where TItems : IEnumerable<T> =>
                throw new NotImplementedException();

            /// <inheritdoc/>
            public ValueTask<bool> WriteWithOwnershipAsync(
                ArraySegment<T> items,
                CancellationToken cancellationToken = default) =>
                throw new NotImplementedException();
        }
    }
}

#endif