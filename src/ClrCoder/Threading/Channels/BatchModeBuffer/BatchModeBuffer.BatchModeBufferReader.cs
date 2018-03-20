// <copyright file="BatchModeBuffer.BatchModeBufferReader.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Validation;

    /// <content>The <see cref="ModeBufferReaderEx"/> implementation.</content>
    public partial class BatchModeBuffer<T>
    {
        private class ModeBufferReaderEx : ChannelReaderBase<T>
        {
            private readonly BatchModeBuffer<T> _owner;

            public ModeBufferReaderEx(BatchModeBuffer<T> owner)
            {
                _owner = owner;
            }

            /// <inheritdoc/>
            public override Task Completion { get; }

            /// <inheritdoc/>
            public override ValueTask<VoidResult> ValueTaskCompletion { get; }

            /// <inheritdoc/>
            public override void CompleteRead(int processedCount, ref ChannelReaderBufferSlice<T> slice)
            {
#if DEBUG
                VxArgs.NonNegative(processedCount, nameof(processedCount));
#endif
                lock (_owner._syncRoot)
                {
                }
            }

            /// <inheritdoc/>
            public override void PartialFree(int newCount, ref ChannelReaderBufferSlice<T> slice)
            {
#if DEBUG
                VxArgs.NonNegative(newCount, nameof(newCount));
#endif
                lock (_owner._syncRoot)
                {
                    var sliceEntry = _owner._sliceEntriesRegistry[slice.Id];
                    int decreaseSize = sliceEntry.Length - newCount;

                    sliceEntry.Length = newCount;

                    // Trying to enlarge next item
                    var next = sliceEntry.Next;
                    if (!_owner.IsLastEntry(sliceEntry)
                        && (next.Status == SliceEntryStatus.Data) && _owner.IsConnectedWithNext(sliceEntry))
                    {
                        next.Start -= decreaseSize;
                        next.Length += decreaseSize;
                    }
                    else
                    {
                        var newEntry = _owner.InsertNewEntryAfter(sliceEntry);
                        newEntry.Status = SliceEntryStatus.Data;
                        newEntry.Length = decreaseSize;
                        newEntry.Start = sliceEntry.Start + newCount;
                    }
                }

                slice.DecreaseLength(newCount);
            }

            /// <inheritdoc/>
            public override ValueTask<T[]> ReadAsync(int count, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override bool TryRead(int count, out T[] items)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override bool TryRead(Span<T> outBuffer, out int readCount)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override bool TryRead(out T item)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override bool TryStartRead(int count, out ChannelReaderBufferSlice<T> slice)
            {
                lock (_owner._syncRoot)
                {
                    if (_owner._entriesListHead == null)
                    {
                        slice = default;
                        return false;
                    }

                    SliceEntry curEntry = _owner._entriesListHead;
                    do
                    {
                        if (curEntry.Status == SliceEntryStatus.Data)
                        {
                            curEntry.Status = SliceEntryStatus.AllocatedForRead;

                            if (curEntry.Length > count)
                            {
                                var newEntry = _owner.InsertNewEntryAfter(curEntry);
                                newEntry.Status = SliceEntryStatus.Data;
                                newEntry.Start = curEntry.Start + count;
                                newEntry.Length -= count;
                                curEntry.Length = count;
                            }

                            slice = new ChannelReaderBufferSlice<T>(
                                _owner._buffer,
                                curEntry.Start,
                                curEntry.Length,
                                curEntry.Id);
                            return true;
                        }

                        curEntry = curEntry.Next;
                    }
                    while (!ReferenceEquals(curEntry, _owner._entriesListHead));
                }

                slice = default;
                return false;
            }

            /// <inheritdoc/>
            public override Task<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override ValueTask<bool> WaitToReadValueTaskAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif