// <copyright file="ChannelWriterWithoutBatchMode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: It's a temporary solution.
    /// </summary>
    /// <typeparam name="T">The type of the items in the channel.</typeparam>
    public abstract class ChannelWriterWithoutBatchMode<T> : IChannelWriter<T>
    {
        /// <inheritdoc/>
        public abstract ValueTask Completion { get; }

        /// <inheritdoc/>
        public abstract void Complete(Exception error = null);

        void IChannelWriter<T>.CompleteWrite(int processedCount, ref ChannelWriterBufferSlice<T> writeSlice) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            // Do nothing.
        }

        void IChannelWriter<T>.PartialFree(int newCount, ref ChannelWriterBufferSlice<T> writeSlice) =>
            throw new NotSupportedException();

        ValueTask<ChannelWriterBufferSlice<T>> IChannelWriter<T>.StartWriteAsync(
            int count,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public abstract bool TryComplete(Exception error = null);

        ChannelWriterBufferSlice<T> IChannelWriter<T>.TryStartWrite(int count) => throw new NotSupportedException();

        /// <inheritdoc/>
        public abstract bool TryWrite(T item);

        int IChannelWriter<T>.TryWrite(ReadOnlySpan<T> items) => throw new NotSupportedException();

        int IChannelWriter<T>.TryWrite<TItems>(TItems items) => throw new NotSupportedException();

        bool IChannelWriter<T>.TryWriteWithOwnership(ArraySegment<T> items) => throw new NotSupportedException();

        /// <inheritdoc/>
        public abstract ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract ValueTask WriteAsync(T item, CancellationToken cancellationToken);

        ValueTask<int> IChannelWriter<T>.WriteAsync(ArraySegment<T> items, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        ValueTask<int> IChannelWriter<T>.WriteAsync<TItems>(TItems items, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        ValueTask<bool> IChannelWriter<T>.WriteWithOwnershipAsync(
            ArraySegment<T> items,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
#endif