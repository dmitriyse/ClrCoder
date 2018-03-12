// <copyright file="BatchModeBuffer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// TODO: Implement and test me.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class BatchModeBuffer<T> : IDataFlowConsumer<T>, IDataFlowProducer<T>
    {
        private const int _initialSliceEntriesCount = 4;

        private readonly object _syncRoot = new object();

        private readonly T[] _buffer;

        private readonly int _maxBufferLength;

        private readonly int _minReadSize;

        private readonly TimeSpan _maxReadAccumulationDelay;

        [NotNull]
        [ItemNotNull]
        private SliceEntry[] _sliceEntriesRegistry;

        [NotNull]
        [ItemNotNull]
        private SliceEntry[] _sliceEntriesAllocationStack;

        private int _sliceEntriesAllocationStackPointer;

        [CanBeNull]
        private SliceEntry _entriesListHead;

        private int _allocatedSize;

        private int _freePosition;

        private int _bufferSizeMask;

        private int _allocLimitLeft;

        /// <summary>
        /// </summary>
        /// <param name="maxBufferLength"></param>
        /// <param name="minReadSize"></param>
        public BatchModeBuffer(int maxBufferLength, int minReadSize, TimeSpan maxReadAccumulationDelay)
        {
            _maxBufferLength = maxBufferLength;
            _minReadSize = minReadSize;
            _maxReadAccumulationDelay = maxReadAccumulationDelay;

            // Buffer size should be power of two.
            // Picking nearest greater number than maxBufferLength 
            int bufferSize = 1;
            while ((bufferSize <<= 1) < maxBufferLength)
            {
            }

            // Do not be greedy, memory is cheap.
            _buffer = new T[bufferSize * 2];
            _bufferSizeMask = _buffer.Length - 1;

            _allocLimitLeft = maxBufferLength;

            _sliceEntriesAllocationStack = new SliceEntry[_initialSliceEntriesCount];
            for (int i = 0; i < _initialSliceEntriesCount; i++)
            {
                _sliceEntriesRegistry[i] = _sliceEntriesAllocationStack[i] = new SliceEntry
                                                                                 {
                                                                                     Id = i
                                                                                 };
            }
        }

        private enum SliceEntryStatus
        {
            AllocatedForWrite,

            Data,

            AllocatedForRead
        }

        /// <inheritdoc/>
        public IChannelReader<T> OpenReader()
        {
            return new ModeBufferReaderEx(this);
        }

        /// <inheritdoc/>
        public IChannelWriter<T> OpenWriter()
        {
            return new BatchModeBufferWriter(this);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void IncreaseSliceEntriesPoolSize()
        {
            int oldSize = _sliceEntriesAllocationStack.Length;
            int newSize = oldSize << 1;

            // Increasing pool size.
            Array.Resize(ref _sliceEntriesRegistry, newSize);
            Array.Resize(ref _sliceEntriesAllocationStack, newSize);

            // Writing new slice entries indexes.
            for (int i = oldSize; i < newSize; i++)
            {
                _sliceEntriesAllocationStack[i] = _sliceEntriesRegistry[i] = new SliceEntry
                                                                                 {
                                                                                     Id = i
                                                                                 };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SliceEntry InsertNewEntryAfter(SliceEntry entry)
        {
            SliceEntry newEntry = _sliceEntriesAllocationStack[_sliceEntriesAllocationStackPointer++];
            if (_sliceEntriesAllocationStackPointer == _sliceEntriesAllocationStack.Length)
            {
                IncreaseSliceEntriesPoolSize();
            }

            SliceEntry next = newEntry.Next = entry.Next;
            newEntry.Prev = entry;
            entry.Next = next.Prev = newEntry;

            return newEntry;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SliceEntry InsertNewEntryLast()
        {
            SliceEntry newEntry = _sliceEntriesAllocationStack[_sliceEntriesAllocationStackPointer++];
            if (_sliceEntriesAllocationStackPointer == _sliceEntriesAllocationStack.Length)
            {
                IncreaseSliceEntriesPoolSize();
            }

            if (_entriesListHead == null)
            {
                _entriesListHead = newEntry;
                _entriesListHead.Prev = _entriesListHead;
                _entriesListHead.Next = _entriesListHead;
            }
            else
            {
                SliceEntry prev = newEntry.Prev = _entriesListHead.Prev;
                newEntry.Next = _entriesListHead;
                _entriesListHead.Prev = prev.Next = newEntry;
            }

            return newEntry;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsConnectedWithNext(SliceEntry entry)
        {
            return entry.Start + entry.Length == entry.Next.Start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsFirstEntry(SliceEntry entry)
        {
            return ReferenceEquals(entry.Prev, _entriesListHead);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsLastEntry(SliceEntry entry)
        {
            return ReferenceEquals(entry.Next, _entriesListHead);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SliceEntry JoinToNextEntry(SliceEntry entry)
        {
            SliceEntry next = entry.Next;
            int allocationLength = entry.Length;
            next.Start -= allocationLength;
            next.Length += allocationLength;

            // Remove op.
            next.Prev = entry.Prev;
            entry.Prev.Next = next;
            _sliceEntriesAllocationStack[--_sliceEntriesAllocationStackPointer] = entry;
            if (ReferenceEquals(entry, _entriesListHead))
            {
                _entriesListHead = next;
            }

            return next;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void JoinToPrevEntry(SliceEntry entry)
        {
            SliceEntry prev = entry.Prev;

            prev.Length += entry.Length;

            // Remove op.
            prev.Next = entry.Next;
            entry.Next.Prev = prev;
            _sliceEntriesAllocationStack[--_sliceEntriesAllocationStackPointer] = entry;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveEntry(SliceEntry entry)
        {
            Debug.Assert(_entriesListHead != null, "_entriesListHead != null");

            // ReSharper disable once PossibleNullReferenceException
            if (ReferenceEquals(_entriesListHead.Next, _entriesListHead))
            {
                Debug.Assert(ReferenceEquals(entry, _entriesListHead), "ReferenceEquals(entry, _entriesListHead)");

                _entriesListHead = null;
                _sliceEntriesAllocationStack[--_sliceEntriesAllocationStackPointer] = _entriesListHead;
            }
            else
            {
                SliceEntry next = entry.Next;
                next.Prev = entry.Prev;
                entry.Prev.Next = next;
                _sliceEntriesAllocationStack[--_sliceEntriesAllocationStackPointer] = entry;
                if (ReferenceEquals(_entriesListHead, entry))
                {
                    _entriesListHead = next;
                }
            }
        }

        private class SliceEntry
        {
            public int Id;

            public SliceEntryStatus Status;

            public int Start;

            public int Length;

            public SliceEntry Next;

            public SliceEntry Prev;
        }
    }
}

#endif