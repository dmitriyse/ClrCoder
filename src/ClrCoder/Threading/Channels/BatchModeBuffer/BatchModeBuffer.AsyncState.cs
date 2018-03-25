// <copyright file="BatchModeBuffer.AsyncState.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    /// <content>The state and it's management methods.</content>
    public partial class BatchModeBuffer<T>
    {
        private readonly TaskCompletionSource<VoidResult> _readCompletedCs = new TaskCompletionSource<VoidResult>();

        private TaskCompletionSource<VoidResult> _writeSpaceAvailableCs;

        private TaskCompletionSource<VoidResult> _dataAvailableCs;

        private Exception _completionError;

        private int _allocatedCount;

        private bool _isCompleted;

        private int _availableDataCount;

        private int _nonAllocatedForReadDataCount;

        private void PostReadCompletion()
        {
            Debug.Assert(_isCompleted, "_isCompleted");
            if (_completionError == null)
            {
                _readCompletedCs.SetResult(default);
            }
            else
            {
                _readCompletedCs.SetException(_completionError);
            }
        }

        private void VerifyChannelNotClosed()
        {
            if (_isCompleted && (_availableDataCount == 0))
            {
                if (_completionError == null)
                {
                    throw new ChannelClosedException();
                }

                throw new ChannelClosedException(_completionError);
            }
        }

        [Conditional("DEBUG")]
        private void VerifyCounters()
        {
            Debug.Assert(_allocatedCount <= _maxBufferLength, "_allocatedCount <= _maxBufferLength");
            Debug.Assert(_availableDataCount <= _allocatedCount, "_availableDataCount <= _allocatedCount");
            Debug.Assert(
                _nonAllocatedForReadDataCount <= _availableDataCount,
                "_nonAllocatedForReadDataCount <= _availableDataCount");
            Debug.Assert(_nonAllocatedForReadDataCount >= 0, "_nonAllocatedForReadDataCount >= 0");
        }

        private struct StateReadGuard<TSyncObject> : IDisposable
            where TSyncObject : struct, ISyncObject
        {
            private readonly TSyncObject _syncObject;

            private readonly BatchModeBuffer<T> _owner;

            private TaskCompletionSource<VoidResult> _writeSpaceAvailableCsShadow;

            private TaskCompletionSource<VoidResult> _dataAvailableCsShadow;

            private bool _postCompletion;

            public StateReadGuard(TSyncObject syncObject, BatchModeBuffer<T> owner)
            {
                _syncObject = syncObject;
                _owner = owner;
                _dataAvailableCsShadow = null;
                _writeSpaceAvailableCsShadow = null;
                _postCompletion = false;

                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                _syncObject.Enter();
            }

            public int GetAvailableDataForReading()
            {
                if (_owner._nonAllocatedForReadDataCount != 0)
                {
                    if (!(_owner._isCompleted && (_owner._availableDataCount == 0)))
                    {
                        if (_owner._dataAvailableCs == null)
                        {
                            _owner._dataAvailableCs = new TaskCompletionSource<VoidResult>();
                        }
                    }
                }

                return _owner._nonAllocatedForReadDataCount;
            }

            public void NotifyAllocatedForReading(int count)
            {
                Debug.Assert(count > 0, "count > 0");
                Debug.Assert(
                    count <= _owner._nonAllocatedForReadDataCount,
                    "count <= _owner._nonAllocatedForReadDataCount");

                _owner._nonAllocatedForReadDataCount -= count;

                _owner.VerifyCounters();
            }

            public void NotifyRead(int count)
            {
                Debug.Assert(count > 0, "count > 0");
                Debug.Assert(count <= _owner._nonAllocatedForReadDataCount);

                _owner._allocatedCount -= count;
                _owner._availableDataCount -= count;
                _owner._nonAllocatedForReadDataCount -= count;

                _owner.VerifyCounters();

                _writeSpaceAvailableCsShadow = _owner._writeSpaceAvailableCs;
                _owner._writeSpaceAvailableCs = null;

                if ((_owner._availableDataCount == 0) && _owner._isCompleted)
                {
                    _postCompletion = true;
                }
            }

            public void NotifyRead(int readCount, int deallocatedCount)
            {
                Debug.Assert(readCount >= 0, "readCount >= 0");
                Debug.Assert(deallocatedCount > 0, "deallocatedCount > 0");
                Debug.Assert(readCount <= deallocatedCount, "readCount <= deallocatedCount");
                Debug.Assert(
                    deallocatedCount <= _owner._availableDataCount - _owner._nonAllocatedForReadDataCount,
                    "deallocatedCount <= _owner._availableDataCount - _owner._nonAllocatedForReadDataCount");

                _owner._nonAllocatedForReadDataCount += deallocatedCount - readCount;
                _owner._availableDataCount -= readCount;
                _owner._allocatedCount -= readCount;

                _owner.VerifyCounters();

                if (deallocatedCount - readCount > 0)
                {
                    _dataAvailableCsShadow = _owner._dataAvailableCs;
                    _owner._dataAvailableCs = null;
                }

                if (readCount > 0)
                {
                    _writeSpaceAvailableCsShadow = _owner._writeSpaceAvailableCs;
                    _owner._writeSpaceAvailableCs = null;
                }

                if ((_owner._availableDataCount == 0) && _owner._isCompleted)
                {
                    _postCompletion = true;
                }
            }

            // public void Notify
            public void Dispose()
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                _syncObject.Exit();

                _writeSpaceAvailableCsShadow?.SetResult(default);

                _dataAvailableCsShadow?.SetResult(default);

                if (_postCompletion)
                {
                    // it's safe to get completion error without a lock.
                    _owner.PostReadCompletion();
                }
            }
        }

        private struct StateWriteGuard<TSyncObject> : IDisposable
            where TSyncObject : struct, ISyncObject
        {
            private readonly TSyncObject _syncObject;

            private readonly BatchModeBuffer<T> _owner;

            private TaskCompletionSource<VoidResult> _writeSpaceAvailableCsShadow;

            private TaskCompletionSource<VoidResult> _dataAvailableCsShadow;

            private bool _postCompletion;

            public StateWriteGuard(TSyncObject syncObject, BatchModeBuffer<T> owner)
            {
                _syncObject = syncObject;
                _owner = owner;
                _dataAvailableCsShadow = null;
                _writeSpaceAvailableCsShadow = null;
                _postCompletion = false;

                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                _syncObject.Enter();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetFreeSpaceForWrite()
            {
                if (_owner._isCompleted)
                {
                    return 0;
                }

                int result = _owner._maxBufferLength - _owner._allocatedCount;
                if (result == 0)
                {
                    if (_owner._writeSpaceAvailableCs == null)
                    {
                        _owner._writeSpaceAvailableCs = new TaskCompletionSource<VoidResult>();
                    }
                }

                return result;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryComplete(Exception error = null)
            {
                bool result = false;
                if (!_owner._isCompleted)
                {
                    _owner._isCompleted = true;
                    _owner._completionError = error;

                    if (_owner._allocatedCount == 0)
                    {
                        _dataAvailableCsShadow = _owner._dataAvailableCs;
                        _owner._dataAvailableCs = null;

                        if (_owner._availableDataCount == 0)
                        {
                            _postCompletion = true;
                        }
                    }

                    result = true;
                }

                return result;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void NotifyAllocated(int count)
            {
                Debug.Assert(count > 0, "count > 0");
                Debug.Assert(
                    count <= _owner._maxBufferLength - _owner._allocatedCount,
                    "count <= _owner._maxBufferLength - _owner._allocatedCount");

                _owner._allocatedCount += count;

                _owner.VerifyCounters();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void NotifyWritten(int count)
            {
                Debug.Assert(count > 0, "count > 0");
                Debug.Assert(
                    count <= _owner._maxBufferLength - _owner._allocatedCount,
                    "count <= _owner._maxBufferLength - _owner._allocatedCount");

                _owner._allocatedCount += count;

                _owner._availableDataCount += count;
                _owner._nonAllocatedForReadDataCount += count;

                _owner.VerifyCounters();

                _dataAvailableCsShadow = _owner._dataAvailableCs;
                _owner._dataAvailableCs = null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void NotifyWritten(int writtenCount, int deallocatedCount)
            {
                Debug.Assert(writtenCount >= 0, "writtenCount > 0");
                Debug.Assert(deallocatedCount > 0, "deallocatedCount > 0");
                Debug.Assert(writtenCount <= deallocatedCount, "writtenCount <= deallocatedCount");
                Debug.Assert(
                    deallocatedCount <= _owner._allocatedCount - _owner._availableDataCount,
                    "deallocatedCount <= _owner._allocatedCount - _owner._availableDataCount");

                _owner._allocatedCount -= deallocatedCount - writtenCount;

                _owner._availableDataCount += writtenCount;
                _owner._nonAllocatedForReadDataCount += writtenCount;

                _owner.VerifyCounters();

                if (deallocatedCount - writtenCount > 0)
                {
                    _writeSpaceAvailableCsShadow = _owner._writeSpaceAvailableCs;
                    _owner._writeSpaceAvailableCs = null;
                }

                if (writtenCount > 0)
                {
                    _dataAvailableCsShadow = _owner._dataAvailableCs;
                    _owner._dataAvailableCs = null;
                }

                if ((_owner._availableDataCount == 0) && _owner._isCompleted)
                {
                    _postCompletion = true;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                _syncObject.Exit();

                _writeSpaceAvailableCsShadow?.SetResult(default);

                _dataAvailableCsShadow?.SetResult(default);

                if (_postCompletion)
                {
                    // it's safe to get completion error without a lock.
                    _owner.PostReadCompletion();
                }
            }
        }
    }
}

#endif