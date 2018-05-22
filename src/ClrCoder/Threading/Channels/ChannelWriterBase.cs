// <copyright file="ChannelWriterBase.cs" company="ClrCoder project">
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
    /// Provides a base class for writing to a channel in the batch mode possibly with the zero-copy.
    /// </summary>
    /// <typeparam name="T">Specifies the type of data that may be written to the channel.</typeparam>
    [NoReorder]
    public abstract class ChannelWriterBase<T> : IChannelWriter<T>
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="ChannelWriterBase{T}"/> class.
        /// </summary>
        ~ChannelWriterBase()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public ValueTask Completion => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual void Complete(Exception error = null)
        {
            if (TryComplete(error))
            {
                return;
            }

            if (error == null)
            {
                throw new ChannelClosedException();
            }

            throw new ChannelClosedException(error);
        }

        /// <inheritdoc/>
        public virtual void CompleteWrite(int processedCount, ref ChannelWriterBufferSlice<T> writeSlice)
        {
            if ((processedCount < 0) || (processedCount > 1))
            {
                throw new ArgumentOutOfRangeException(
                    "processedCount should be greater or equal to zero and less than slice length.",
                    nameof(processedCount));
            }

            if (processedCount == 1)
            {
                WriteAsync(writeSlice.Span[0]).GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public virtual void PartialFree(int newCount, ref ChannelWriterBufferSlice<T> writeSlice)
        {
            if (newCount != 1)
            {
                throw new ArgumentOutOfRangeException(
                    "newCount should be less or equal than current and greater than zero.",
                    nameof(newCount));
            }

            // Do nothing.
        }

        /// <inheritdoc/>
        public virtual async ValueTask<ChannelWriterBufferSlice<T>> StartWriteAsync(
            int count,
            CancellationToken cancellationToken)
        {
            if (await WaitToWriteAsync(cancellationToken))
            {
                return new ChannelWriterBufferSlice<T>(new T[1], 0, 1, 0);
            }

            throw new ChannelClosedException();
        }

        /// <inheritdoc/>
        public virtual bool TryComplete(Exception error = null)
        {
            return false;
        }

        /// <inheritdoc/>
        public virtual ChannelWriterBufferSlice<T> TryStartWrite(int count)
        {
            return new ChannelWriterBufferSlice<T>(new T[1], 0, 1, 0);
        }

        /// <inheritdoc/>
        public virtual int TryWrite(ReadOnlySpan<T> items)
        {
            int writtenCount = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (!TryWrite(items[writtenCount]))
                {
                    break;
                }

                writtenCount++;
            }

            return writtenCount;
        }

        /// <inheritdoc/>
        public abstract bool TryWrite(T item);

        /// <inheritdoc/>
        public virtual bool TryWriteWithOwnership(ArraySegment<T> items)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public abstract ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public virtual async ValueTask<int> WriteAsync(
            ArraySegment<T> items,
            CancellationToken cancellationToken = default)
        {
            if (await WaitToWriteAsync(cancellationToken))
            {
                int DoWrite()
                {
                    ReadOnlySpan<T> itemsSpan = items;
                    int writtenCount = 0;
                    for (int i = 0; i < itemsSpan.Length; i++)
                    {
                        if (!TryWrite(itemsSpan[writtenCount]))
                        {
                            break;
                        }

                        writtenCount++;
                    }

                    return writtenCount;
                }

                return DoWrite();
            }

            throw new ChannelClosedException();
        }

        /// <inheritdoc/>
        public abstract int TryWrite<TItems>(TItems items)
            where TItems : IEnumerable<T>;

        /// <inheritdoc/>
        public virtual async ValueTask<int> WriteAsync<TItems>(
            TItems items,
            CancellationToken cancellationToken = default)
            where TItems : IEnumerable<T>
        {
            if (await WaitToWriteAsync(cancellationToken))
            {
                int writtenCount = 0;
                foreach (var item in items)
                {
                    if (!TryWrite(item))
                    {
                        break;
                    }

                    writtenCount++;
                }

                return writtenCount;
            }

            throw new ChannelClosedException();
        }

        /// <inheritdoc/>
        public ValueTask WriteAsync(T item, CancellationToken cancellationToken = default)
        {
            try
            {
                return
                    cancellationToken.IsCancellationRequested
                        ? new ValueTask(Task.FromCanceled(cancellationToken))
                        : TryWrite(item)
                            ? default
                            : new ValueTask(WriteAsyncCore(item, cancellationToken));
            }
            catch (Exception e)
            {
                return new ValueTask(Task.FromException(e));
            }
        }

        /// <inheritdoc/>
        public ValueTask<bool> WriteWithOwnershipAsync(
            ArraySegment<T> items,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
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

        private async Task WriteAsyncCore(T innerItem, CancellationToken ct)
        {
            while (await WaitToWriteAsync(ct).ConfigureAwait(false))
            {
                if (TryWrite(innerItem))
                {
                    return;
                }
            }

            throw new ChannelClosedException();
        }
    }
}

#endif