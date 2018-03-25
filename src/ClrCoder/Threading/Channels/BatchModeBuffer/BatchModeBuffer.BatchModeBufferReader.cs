// <copyright file="BatchModeBuffer.BatchModeBufferReader.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <content>The <see cref="BatchModeBufferReader{T}"/> implementation.</content>
    public partial class BatchModeBuffer<T>
    {
        [NoReorder]
        private class BatchModeBufferReader<TSyncObject> : IChannelReader<T>
            where TSyncObject : struct, ISyncObject
        {
            private readonly BatchModeBuffer<T> _owner;

            private readonly TSyncObject _syncObject;

            private bool _readInProgress;

            public BatchModeBufferReader(BatchModeBuffer<T> owner, TSyncObject syncObject)
            {
                _owner = owner;
                _syncObject = syncObject;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// </summary>
            ~BatchModeBufferReader()
            {
                Debug.Assert(false, "Buffer reader should be disposed manually.");
            }

            /// <inheritdoc/>
            public ValueTask Completion => new ValueTask(_owner._readCompletedCs.Task);

            /// <inheritdoc/>
            public async ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
            {
                Task<VoidResult> taskToAwait = null;
                using (var seqGuard = new SequentialUsageGuard(this))
                {
                    using (var guard = new StateReadGuard<TSyncObject>(_syncObject, _owner))
                    {
                        seqGuard.OnStateGuardEntered();

                        if (guard.GetAvailableDataForReading() == 0)
                        {
                            taskToAwait = _owner._dataAvailableCs?.Task;
                        }
                    }

                    if (taskToAwait != null)
                    {
                        await taskToAwait;
                    }

                    // it's safe to use shadow.
                    return !_owner._isCompleted;
                }
            }

            /// <inheritdoc/>
            public bool TryRead(out T item)
            {
                using (var guard = new StateReadGuard<TSyncObject>())
                {
                    EnsureSequentialUsage();

                    if (guard.GetAvailableDataForReading() == 0)
                    {
                        item = default;
                        return false;
                    }

                    LinkedListNode<SliceEntry> curNode = _owner._sliceEntries.Last;

                    // Searching for node to read from
                    while (curNode != null)
                    {
                        var sliceEntry = curNode.Value;

                        if (sliceEntry.Status == SliceEntryStatus.Data)
                        {
                            Debug.Assert(sliceEntry.Length != 0, "sliceEntry.Length != 0");

                            guard.NotifyRead(1);

                            item = sliceEntry.Buffer[sliceEntry.Start++];

                            // Removing empty nodes.
                            if (--sliceEntry.Length == 0)
                            {
                                _owner._sliceEntries.Remove(curNode);
                            }

                            return true;
                        }

                        curNode = curNode.Next;
                    }

                    Debug.Assert(false, "Available data counter does not corresponds to slice entries.");
                    item = default;
                    return false;
                }
            }

            /// <inheritdoc/>
            public async ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
            {
                using (var seqGuard = new SequentialUsageGuard(this))
                {
                    while (true)
                    {
                        Task<VoidResult> taskToAwait;

                        using (var guard = new StateReadGuard<TSyncObject>())
                        {
                            if (guard.GetAvailableDataForReading() == 0)
                            {
                                _owner.VerifyChannelNotClosed();

                                seqGuard.OnStateGuardEntered();

                                Debug.Assert(_owner._dataAvailableCs != null, "_owner._dataAvailableCs != null");

                                taskToAwait = _owner._dataAvailableCs.Task;
                            }
                            else
                            {
                                seqGuard.OnStateGuardEntered();

                                LinkedListNode<SliceEntry> curNode = _owner._sliceEntries.Last;

                                // Searching for node to read from
                                while (curNode != null)
                                {
                                    var sliceEntry = curNode.Value;

                                    if (sliceEntry.Status == SliceEntryStatus.Data)
                                    {
                                        Debug.Assert(sliceEntry.Length != 0, "sliceEntry.Length != 0");

                                        guard.NotifyRead(1);

                                        var item = sliceEntry.Buffer[sliceEntry.Start++];

                                        // Removing empty nodes.
                                        if (--sliceEntry.Length == 0)
                                        {
                                            _owner._sliceEntries.Remove(curNode);
                                        }

                                        return item;
                                    }

                                    curNode = curNode.Next;
                                }

                                Debug.Assert(false, "Available data counter does not corresponds to slice entries.");

                                throw new InvalidOperationException("Impossible state!");
                            }
                        }

                        await taskToAwait.WithCancellation(cancellationToken);
                    }
                }
            }

            /// <inheritdoc/>
            public ChannelReaderBufferSlice<T> TryStartRead(int count)
            {
                if (count <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Count should be non-zero positive.");
                }

                using (var guard = new StateReadGuard<TSyncObject>())
                {
                    EnsureSequentialUsage();

                    int nonAllocatedLength = guard.GetAvailableDataForReading();
                    if (nonAllocatedLength == 0)
                    {
                        return new ChannelReaderBufferSlice<T>(EmptyBuffer, 0, 0, 0);
                    }

                    LinkedListNode<SliceEntry> curNode = _owner._sliceEntries.Last;

                    // Searching for node to read from
                    while (curNode != null)
                    {
                        var sliceEntry = curNode.Value;

                        if (sliceEntry.Status == SliceEntryStatus.Data)
                        {
                            Debug.Assert(sliceEntry.Length != 0, "sliceEntry.Length != 0");

                            int allocateForReadLength = Math.Min(count, sliceEntry.Length);

                            guard.NotifyAllocatedForReading(allocateForReadLength);

                            if (allocateForReadLength == sliceEntry.Length)
                            {
                                sliceEntry.Status = SliceEntryStatus.AllocatedForRead;
                                return new ChannelReaderBufferSlice<T>(
                                    sliceEntry.Buffer,
                                    sliceEntry.Start,
                                    sliceEntry.Length,
                                    sliceEntry.Id);
                            }

                            int freeSpaceLeft = sliceEntry.Length - allocateForReadLength;
                            sliceEntry.Length -= freeSpaceLeft;
                            sliceEntry.Status = SliceEntryStatus.AllocatedForRead;

                            var newEntry = _owner.AllocateEmptyEntryAfter(curNode);
                            newEntry.Buffer = sliceEntry.Buffer;
                            newEntry.Start = sliceEntry.Start + allocateForReadLength;
                            newEntry.Length = freeSpaceLeft;
                            newEntry.Status = SliceEntryStatus.Data;

                            return new ChannelReaderBufferSlice<T>(
                                sliceEntry.Buffer,
                                sliceEntry.Start,
                                sliceEntry.Length,
                                sliceEntry.Id);
                        }

                        curNode = curNode.Next;
                    }

                    Debug.Assert(false, "Available data counter does not corresponds to slice entries.");

                    throw new InvalidOperationException("Impossible state!");
                }
            }

            /// <inheritdoc/>
            public async ValueTask<ChannelReaderBufferSlice<T>> StartReadAsync(
                int count,
                CancellationToken cancellationToken = default)
            {
                if (count <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Count should be non-zero positive.");
                }

                bool isReadStarted = false;
                while (true)
                {
                    Task<VoidResult> taskToAwait;

                    using (var guard = new StateReadGuard<TSyncObject>())
                    {
                        if (!isReadStarted)
                        {
                            EnsureSequentialUsage();
                            _readInProgress = true;
                            isReadStarted = true;
                        }

                        int nonAllocatedLength = guard.GetAvailableDataForReading();
                        if (nonAllocatedLength == 0)
                        {
                            try
                            {
                                _owner.VerifyChannelNotClosed();
                            }
                            catch
                            {
                                _readInProgress = false;
                                throw;
                            }

                            Debug.Assert(_owner._dataAvailableCs != null, "_owner._dataAvailableCs != null");

                            taskToAwait = _owner._dataAvailableCs.Task;
                        }
                        else
                        {
                            LinkedListNode<SliceEntry> curNode = _owner._sliceEntries.Last;

                            // Searching for node to read from
                            while (curNode != null)
                            {
                                var sliceEntry = curNode.Value;

                                if (sliceEntry.Status == SliceEntryStatus.Data)
                                {
                                    Debug.Assert(sliceEntry.Length != 0, "sliceEntry.Length != 0");

                                    int allocateForReadLength = Math.Min(count, sliceEntry.Length);

                                    guard.NotifyAllocatedForReading(allocateForReadLength);

                                    if (allocateForReadLength == sliceEntry.Length)
                                    {
                                        sliceEntry.Status = SliceEntryStatus.AllocatedForRead;
                                        return new ChannelReaderBufferSlice<T>(
                                            sliceEntry.Buffer,
                                            sliceEntry.Start,
                                            sliceEntry.Length,
                                            sliceEntry.Id);
                                    }

                                    int freeSpaceLeft = sliceEntry.Length - allocateForReadLength;
                                    sliceEntry.Length -= freeSpaceLeft;
                                    sliceEntry.Status = SliceEntryStatus.AllocatedForRead;

                                    var newEntry = _owner.AllocateEmptyEntryAfter(curNode);
                                    newEntry.Buffer = sliceEntry.Buffer;
                                    newEntry.Start = sliceEntry.Start + allocateForReadLength;
                                    newEntry.Length = freeSpaceLeft;
                                    newEntry.Status = SliceEntryStatus.Data;

                                    return new ChannelReaderBufferSlice<T>(
                                        sliceEntry.Buffer,
                                        sliceEntry.Start,
                                        sliceEntry.Length,
                                        sliceEntry.Id);
                                }

                                curNode = curNode.Next;
                            }

                            Debug.Assert(false, "Available data counter does not corresponds to slice entries.");

                            throw new InvalidOperationException("Impossible state!");
                        }
                    }

                    await taskToAwait.WithCancellation(cancellationToken);
                }
            }

            /// <inheritdoc/>
            public void PartialFree(int newCount, ref ChannelReaderBufferSlice<T> slice)
            {
                if ((newCount < 1) || (newCount > slice.Length))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(newCount),
                        "New count should be greater than zero and less or equal to already allocated length.");
                }

                using (var guard = new StateReadGuard<TSyncObject>())
                {
                    if (!_owner._idToSliceEntries.TryGetValue(slice.Id, out var sliceEntryNode))
                    {
                        throw new ArgumentException("Wrong slice variable.");
                    }

                    if (!_readInProgress)
                    {
                        throw new InvalidOperationException("PartialFree operation has not been started.");
                    }

                    if (slice.Length != sliceEntryNode.Value.Length)
                    {
                        throw new ArgumentException("The provided slice have an invalid state", nameof(slice));
                    }

                    int decreaseSize = sliceEntryNode.Value.Length - newCount;

                    if (decreaseSize == 0)
                    {
                        // Shit it - shit out
                        return;
                    }

                    slice.DecreaseLength(decreaseSize);

                    var sliceEntry = sliceEntryNode.Value;
                    sliceEntry.Length = newCount;

                    // Splitting released part to the new entry.
                    var newEntry = _owner.AllocateEmptyEntryAfter(sliceEntryNode);
                    newEntry.Buffer = sliceEntry.Buffer;
                    newEntry.Start = sliceEntry.Start + sliceEntry.Length;
                    newEntry.Length = decreaseSize;
                    newEntry.Status = SliceEntryStatus.Data;

                    guard.NotifyRead(0, decreaseSize);
                }
            }

            /// <inheritdoc/>
            public void CompleteRead(int processedCount, ref ChannelReaderBufferSlice<T> slice)
            {
                if ((processedCount < 0) || (processedCount > slice.Length))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(processedCount),
                        "processedCount should be non-negative and less than allocated count");
                }

                using (var guard = new StateReadGuard<TSyncObject>(_syncObject, _owner))
                {
                    if (!_owner._idToSliceEntries.TryGetValue(slice.Id, out var sliceEntryNode))
                    {
                        throw new ArgumentException("Wrong slice variable.");
                    }

                    if (!_readInProgress)
                    {
                        throw new InvalidOperationException("CompleteWrite operation has not been started.");
                    }

                    var sliceEntry = sliceEntryNode.Value;

                    if (slice.Length != sliceEntry.Length)
                    {
                        throw new ArgumentException("The provided slice have an invalid state", nameof(slice));
                    }

                    guard.NotifyRead(processedCount, sliceEntry.Length);

                    int nonProcessedCount = sliceEntry.Length - processedCount;

                    if (processedCount == 0)
                    {
                        sliceEntry.Status = SliceEntryStatus.Data;
                    }
                    else if (nonProcessedCount == 0)
                    {
                        _owner._sliceEntries.Remove(sliceEntryNode);
                    }
                    else
                    {
                        sliceEntry.Length = nonProcessedCount;
                        sliceEntry.Start += processedCount;
                        sliceEntry.Status = SliceEntryStatus.Data;
                    }

                    _readInProgress = false;
                }
            }

            /// <inheritdoc/>
            public int TryRead(Span<T> outBuffer)
            {
                using (var guard = new StateReadGuard<TSyncObject>())
                {
                    EnsureSequentialUsage();

                    if (outBuffer.Length == 0)
                    {
                        return 0;
                    }

                    int availableDataCount = guard.GetAvailableDataForReading();
                    if (availableDataCount == 0)
                    {
                        return 0;
                    }

                    int amountToRead = Math.Min(availableDataCount, outBuffer.Length);
                    guard.NotifyRead(amountToRead);

                    int readCount = amountToRead;
                    int outPosition = 0;

                    // Searching for node to read from
                    LinkedListNode<SliceEntry> curNode = _owner._sliceEntries.Last;
                    while ((curNode != null) && (amountToRead != 0))
                    {
                        var sliceEntry = curNode.Value;

                        if (sliceEntry.Status == SliceEntryStatus.Data)
                        {
                            Debug.Assert(sliceEntry.Length != 0, "sliceEntry.Length != 0");

                            int readChunkLength = Math.Min(amountToRead, curNode.Value.Length);

                            for (int i = 0; i < readChunkLength; i++)
                            {
                                outBuffer[outPosition++] = sliceEntry.Buffer[i + sliceEntry.Start];
                            }

                            amountToRead -= readChunkLength;

                            if (readChunkLength == curNode.Value.Length)
                            {
                                var nodeToRemove = curNode;

                                curNode = curNode.Next;

                                _owner._sliceEntries.Remove(nodeToRemove);
                                continue;
                            }

                            curNode.Value.Start += readChunkLength;
                            curNode.Value.Length -= readChunkLength;
                        }

                        curNode = curNode.Next;
                    }

                    return readCount;
                }
            }

            /// <inheritdoc/>
            public bool TryRead(int count, out ArraySegment<T> items)
            {
                if (count <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Count should be non-zero positive.");
                }

                using (var guard = new StateReadGuard<TSyncObject>(_syncObject, _owner))
                {
                    EnsureSequentialUsage();

                    int dataAvailable = guard.GetAvailableDataForReading();

                    if (dataAvailable == 0)
                    {
                        items = default;
                        return false;
                    }

                    LinkedListNode<SliceEntry> curNode = _owner._sliceEntries.Last;
                    while (curNode != null)
                    {
                        var sliceEntry = curNode.Value;

                        if (sliceEntry.Status == SliceEntryStatus.Data)
                        {
                            Debug.Assert(sliceEntry.Length != 0, "sliceEntry.Length != 0");

                            int amountToRead = Math.Min(sliceEntry.Length, count);

                            guard.NotifyRead(amountToRead);

                            items = new ArraySegment<T>(sliceEntry.Buffer, sliceEntry.Start, amountToRead);

                            if (amountToRead == sliceEntry.Length)
                            {
                                _owner._sliceEntries.Remove(curNode);
                            }
                            else
                            {
                                sliceEntry.Start += amountToRead;
                                sliceEntry.Length -= amountToRead;
                            }

                            return true;
                        }

                        curNode = curNode.Next;
                    }

                    Debug.Assert(false, "Counters are not corresponding to the slice entries list.");
                    throw new InvalidOperationException("Impossible state!");
                }
            }

            /// <inheritdoc/>
            public async ValueTask<ArraySegment<T>> ReadAsync(int count, CancellationToken cancellationToken)
            {
                if (count <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Count should be non-zero positive.");
                }

                using (var seqGuard = new SequentialUsageGuard(this))
                {
                    while (true)
                    {
                        Task<VoidResult> taskToAwait;
                        using (var guard = new StateReadGuard<TSyncObject>(_syncObject, _owner))
                        {
                            int dataAvailable = guard.GetAvailableDataForReading();

                            if (dataAvailable == 0)
                            {
                                _owner.VerifyChannelNotClosed();

                                seqGuard.OnStateGuardEntered();

                                Debug.Assert(_owner._dataAvailableCs != null, "_owner._dataAvailableCs != null");

                                taskToAwait = _owner._dataAvailableCs.Task;
                            }
                            else
                            {
                                seqGuard.OnStateGuardEntered();

                                LinkedListNode<SliceEntry> curNode = _owner._sliceEntries.Last;
                                while (curNode != null)
                                {
                                    var sliceEntry = curNode.Value;

                                    if (sliceEntry.Status == SliceEntryStatus.Data)
                                    {
                                        Debug.Assert(sliceEntry.Length != 0, "sliceEntry.Length != 0");

                                        int amountToRead = Math.Min(sliceEntry.Length, count);

                                        guard.NotifyRead(amountToRead);

                                        var items = new ArraySegment<T>(
                                            sliceEntry.Buffer,
                                            sliceEntry.Start,
                                            amountToRead);

                                        if (amountToRead == sliceEntry.Length)
                                        {
                                            _owner._sliceEntries.Remove(curNode);
                                        }
                                        else
                                        {
                                            sliceEntry.Start += amountToRead;
                                            sliceEntry.Length -= amountToRead;
                                        }

                                        return items;
                                    }

                                    curNode = curNode.Next;
                                }

                                Debug.Assert(false, "Counters are not corresponding to the slice entries list.");
                                throw new InvalidOperationException("Impossible state!");
                            }
                        }

                        await taskToAwait.WithCancellation(cancellationToken);
                    }
                }
            }

            private void EnsureSequentialUsage()
            {
                if (_readInProgress)
                {
                    throw new InvalidOperationException("Every reader should be used sequentially.");
                }
            }

            private struct SequentialUsageGuard : IDisposable
            {
                private readonly BatchModeBufferReader<TSyncObject> _reader;

                private bool _readStarted;

                public SequentialUsageGuard(BatchModeBufferReader<TSyncObject> reader)
                {
                    _reader = reader;
                    _readStarted = false;
                }

                public void OnStateGuardEntered()
                {
                    if (!_readStarted)
                    {
                        _reader.EnsureSequentialUsage();
                        _readStarted = true;
                        _reader._readInProgress = true;
                    }
                }

                /// <inheritdoc/>
                public void Dispose()
                {
                    if (_readStarted)
                    {
                        using (_reader._syncObject.Lock())
                        {
                            _reader._readInProgress = false;
                        }
                    }
                }
            }
        }
    }
}
#endif