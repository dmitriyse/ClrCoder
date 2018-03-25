// <copyright file="BatchModeBuffer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// TODO: Implement and test me.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class BatchModeBuffer<T> : IDataFlowConsumer<T>, IDataFlowProducer<T>
    {
        private static readonly T[] EmptyBuffer = new T[0];

        [CanBeNull]
        private readonly object _syncRoot;

        private readonly int _maxBufferLength;

        private readonly int _minReadSize;

        private readonly TimeSpan _maxReadAccumulationDelay;

        private readonly Dictionary<int, LinkedListNode<SliceEntry>> _idToSliceEntries =
            new Dictionary<int, LinkedListNode<SliceEntry>>();

        private readonly LinkedList<SliceEntry> _sliceEntries = new LinkedList<SliceEntry>();

        private readonly int _maxSliceLength;

        private int _nextId = 0;

        /// <summary>
        /// </summary>
        /// <param name="maxBufferLength"></param>
        /// <param name="minReadSize"></param>
        public BatchModeBuffer(
            int maxBufferLength,
            int minReadSize,
            TimeSpan maxReadAccumulationDelay,
            object syncRoot = null)
        {
            _maxBufferLength = maxBufferLength;
            _minReadSize = minReadSize;
            _maxReadAccumulationDelay = maxReadAccumulationDelay;
            _syncRoot = syncRoot;
            _maxSliceLength = Math.Max(512, _minReadSize);
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IChannelWriter<T> OpenWriter()
        {
            if (_syncRoot == null)
            {
                return new BatchModeBufferWriter<SyncFreeObject>(this, default);
            }

            return new BatchModeBufferWriter<MonitorSyncObject>(this, new MonitorSyncObject(_syncRoot));
        }

        private SliceEntry AllocateEmptyEntryAfter(LinkedListNode<SliceEntry> node)
        {
            var newEntry = new SliceEntry
                               {
                                   Id = _nextId++
                               };
            _sliceEntries.AddAfter(node, newEntry);

            return newEntry;
        }

        private SliceEntry AllocateNewEntry()
        {
            var result = new SliceEntry
                             {
                                 Buffer = new T[_maxSliceLength],
                                 Status = SliceEntryStatus.Data,
                                 Id = _nextId++
                             };

            _idToSliceEntries.Add(result.Id, _sliceEntries.AddLast(result));

            return result;
        }

        private void RemoveNode(LinkedListNode<SliceEntry> sliceEntryNode)
        {
            _idToSliceEntries.Remove(sliceEntryNode.Value.Id);
            _sliceEntries.Remove(sliceEntryNode);
        }

        private Span<T> TryAllocateUnsafe(int amount)
        {
            // We don't check logical amount.
            SliceEntry lastEntry = _sliceEntries.Last?.Value;

            Span<T> result;

            if ((lastEntry != null)
                && (lastEntry.Status == SliceEntryStatus.Data))
            {
                result = lastEntry.AllocateForWrite(amount);
                if (result.Length != 0)
                {
                    return result;
                }
            }

            lastEntry = AllocateNewEntry();
            return lastEntry.AllocateForWrite(amount);
        }

        private class SliceEntry
        {
            public int Id;

            public T[] Buffer;

            public int Start;

            public int Length;

            public SliceEntryStatus Status;

            public int SpaceLeft => Buffer.Length - Start - Length;

            public Span<T> AllocateForWrite(int maxAmount)
            {
                int writePosition = Start + Length;
                int amount = Math.Min(Buffer.Length - writePosition, maxAmount);
                Length += amount;
                return new Span<T>(Buffer, writePosition, amount);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryWriteLast(T item)
            {
                bool result = false;
                int writePosition = Start + Length;
                if (writePosition < Buffer.Length)
                {
                    Buffer[writePosition] = item;
                    Length++;
                    result = true;
                }

                return result;
            }
        }
    }
}

#endif