// <copyright file="ChannelWriterMultiplexer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1

namespace ClrCoder.Threading.Channels.Puzzle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Validation;

    /// <summary>
    /// Writer that writes to multiple inner writers.
    /// </summary>
    /// <typeparam name="T">The type of the item in the channel.</typeparam>
    public class ChannelWriterMultiplexer<T> : ChannelWriterWithoutBatchMode<T>
    {
        private readonly InnerWriterHandler[] _innerWriters;

        private readonly Func<T, T> _cloneFunc;

        private readonly TaskCompletionSource<VoidResult> _completionTcs;

        private readonly object _syncRoot = new object();

        private bool _writeCompleted;

        [CanBeNull]
        private Exception _writeCompletionError;

        //// ReSharper disable once NotAccessedField.Local
        private bool _writeCompletedFromChannel;

        [CanBeNull]
        private Task<bool> _currentMultiWriteTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelWriterMultiplexer{T}"/> class.
        /// </summary>
        /// <param name="innerWriters">The list of inner writers.</param>
        /// <param name="cloneFunc">The item clone func.</param>
        public ChannelWriterMultiplexer(
            IReadOnlyCollection<IChannelWriter<T>> innerWriters,
            Func<T, T> cloneFunc = null)
        {
            VxArgs.NotEmptyReadOnly(innerWriters, nameof(innerWriters));
            _innerWriters = innerWriters.Select(x => new InnerWriterHandler(this, x)).ToArray();
            _cloneFunc = cloneFunc ?? (x => x);
            _completionTcs = new TaskCompletionSource<VoidResult>();
            Completion = new ValueTask(_completionTcs.Task);
        }

        /// <inheritdoc/>
        public override ValueTask Completion { get; }

        /// <inheritdoc/>
        public override void Complete(Exception error = null)
        {
            if (!TryComplete(error))
            {
                throw new ChannelClosedException();
            }
        }

        /// <inheritdoc/>
        public override bool TryComplete(Exception error = null)
        {
            lock (_syncRoot)
            {
                if (_writeCompleted)
                {
                    return false;
                }

                _writeCompleted = true;
                _writeCompletionError = error;
                _writeCompletedFromChannel = true;

                if (_currentMultiWriteTask == null)
                {
                    Exception completeException = null;
                    foreach (InnerWriterHandler innerWriterHandler in _innerWriters)
                    {
                        try
                        {
                            innerWriterHandler.InnerWriter.TryComplete(_writeCompletionError);
                        }
                        catch (Exception ex)
                        {
                            completeException = ex;
                        }
                    }

                    if (completeException != null)
                    {
                        throw completeException;
                    }
                }

                return true;
            }
        }

        /// <inheritdoc/>
        public override bool TryWrite(T item)
        {
            int writtenSynchronouslyCount = 0;

            lock (_syncRoot)
            {
                if (_currentMultiWriteTask != null)
                {
                    return false;
                }

                if (_writeCompleted)
                {
                    return false;
                }

                ValueBox<T>? nextItemClone = item;

                foreach (InnerWriterHandler innerWriterHandler in _innerWriters)
                {
                    if (nextItemClone == null)
                    {
                        nextItemClone = _cloneFunc(item);
                    }

                    try
                    {
                        // ReSharper disable once PossibleInvalidOperationException
                        if (innerWriterHandler.InnerWriter.TryWrite((T)nextItemClone.Value))
                        {
                            nextItemClone = null;
                            writtenSynchronouslyCount++;
                            innerWriterHandler.SetCurrentWriteCompleted();
                        }
                        else
                        {
                            innerWriterHandler.SetItemToWriteAsync(item);
                            nextItemClone = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        writtenSynchronouslyCount++;
                        if (!_writeCompleted)
                        {
                            _writeCompleted = true;
                            _writeCompletionError = ex;
                            _writeCompletedFromChannel = false;
                        }

                        innerWriterHandler.SetCurrentWriteCompleted(ex);
                    }
                }

                // Trying fully synchronous write.
                if (writtenSynchronouslyCount == _innerWriters.Length)
                {
                    foreach (var innerWriterHandler in _innerWriters)
                    {
                        innerWriterHandler.ClearWriteState();
                    }

                    if (_writeCompleted)
                    {
                        foreach (InnerWriterHandler innerWriterHandler in _innerWriters)
                        {
                            // TODO: add propagation policy.
                            innerWriterHandler.InnerWriter.TryComplete(_writeCompletionError);
                        }
                    }

                    return true;
                }

                _currentMultiWriteTask = StartCurrentMultiWriteTask();
                return true;
            }
        }

        /// <inheritdoc/>
        public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken != default)
            {
                throw new NotImplementedException();
            }

            lock (_syncRoot)
            {
                if (_currentMultiWriteTask != null)
                {
                    return new ValueTask<bool>(_currentMultiWriteTask);
                }

                return new ValueTask<bool>(!_writeCompleted);
            }
        }

        /// <inheritdoc/>
        public override async ValueTask WriteAsync(T item, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            ////if (cancellationToken != default)
            ////{
            ////    throw new NotImplementedException();
            ////}

            ////Task writeOperationToWait = null;
            ////lock (_syncRoot)
            ////{
            ////    if (_currentWriteCompletionTcs != null)
            ////    {
            ////        writeOperationToWait = _currentWriteCompletionTcs.Task;
            ////    }
            ////}

            ////if (writeOperationToWait != null)
            ////{
            ////    bool await writeOperationToWait
            ////}
        }

        private async Task<bool> StartCurrentMultiWriteTask()
        {
            // Here we are under lock
            bool isWriteCompleted = _writeCompleted;
            await Task.Yield();

            foreach (InnerWriterHandler innerWriterHandler in _innerWriters)
            {
                if (!innerWriterHandler.WriteCompletedSynchronously)
                {
                    Exception error = null;
                    bool isCompletedFromChannel = true;
                    bool thisWriterCompleted = false;

                    while (true)
                    {
                        try
                        {
                            if (await innerWriterHandler.InnerWriter.WaitToWriteAsync())
                            {
                                // ReSharper disable once PossibleInvalidOperationException
                                if (innerWriterHandler.InnerWriter.TryWrite(
                                    (T)innerWriterHandler.ItemToWriteAsync.Value))
                                {
                                    break;
                                }

                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            error = ex;
                            isCompletedFromChannel = false;
                            thisWriterCompleted = true;
                            break;
                        }

                        thisWriterCompleted = true;
                        try
                        {
                            await innerWriterHandler.InnerWriter.Completion;
                        }
                        catch (Exception ex)
                        {
                            error = ex;
                            break;
                        }
                    }

                    // Trying set writer completed state.
                    if (!isWriteCompleted && thisWriterCompleted)
                    {
                        lock (_syncRoot)
                        {
                            isWriteCompleted = true;
                            if (!_writeCompleted)
                            {
                                _writeCompleted = true;
                                _writeCompletedFromChannel = isCompletedFromChannel;
                                _writeCompletionError = error;
                            }
                        }
                    }
                }
            }

            // Completing async multi write.
            lock (_syncRoot)
            {
                isWriteCompleted = _writeCompleted;
                foreach (InnerWriterHandler innerWriterHandler in _innerWriters)
                {
                    innerWriterHandler.ClearWriteState();
                    if (isWriteCompleted)
                    {
                        innerWriterHandler.InnerWriter.TryComplete(_writeCompletionError);
                    }
                }

                _currentMultiWriteTask = null;
            }

            return !isWriteCompleted;
        }

        private class InnerWriterHandler
        {
            private readonly ChannelWriterMultiplexer<T> _owner;

            public InnerWriterHandler(ChannelWriterMultiplexer<T> owner, IChannelWriter<T> innerWriter)
            {
                InnerWriter = innerWriter;
                _owner = owner;
            }

            public IChannelWriter<T> InnerWriter { get; }

            public bool WriteCompletedSynchronously { get; private set; }

            public ValueBox<T>? ItemToWriteAsync { get; private set; }

            [CanBeNull]
            public Exception WriteError { get; private set; }

            public void ClearWriteState()
            {
                WriteError = null;
                WriteCompletedSynchronously = false;
                ItemToWriteAsync = null;
            }

            public void SetCurrentWriteCompleted([CanBeNull] Exception error = null)
            {
                WriteCompletedSynchronously = true;
                WriteError = error;
            }

            public void SetItemToWriteAsync(T item)
            {
                ItemToWriteAsync = item;
            }
        }
    }
}

#endif