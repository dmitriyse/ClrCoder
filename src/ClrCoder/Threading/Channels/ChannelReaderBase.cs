// <copyright file="ChannelReaderBase.cs" company="ClrCoder project">
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

    using JetBrains.Annotations;

    /// <summary>
    /// Provides a base class for reading from a channel in the batch mode possibly with the zero-copy.
    /// </summary>
    /// <typeparam name="T">Specifies the type of data that may be read from the channel.</typeparam>
    [NoReorder]
    public abstract class ChannelReaderBase<T> : IChannelReader<T>
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="ChannelReaderBase{T}"/> class.
        /// </summary>
        ~ChannelReaderBase()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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

        /// <inheritdoc/>
        public virtual ValueTask ValueTaskCompletion => new ValueTask(TaskEx.NeverCompletingTaskValue);

        /// <inheritdoc/>
        public virtual ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<T>(Task.FromCanceled<T>(cancellationToken));
            }

            try
            {
                if (TryRead(out T fastItem))
                {
                    return new ValueTask<T>(fastItem);
                }
            }
            catch (Exception exc) when (!(exc is ChannelClosedException || exc is OperationCanceledException))
            {
                return new ValueTask<T>(Task.FromException<T>(exc));
            }

            return ReadAsyncCore(cancellationToken);

            async ValueTask<T> ReadAsyncCore(CancellationToken ct)
            {
                while (true)
                {
                    if (!await WaitToReadAsync(ct).ConfigureAwait(false))
                    {
                        throw new ChannelClosedException();
                    }

                    if (TryRead(out T item))
                    {
                        return item;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public abstract bool TryRead(out T item);

        /// <inheritdoc/>
        public abstract ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract ValueTask<ChannelReaderBufferSlice<T>> StartReadAsync(
            int count,
            CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract ChannelReaderBufferSlice<T> TryStartRead(int count);

        /// <inheritdoc/>
        public abstract void CompleteRead(int processedCount, ref ChannelReaderBufferSlice<T> slice);

        /// <inheritdoc/>
        public abstract void PartialFree(int newCount, ref ChannelReaderBufferSlice<T> slice);

        /// <inheritdoc/>
        public virtual int TryRead(Span<T> outBuffer)
        {
            int readCount = 0;
            for (int i = 0; i < outBuffer.Length; i++)
            {
                if (!TryRead(out outBuffer[readCount]))
                {
                    break;
                }

                readCount++;
            }

            return readCount;
        }

        /// <inheritdoc/>
        public bool TryRead(int count, out ArraySegment<T> items)
        {
            if (TryRead(out var item))
            {
                items = new ArraySegment<T>(new[] { item });
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public ValueTask<ArraySegment<T>> ReadAsync(int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<ArraySegment<T>>(Task.FromCanceled<ArraySegment<T>>(cancellationToken));
            }

            try
            {
                if (TryRead(out T fastItem))
                {
                    return new ValueTask<ArraySegment<T>>(new ArraySegment<T>(new[] { fastItem }));
                }
            }
            catch (Exception exc) when (!(exc is ChannelClosedException || exc is OperationCanceledException))
            {
                return new ValueTask<ArraySegment<T>>(Task.FromException<ArraySegment<T>>(exc));
            }

            return ReadAsyncCore(cancellationToken);

            async ValueTask<ArraySegment<T>> ReadAsyncCore(CancellationToken ct)
            {
                while (true)
                {
                    if (!await WaitToReadAsync(ct).ConfigureAwait(false))
                    {
                        throw new ChannelClosedException();
                    }

                    if (TryRead(out T item))
                    {
                        return new ArraySegment<T>(new[] { item });
                    }
                }
            }
        }
    }
}

#endif