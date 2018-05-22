// <copyright file="WriterProxyWithDropOnErrors.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1

namespace ClrCoder.Threading.Channels.Puzzle
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// The channel writer proxy, that drop items when inner channel is in the faulted state.
    /// </summary>
    /// <typeparam name="T">The type of the items in the channel.</typeparam>
    [PublicAPI]
    public class WriterProxyWithDropOnErrors<T> : ChannelWriterWithoutBatchMode<T>
    {
        private readonly IChannelWriter<T> _innerWriter;

        [CanBeNull]
        private readonly Action<T> _dropItemAction;

        private readonly TaskCompletionSource<VoidResult> _writeCompletionTcs = new TaskCompletionSource<VoidResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WriterProxyWithDropOnErrors{T}"/> class.
        /// </summary>
        /// <param name="innerWriter">The inner writer.</param>
        /// <param name="dropItemAction">The drop action.</param>
        public WriterProxyWithDropOnErrors(IChannelWriter<T> innerWriter, Action<T> dropItemAction = null)
        {
            _innerWriter = innerWriter;
            _dropItemAction = dropItemAction;
            Completion = new ValueTask(_writeCompletionTcs.Task);
        }

        /// <inheritdoc/>
        public override ValueTask Completion { get; }

        /// <inheritdoc/>
        public override void Complete(Exception error = null)
        {
            bool resultWasSet = error == null
                                    ? _writeCompletionTcs.TrySetResult(default)
                                    : _writeCompletionTcs.TrySetException(error);
            if (!resultWasSet)
            {
                throw new ChannelClosedException();
            }

            _innerWriter.TryComplete(error);
        }

        /// <inheritdoc/>
        public override bool TryComplete(Exception error = null)
        {
            bool result =  error == null
                       ? _writeCompletionTcs.TrySetResult(default)
                       : _writeCompletionTcs.TrySetException(error);
            if (result)
            {
                _innerWriter.TryComplete(error);
            }

            return result;
        }

        /// <inheritdoc/>
        public override bool TryWrite(T item)
        {
            if (_writeCompletionTcs.Task.IsCompleted)
            {
                return false;
            }

            // Dropping.
            if (_innerWriter.Completion.IsCompleted)
            {
                _dropItemAction?.Invoke(item);
                return true;
            }

            return _innerWriter.TryWrite(item);
        }

        /// <inheritdoc/>
        public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken)
        {
            if (_writeCompletionTcs.Task.IsCompleted)
            {
                return new ValueTask<bool>(false);
            }

            if (_innerWriter.Completion.IsCompleted)
            {
                return new ValueTask<bool>(true);
            }

            return _innerWriter.WaitToWriteAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override async ValueTask WriteAsync(T item, CancellationToken cancellationToken)
        {
            if (_writeCompletionTcs.Task.IsCompleted)
            {
                throw new ChannelClosedException();
            }

            if (_innerWriter.Completion.IsCompleted)
            {
                _dropItemAction?.Invoke(item);
            }

            try
            {
                await _innerWriter.WriteAsync(item, cancellationToken);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
            {
                throw;
            }
            catch (Exception)
            {
                _dropItemAction?.Invoke(item);
            }
        }
    }
}

#endif