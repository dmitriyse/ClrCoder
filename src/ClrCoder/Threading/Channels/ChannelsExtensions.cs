// <copyright file="ChannelsExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Validation;

    /// <summary>
    /// The channels API extension methods.
    /// </summary>
    [PublicAPI]
    public static class ChannelsExtensions
    {
        /// <summary>
        /// Performs copy processing from the reader to the writer.
        /// </summary>
        /// <typeparam name="T">The type of the items to copy.</typeparam>
        /// <param name="reader">The source.</param>
        /// <param name="writer">The target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Async execution TPL task.</returns>
        public static async Task Copy<T>(
            this IChannelReader<T> reader,
            IChannelWriter<T> writer,
            bool allowDropLast = true,
            CancellationToken cancellationToken = default,
            CancellationToken writeLastCancellationToken = default)
        {
            VxArgs.NotNull(reader, nameof(reader));
            VxArgs.NotNull(writer, nameof(writer));
            if (!allowDropLast && (cancellationToken == default))
            {
                throw new ArgumentException(
                    "cancellationToken should be specified, if allowDropLast is false",
                    nameof(cancellationToken));
            }

            if (allowDropLast && (writeLastCancellationToken != default))
            {
                throw new ArgumentException(
                    "writeLastCancellationToken should be used only when allowDropLast is false.",
                    nameof(writeLastCancellationToken));
            }

            CancellationToken readCancellationToken = cancellationToken;
            CancellationToken writeCancellationToken = allowDropLast ? cancellationToken : writeLastCancellationToken;

            while (true)
            {
                // Once read successed. 
                if (reader.TryRead(out var frame))
                {
                    // Trying to write, until target closed, or cancellation raised.
                    while (true)
                    {
                        if (writer.TryWrite(frame))
                        {
                            break;
                        }

                        if (!await writer.WaitToWriteAsync(writeCancellationToken))
                        {
                            throw new TargetWriterBecomesUnusableException();
                        }
                    }
                }
                else
                {
                    if (!await reader.WaitToReadAsync(readCancellationToken))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Tries to propagate completion from the reader to the writer.
        /// </summary>
        /// <typeparam name="T">The type of the items in the reader and writer.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="passthroughError"><see langword="true"/> - passthrough error, <see langword="false"/> - mute error.</param>
        /// <returns>true, if completion was propagated, false otherwise.</returns>
        public static bool TryPropagateCompletion<T>(
            this IChannelReader<T> reader,
            IChannelWriter<T> writer,
            bool passthroughError = true)
        {
            VxArgs.NotNull(reader, nameof(reader));
            VxArgs.NotNull(writer, nameof(writer));

            if (reader.Completion.IsCompleted)
            {
                Exception error = null;
                try
                {
                    reader.Completion.GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    if (passthroughError)
                    {
                        error = ex;
                    }
                }

                return writer.TryComplete(error);
            }

            return false;
        }
    }
}
#endif