// <copyright file="ChannelReaderWithoutBatchMode.cs" company="ClrCoder project">
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
    public abstract class ChannelReaderWithoutBatchMode<T> : IChannelReader<T>
    {
        /// <inheritdoc/>
        public abstract ValueTask Completion { get; }

        /// <inheritdoc/>
        public abstract void Complete(Exception error = null);

        void IChannelReader<T>.CompleteRead(int processedCount, ref ChannelReaderBufferSlice<T> slice) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            // Do nothing.
        }

        void IChannelReader<T>.PartialFree(int newCount, ref ChannelReaderBufferSlice<T> slice) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public abstract ValueTask<T> ReadAsync(CancellationToken cancellationToken = default);

        ValueTask<ArraySegment<T>> IChannelReader<T>.ReadAsync(int count, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        ValueTask<ChannelReaderBufferSlice<T>> IChannelReader<T>.StartReadAsync(
            int count,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public abstract bool TryComplete(Exception error = null);

        /// <inheritdoc/>
        public abstract bool TryRead(out T item);

        int IChannelReader<T>.TryRead(Span<T> outBuffer) => throw new NotSupportedException();

        bool IChannelReader<T>.TryRead(int count, out ArraySegment<T> items) => throw new NotSupportedException();

        ChannelReaderBufferSlice<T> IChannelReader<T>.TryStartRead(int count) => throw new NotSupportedException();

        /// <inheritdoc/>
        public abstract ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default);
    }
}
#endif