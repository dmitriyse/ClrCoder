// <copyright file="BatchModeBuffer.BatchModeBufferWriter.cs" company="ClrCoder project">
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
    using System.Threading.Channels;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <content>The <see cref="BatchModeBufferWriter{T}"/> implementation.</content>
    public partial class BatchModeBuffer<T>
    {
        [NoReorder]
        private class BatchModeBufferWriter<TSyncObject> : IChannelWriter<T>
            where TSyncObject : struct, ISyncObject
        {
            private readonly BatchModeBuffer<T> _owner;

            private readonly TSyncObject _syncObject;

            private bool _writeInProgress;

            public BatchModeBufferWriter(BatchModeBuffer<T> owner, TSyncObject syncObject)
            {
                _owner = owner;
                _syncObject = syncObject;
            }

            /// <summary>
            /// </summary>
            ~BatchModeBufferWriter()
            {
                Debug.Assert(false, "Buffer writer should be disposed manually.");
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                // Do nothing.
                GC.SuppressFinalize(this);
            }

            /// <inheritdoc/>
            public void Complete(Exception error = null)
            {
                using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                {
                    if (!guard.TryComplete(error))
                    {
                        throw new ChannelClosedException();
                    }
                }
            }

            /// <inheritdoc/>
            public bool TryComplete(Exception error = null)
            {
                using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                {
                    return guard.TryComplete();
                }
            }

            /// <inheritdoc/>
            public async ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default)
            {
                Task<VoidResult> taskToAwait = null;
                using (var seqGuard = new SequentialUsageGuard(this))
                {
                    using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                    {
                        seqGuard.OnStateGuardEntered();

                        if (guard.GetFreeSpaceForWrite() == 0)
                        {
                            taskToAwait = _owner._writeSpaceAvailableCs?.Task;
                        }
                    }

                    if (taskToAwait != null)
                    {
                        await taskToAwait.WithCancellation(cancellationToken);
                    }

                    // it's safe to use shadow.
                    return !_owner._isCompleted;
                }
            }

            /// <inheritdoc/>
            public bool TryWrite(T item)
            {
                using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                {
                    EnsureSequentialUsage();

                    bool result = false;
                    if (guard.GetFreeSpaceForWrite() > 0)
                    {
                        guard.NotifyWritten(1);

                        var sliceEntry = _owner._sliceEntries.Last?.Value;

                        if ((sliceEntry == null) || (sliceEntry.Status != SliceEntryStatus.Data)
                                                 || !sliceEntry.TryWriteLast(item))
                        {
                            _owner.AllocateNewEntry().TryWriteLast(item);
                        }

                        result = true;
                    }

                    return result;
                }
            }

            /// <inheritdoc/>
            public async ValueTask WriteAsync(T item, CancellationToken cancellationToken = default)
            {
                using (var seqGuard = new SequentialUsageGuard(this))
                {
                    while (true)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        Task<VoidResult> taskToAwait;

                        using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                        {
                            if (_owner._isCompleted)
                            {
                                throw new ChannelClosedException();
                            }

                            seqGuard.OnStateGuardEntered();

                            if (guard.GetFreeSpaceForWrite() > 0)
                            {
                                guard.NotifyWritten(1);

                                var sliceEntry = _owner._sliceEntries.Last?.Value;

                                if ((sliceEntry == null) || (sliceEntry.Status != SliceEntryStatus.Data)
                                                         || !sliceEntry.TryWriteLast(item))
                                {
                                    _owner.AllocateNewEntry().TryWriteLast(item);
                                }

                                return;
                            }

                            Debug.Assert(_owner._writeSpaceAvailableCs != null);
                            taskToAwait = _owner._writeSpaceAvailableCs.Task;
                        }

                        await taskToAwait.WithCancellation(cancellationToken);

                        // TODO: put this to avoid possible recursion.
                        // await Task.Yield();
                    }
                }
            }

            /// <inheritdoc/>
            public ChannelWriterBufferSlice<T> TryStartWrite(int count)
            {
                if (count <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Count should be non-zero positive value.");
                }

                using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                {
                    EnsureSequentialUsage();

                    if (_owner._isCompleted)
                    {
                        throw new ChannelClosedException();
                    }

                    int freeSpace = guard.GetFreeSpaceForWrite();
                    if (freeSpace != 0)
                    {
                        int amountToAllocate = Math.Min(Math.Min(count, freeSpace), _owner._maxSliceLength);

                        guard.NotifyAllocated(amountToAllocate);

                        var sliceEntry = _owner.AllocateNewEntry();
                        sliceEntry.Status = SliceEntryStatus.AllocatedForWrite;
                        sliceEntry.Length = amountToAllocate;
                        _writeInProgress = true;

                        return new ChannelWriterBufferSlice<T>(sliceEntry.Buffer, 0, amountToAllocate, sliceEntry.Id);
                    }

                    return new ChannelWriterBufferSlice<T>(EmptyBuffer, 0, 0, 0);
                }
            }

            /// <inheritdoc/>
            public async ValueTask<ChannelWriterBufferSlice<T>> StartWriteAsync(
                int count,
                CancellationToken cancellationToken)
            {
                if (count <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Count should be non-zero positive value.");
                }

                bool isWriteStarted = false;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Task<VoidResult> taskToAwait;

                    using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                    {
                        if (!isWriteStarted)
                        {
                            EnsureSequentialUsage();
                            _writeInProgress = true;
                            isWriteStarted = true;
                        }

                        if (_owner._isCompleted)
                        {
                            _writeInProgress = false;
                            throw new ChannelClosedException();
                        }

                        int freeSpace = guard.GetFreeSpaceForWrite();
                        if (freeSpace != 0)
                        {
                            int amountToAllocate = Math.Min(Math.Min(count, freeSpace), _owner._maxSliceLength);

                            guard.NotifyAllocated(amountToAllocate);

                            var sliceEntry = _owner.AllocateNewEntry();
                            sliceEntry.Status = SliceEntryStatus.AllocatedForWrite;
                            sliceEntry.Length = amountToAllocate;

                            var slice = new ChannelWriterBufferSlice<T>(
                                sliceEntry.Buffer,
                                0,
                                amountToAllocate,
                                sliceEntry.Id);
                            return slice;
                        }

                        Debug.Assert(_owner._writeSpaceAvailableCs != null, "_owner._writeSpaceAvailableCs != null");
                        taskToAwait = _owner._writeSpaceAvailableCs.Task;
                    }

                    await taskToAwait.WithCancellation(cancellationToken);

                    // TODO: put this to avoid possible recursion.
                    // await Task.Yield();
                }
            }

            /// <inheritdoc/>
            public void PartialFree(int newCount, ref ChannelWriterBufferSlice<T> writeSlice)
            {
                if ((newCount < 1) || (newCount > writeSlice.Length))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(newCount),
                        "New count should be greater than zero and less or equal to already allocated length.");
                }

                using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                {
                    if (!_owner._idToSliceEntries.TryGetValue(writeSlice.Id, out var sliceEntryNode))
                    {
                        throw new ArgumentException("Wrong slice variable.");
                    }

                    if (!_writeInProgress)
                    {
                        throw new InvalidOperationException("PartialFree operation has not been started.");
                    }

                    if (writeSlice.Length != sliceEntryNode.Value.Length)
                    {
                        throw new ArgumentException("The provided slice have an invalid state", nameof(writeSlice));
                    }

                    int decreaseSize = sliceEntryNode.Value.Length - newCount;

                    if (decreaseSize == 0)
                    {
                        // shit in shit out
                        return;
                    }

                    writeSlice.DecreaseLength(decreaseSize);

                    sliceEntryNode.Value.Length = newCount;

                    guard.NotifyWritten(0, decreaseSize);
                }
            }

            /// <inheritdoc/>
            public void CompleteWrite(int processedCount, ref ChannelWriterBufferSlice<T> writeSlice)
            {
                if ((processedCount < 0) || (processedCount > writeSlice.Length))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(processedCount),
                        "processedCount should be non-negative and less than allocated count");
                }

                using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                {
                    if (!_owner._idToSliceEntries.TryGetValue(writeSlice.Id, out var sliceEntryNode))
                    {
                        throw new ArgumentException("Wrong slice variable.");
                    }

                    if (!_writeInProgress)
                    {
                        throw new InvalidOperationException("CompleteWrite operation has not been started.");
                    }

                    var sliceEntry = sliceEntryNode.Value;
                    if (writeSlice.Length != sliceEntry.Length)
                    {
                        throw new ArgumentException("The provided slice have an invalid state", nameof(writeSlice));
                    }

                    guard.NotifyWritten(processedCount, sliceEntry.Length);

                    sliceEntry.Status = SliceEntryStatus.Data;

                    if (processedCount == 0)
                    {
                        _owner.RemoveNode(sliceEntryNode);
                    }
                    else
                    {
                        sliceEntry.Length = processedCount;
                    }

                    _writeInProgress = false;
                }
            }

            /// <inheritdoc/>
            public int TryWrite(ReadOnlySpan<T> items)
            {
                using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                {
                    EnsureSequentialUsage();

                    if (_owner._isCompleted)
                    {
                        throw new ChannelClosedException();
                    }

                    // Shit in - shit out
                    if (items.Length == 0)
                    {
                        return 0;
                    }

                    int spaceLeft = guard.GetFreeSpaceForWrite();

                    if (spaceLeft == 0)
                    {
                        return 0;
                    }

                    int amountToWrite = Math.Min(spaceLeft, items.Length);
                    int writtenCount = amountToWrite;

                    guard.NotifyWritten(amountToWrite);

                    int sourcePosition = 0;

                    // Perform copying.
                    while (amountToWrite > 0)
                    {
                        var target = _owner.TryAllocateUnsafe(amountToWrite);
                        amountToWrite -= target.Length;
                        for (int i = 0; i < target.Length; i++)
                        {
                            target[i] = items[sourcePosition++];
                        }
                    }

                    return writtenCount;
                }
            }

            /// <inheritdoc/>
            public bool TryWriteWithOwnership(ArraySegment<T> items)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public async ValueTask<int> WriteAsync(ArraySegment<T> items, CancellationToken cancellationToken = default)
            {
                if (items.Count == 0)
                {
                    return 0;
                }

                using (var seqGuard = new SequentialUsageGuard(this))
                {
                    while (true)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        Task<VoidResult> taskToAwait;

                        using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                        {
                            if (_owner._isCompleted)
                            {
                                throw new ChannelClosedException();
                            }

                            seqGuard.OnStateGuardEntered();

                            int spaceLeft = guard.GetFreeSpaceForWrite();
                            if (spaceLeft > 0)
                            {
                                int amountToWrite = Math.Min(spaceLeft, items.Count);
                                guard.NotifyWritten(amountToWrite);
                                int writtenCount = amountToWrite;

                                int sourcePosition = items.Offset;

                                // Perform copying.
                                while (amountToWrite > 0)
                                {
                                    void DoWrite()
                                    {
                                        var target = _owner.TryAllocateUnsafe(amountToWrite);
                                        amountToWrite -= target.Length;
                                        for (int i = 0; i < target.Length; i++)
                                        {
                                            target[i] = items.Array[sourcePosition++];
                                        }
                                    }
                                    DoWrite();
                                }

                                return writtenCount;
                            }

                            Debug.Assert(_owner._writeSpaceAvailableCs != null);
                            taskToAwait = _owner._writeSpaceAvailableCs.Task;
                        }

                        await taskToAwait.WithCancellation(cancellationToken);

                        // TODO: put this to avoid possible recursion.
                        // await Task.Yield();
                    }
                }
            }

            /// <inheritdoc/>
            public async ValueTask<int> WriteAsync<TItems>(TItems items, CancellationToken cancellationToken = default)
                where TItems : IEnumerable<T>
            {
                using (var enumerator = items.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                    {
                        // No items
                        return 0;
                    }

                    using (var seqGuard = new SequentialUsageGuard(this))
                    {
                        while (true)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            Task<VoidResult> taskToAwait;

                            using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                            {
                                if (_owner._isCompleted)
                                {
                                    throw new ChannelClosedException();
                                }

                                seqGuard.OnStateGuardEntered();

                                if (guard.GetFreeSpaceForWrite() > 0)
                                {
                                    int writtenCount = 0;
                                    while (_owner._allocatedCount < _owner._maxBufferLength)
                                    {
                                        // ReSharper disable once AccessToDisposedClosure
                                        T item = enumerator.Current;

                                        guard.NotifyWritten(1);

                                        writtenCount++;

                                        var sliceEntry = _owner._sliceEntries.Last?.Value;

                                        if ((sliceEntry == null)
                                            || (sliceEntry.Status != SliceEntryStatus.Data)
                                            || !sliceEntry.TryWriteLast(item))
                                        {
                                            _owner.AllocateNewEntry().TryWriteLast(item);
                                        }

                                        // ReSharper disable once AccessToDisposedClosure
                                        if (!enumerator.MoveNext())
                                        {
                                            return writtenCount;
                                        }
                                    }
                                }

                                Debug.Assert(_owner._writeSpaceAvailableCs != null);
                                taskToAwait = _owner._writeSpaceAvailableCs.Task;
                            }

                            await taskToAwait.WithCancellation(cancellationToken);

                            // TODO: put this to avoid possible recursion.
                            // await Task.Yield();
                        }
                    }
                }
            }

            public int TryWrite<TItems>(TItems items)
                where TItems : IEnumerable<T>
            {
                using (var enumerator = items.GetEnumerator())
                {
                    using (var guard = new StateWriteGuard<TSyncObject>(_syncObject, _owner))
                    {
                        EnsureSequentialUsage();

                        if (_owner._isCompleted)
                        {
                            throw new ChannelClosedException();
                        }

                        if (!enumerator.MoveNext())
                        {
                            // No items
                            return 0;
                        }

                        if (guard.GetFreeSpaceForWrite() == 0)
                        {
                            return 0;
                        }

                        int writtenCount = 0;

                        while (_owner._allocatedCount < _owner._maxBufferLength)
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            T item = enumerator.Current;

                            guard.NotifyWritten(1);

                            writtenCount++;

                            var sliceEntry = _owner._sliceEntries.Last?.Value;

                            if ((sliceEntry == null)
                                || (sliceEntry.Status != SliceEntryStatus.Data)
                                || !sliceEntry.TryWriteLast(item))
                            {
                                _owner.AllocateNewEntry().TryWriteLast(item);
                            }

                            // ReSharper disable once AccessToDisposedClosure
                            if (!enumerator.MoveNext())
                            {
                                return writtenCount;
                            }
                        }

                        return writtenCount;
                    }
                }
            }

            /// <inheritdoc/>
            public ValueTask<bool> WriteWithOwnershipAsync(
                ArraySegment<T> items,
                CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            private void EnsureSequentialUsage()
            {
                if (_writeInProgress)
                {
                    throw new InvalidOperationException("Every writer should be used sequentially.");
                }
            }

            private struct SequentialUsageGuard : IDisposable
            {
                private readonly BatchModeBufferWriter<TSyncObject> _writer;

                private bool _writeStarted;

                public SequentialUsageGuard(BatchModeBufferWriter<TSyncObject> writer)
                {
                    _writer = writer;
                    _writeStarted = false;
                }

                public void OnStateGuardEntered()
                {
                    if (!_writeStarted)
                    {
                        _writer.EnsureSequentialUsage();
                        _writeStarted = true;
                        _writer._writeInProgress = true;
                    }
                }

                /// <inheritdoc/>
                public void Dispose()
                {
                    if (_writeStarted)
                    {
                        using (_writer._syncObject.Lock())
                        {
                            _writer._writeInProgress = false;
                        }
                    }
                }
            }
        }
    }
}
#endif