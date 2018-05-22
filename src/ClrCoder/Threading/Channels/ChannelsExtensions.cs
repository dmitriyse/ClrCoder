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
            CancellationToken cancellationToken = default,
            ChannelCompletionPassthroughProc producerCompletionPassthroughProc = null,
            ChannelCompletionPassthroughProc consumerCompletionPassthroughProc = null,
            Func<T, ValueTask> handleLostItemsProc = null)
        {
            VxArgs.NotNull(reader, nameof(reader));
            VxArgs.NotNull(writer, nameof(writer));

            CancellationToken readCancellationToken = cancellationToken;

            if (producerCompletionPassthroughProc == null)
            {
                producerCompletionPassthroughProc = (error, isFromChannel) =>
                    {
                        writer.TryComplete(error);
                        return new ValueTask();
                    };
            }

            if (consumerCompletionPassthroughProc == null)
            {
                consumerCompletionPassthroughProc = (error, isFromChannel) =>
                    {
                        reader.TryComplete(error);
                        return new ValueTask();
                    };
            }

            Exception handlersError = null;
            try
            {
                while (true)
                {
                    T item;
                    try
                    {
                        if (!reader.TryRead(out item))
                        {
                            continue;
                        }

                        if (!await reader.WaitToReadAsync(readCancellationToken))
                        {
                            Exception error = null;
                            try
                            {
                                await reader.Completion;
                            }
                            catch (Exception ex)
                            {
                                error = ex;
                            }
                            finally
                            {
                                try
                                {
                                    await producerCompletionPassthroughProc(error);
                                }
                                catch (Exception ex)
                                {
                                    handlersError = ex;
                                }
                            }

                            return;
                        }
                    }
                    catch (Exception error)
                    {
                        try
                        {
                            await producerCompletionPassthroughProc(error, false);
                        }
                        catch (Exception ex)
                        {
                            handlersError = ex;
                        }

                        return;
                    }

                    // Trying to write, until target closed, or cancellation raised.
                    while (true)
                    {
                        try
                        {
                            if (writer.TryWrite(item))
                            {
                                break;
                            }

                            if (!await writer.WaitToWriteAsync(readCancellationToken))
                            {
                                if (handleLostItemsProc != null)
                                {
                                    try
                                    {
                                        await handleLostItemsProc(item);
                                    }
                                    catch (Exception ex)
                                    {
                                        handlersError = ex;
                                    }
                                }

                                Exception error = null;
                                try
                                {
                                    await writer.Completion;
                                }
                                catch (Exception ex)
                                {
                                    error = ex;
                                }
                                finally
                                {
                                    try
                                    {
                                        await consumerCompletionPassthroughProc(error);
                                    }
                                    catch (Exception ex)
                                    {
                                        handlersError = ex;
                                    }
                                }

                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            try
                            {
                                if (handleLostItemsProc != null)
                                {
                                    try
                                    {
                                        await handleLostItemsProc(item);
                                    }
                                    catch (Exception ex)
                                    {
                                        handlersError = ex;
                                    }
                                }
                            }
                            finally
                            {
                                try
                                {
                                    await consumerCompletionPassthroughProc(error, false);
                                }
                                catch (Exception ex)
                                {
                                    handlersError = ex;
                                }
                            }

                            return;
                        }
                    }
                }
            }
            finally
            {
                if (handlersError != null)
                {
                    throw handlersError;
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