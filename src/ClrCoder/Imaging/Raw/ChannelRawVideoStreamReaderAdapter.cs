// <copyright file="ChannelRawVideoStreamReaderAdapter.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_3

namespace ClrCoder.Imaging.Raw
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using NodaTime;

    using Threading.Channels;

    using Validation;

    /// <summary>
    /// The wrapper over BCL channel reader with video frames.
    /// </summary>
    public class ChannelRawVideoStreamReaderAdapter : ChannelReaderWithoutBatchMode<IRawVideoFrame>,
                                                      IRawVideoStreamReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelRawVideoStreamReaderAdapter"/> class.
        /// </summary>
        /// <param name="innerReader">The BCL channel reader.</param>
        /// <param name="videoFormat">The format of the video stream.</param>
        /// <param name="firstFrameInstant">Binding of the video stream to the real world.</param>
        public ChannelRawVideoStreamReaderAdapter(
            IChannelReader<IRawVideoFrame> innerReader,
            RawVideoStreamFormat videoFormat,
            Instant? firstFrameInstant)
        {
            VxArgs.NotNull(innerReader, nameof(innerReader));
            VxArgs.NotNull(videoFormat, nameof(videoFormat));

            InnerReader = innerReader;
            VideoFormat = videoFormat;
            FirstFrameInstant = firstFrameInstant;
        }

        /// <inheritdoc/>
        public RawVideoStreamFormat VideoFormat { get; }

        /// <inheritdoc/>
        public Instant? FirstFrameInstant { get; }

        /// <inheritdoc/>
        public override ValueTask Completion => InnerReader.Completion;

        /// <summary>
        /// The inner reader.
        /// </summary>
        public IChannelReader<IRawVideoFrame> InnerReader { get; }

        /// <inheritdoc/>
        public override void Complete(Exception error = null) => InnerReader.Complete(error);

        /// <inheritdoc/>
        public async Task DisposeAsync()
        {
            // Do nothing.
        }

        /// <inheritdoc/>
        public override ValueTask<IRawVideoFrame> ReadAsync(CancellationToken cancellationToken = default) =>
            InnerReader.ReadAsync(cancellationToken);

        /// <inheritdoc/>
        public override bool TryComplete(Exception error = null) => InnerReader.TryComplete(error);

        /// <inheritdoc/>
        public override bool TryRead(out IRawVideoFrame item) => InnerReader.TryRead(out item);

        /// <inheritdoc/>
        public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default) =>
            InnerReader.WaitToReadAsync(cancellationToken);
    }
}

#endif