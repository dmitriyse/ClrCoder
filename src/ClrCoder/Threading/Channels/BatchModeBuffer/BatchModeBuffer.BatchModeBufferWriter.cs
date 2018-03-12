// <copyright file="BatchModeBuffer.BatchModeBufferWriter.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Validation;

    /// <content>The <see cref="BatchModeBufferWriter"/> implementation.</content>
    public partial class BatchModeBuffer<T>
    {
        private class BatchModeBufferWriter : ChannelWriterEx<T>
        {
            private readonly BatchModeBuffer<T> _owner;

            public BatchModeBufferWriter(BatchModeBuffer<T> owner)
            {
                _owner = owner;
            }

            /// <inheritdoc/>
            public override void CompleteWrite(int processedCount, ref ChannelWriterBufferSlice<T> writeSlice)
            {
                lock (_owner._syncRoot)
                {
                    var sliceEntry = _owner._sliceEntriesRegistry[writeSlice.Id];

                    // int allocatedLength = sliceEntry.AllocatedLength;
                    int unprocessedCount = sliceEntry.Length - processedCount;
                    sliceEntry.Length = processedCount;
                    sliceEntry.Status = SliceEntryStatus.Data;

                    // Update global state
                    _owner._allocLimitLeft += unprocessedCount;

                    if (_owner.IsLastEntry(sliceEntry))
                    {
                        // Update global state
                        _owner._allocatedSize -= unprocessedCount;

                        // Updating entry.
                        if (processedCount != 0)
                        {
                            // Trying to merge to th prev entry.
                            SliceEntry prev = sliceEntry.Prev;
                            if (!_owner.IsFirstEntry(sliceEntry)
                                && (prev.Status == SliceEntryStatus.Data)
                                && _owner.IsConnectedWithNext(prev))
                            {
                                _owner.JoinToPrevEntry(sliceEntry);
                            }
                        }
                        else
                        {
                            _owner.RemoveEntry(sliceEntry);
                        }
                    }
                    else
                    {
                        if (processedCount != 0)
                        {
                            SliceEntry entryToTryJoinWithPrev = sliceEntry;

                            // Trying to merge to the next entry.
                            var next = sliceEntry.Next;
                            if ((unprocessedCount == 0)
                                && (next.Status == SliceEntryStatus.Data) && _owner.IsConnectedWithNext(sliceEntry))
                            {
                                entryToTryJoinWithPrev = _owner.JoinToNextEntry(sliceEntry);
                            }

                            // Trying to merge to th prev entry.
                            SliceEntry prev = entryToTryJoinWithPrev.Prev;
                            if (!_owner.IsFirstEntry(entryToTryJoinWithPrev)
                                && (prev.Status == SliceEntryStatus.Data)
                                && _owner.IsConnectedWithNext(prev))
                            {
                                _owner.JoinToPrevEntry(entryToTryJoinWithPrev);
                            }
                        }
                        else
                        {
                            _owner.RemoveEntry(sliceEntry);
                        }
                    }
                }
            }

            /// <inheritdoc/>
            public override bool TryWrite(ReadOnlySpan<T> items, out int written)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override ValueTask<int> WriteAsync(ReadOnlySpan<T> items, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override bool TryWriteWithOwnership(T[] items)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override ValueTask<ValueVoid> WriteAsyncWithOwnership(T[] items, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override void PartialFree(int newCount, ref ChannelWriterBufferSlice<T> writeSlice)
            {
#if DEBUG
                VxArgs.NonNegative(newCount, nameof(newCount));
#endif
                lock (_owner._syncRoot)
                {
                    var sliceEntry = _owner._sliceEntriesRegistry[writeSlice.Id];

                    Debug.Assert(newCount <= sliceEntry.Length, "newCount <= sliceEntry.AllocatedLength");

                    int decreaseSize = sliceEntry.Length - newCount;

                    // Updating global state
                    _owner._allocLimitLeft += decreaseSize;

                    if (_owner.IsLastEntry(sliceEntry))
                    {
                        // Updating global state
                        _owner._allocatedSize -= decreaseSize;
                    }

                    sliceEntry.Length = newCount;
                }

                writeSlice.DecreaseLength(newCount);
            }

            /// <inheritdoc/>
            public override ValueTask<bool> WaitToWriteValueTaskAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override ValueTask<ValueVoid> WriteValueTaskAsync(T item, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override bool TryComplete(Exception error = null)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override bool TryWrite(T item)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override bool TryStartWrite(int count, out ChannelWriterBufferSlice<T> slice)
            {
#if DEBUG
                VxArgs.NonNegative(count, nameof(count));
#endif
                lock (_owner._syncRoot)
                {
                    // Even we have space we need to strictly follow _owner._maxBufferLength limit.
                    count = Math.Min(count, _owner._allocLimitLeft);

                    // Checking if there are any free space.
                    if ((_owner._allocatedSize < _owner._buffer.Length) || (count == 0))
                    {
                        slice = default;
                        return false;
                    }

                    int bufferLength = _owner._buffer.Length;

                    int writeStartOffset = (_owner._freePosition + _owner._allocatedSize) & _owner._bufferSizeMask;
                    int sliceLength = _owner._freePosition - writeStartOffset;

                    if (sliceLength > 0)
                    {
                        // The case where unallocated space is not overlapped over buffer end.
                        sliceLength = Math.Min(count, sliceLength);
                        _owner._allocatedSize += sliceLength;
                    }
                    else
                    {
                        // Unallocated space is overlapped over buffer end
                        // ---
                        sliceLength = bufferLength - writeStartOffset;
                        if (count < sliceLength)
                        {
                            // First part of unallocated space is big enough to fit required buffer size.
                            sliceLength = count;
                            _owner._allocatedSize += sliceLength;
                        }
                        else
                        {
                            // First part of unallocated space cannot fit all required "count"
                            // So we try to choose biggest part of unallocated space: first or second (overlapping).
                            // ====
                            if (_owner._freePosition < sliceLength)
                            {
                                // Choosing non overlapping part.
                                _owner._allocatedSize += sliceLength;
                            }
                            else
                            {
                                // Choosing second (overlapping part).
                                // ----
                                writeStartOffset = 0;
                                if (_owner._freePosition < count)
                                {
                                    // If second part is less than required count, than we allocating buffer totally.
                                    sliceLength = _owner._freePosition;
                                    _owner._allocatedSize = bufferLength;
                                }
                                else
                                {
                                    // Second part have more than enough space to fit "count"
                                    _owner._allocatedSize += sliceLength + count;
                                    sliceLength = count;
                                }
                            }
                        }
                    }

                    _owner._allocLimitLeft -= sliceLength;

                    // Allocating slice entry
                    var newEntry = _owner.InsertNewEntryLast();

                    newEntry.Status = SliceEntryStatus.AllocatedForWrite;
                    newEntry.Start = writeStartOffset;
                    newEntry.Length = sliceLength;

                    // Returning result.
                    slice = new ChannelWriterBufferSlice<T>(_owner._buffer, writeStartOffset, sliceLength, newEntry.Id);
                    return true;
                }
            }

            /// <inheritdoc/>
            public override Task<bool> WaitToWriteAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif