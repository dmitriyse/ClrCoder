// <copyright file="StreamBinaryResource.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the .Net stream abstraction as a binary resource.
    /// </summary>
    /// <typeparam name="TStream">The type of the stream.</typeparam>
    public class StreamBinaryResource<TStream> : IBinaryResource
        where TStream : Stream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamBinaryResource{TStream}"/> class.
        /// </summary>
        /// <param name="stream">The stream to represent as a binary resource.</param>
        public StreamBinaryResource(TStream stream)
        {
            Stream = stream;
        }

        /// <summary>
        /// The underlying stream.
        /// </summary>
        public TStream Stream { get; }

        /// <inheritdoc/>
        public ValueTask<(byte[], int)> ReadIntoBuffer(bool allowCaching = true, bool exactSize = false)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ReadFromFile(Func<string, Task> readProc)
        {
            // TODO: Implement with temporary file or infer file from the file stream (probably).
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task ReadFromMemory(Func<IntPtr, long, Task> readProc)
        {
#if !NETSTANDARD1_0 && !NETSTANDARD1_1
            if (Stream is MemoryStream memoryStream)
            {
                // Do nothing.
            }
            else
            {
                var length = Stream.GetLengthSafe();
                if (length != null)
                {
                    memoryStream = new MemoryStream((int)length);
                }
                else
                {
                    memoryStream = new MemoryStream();
                }

                if (Stream.CanSeek)
                {
                    Stream.Seek(0, SeekOrigin.Begin);
                }

                await Stream.CopyToAsync(memoryStream);
            }

            if (memoryStream.TryGetBuffer(out var segment))
            {
                var buffer = segment.Array;
                unsafe
                {
                    fixed (byte* bufferPtr = buffer)
                    {
                        // Sync/async interleave is required to hold array pinned.
                        readProc((IntPtr)(bufferPtr + segment.Offset), memoryStream.Length).ConfigureAwait(false)
                            .GetAwaiter().GetResult();
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Fallback to memory copy is not implemented yet.");
            }

#else
            throw new NotSupportedException();
#endif
        }

        /// <inheritdoc/>
        public async Task ReadFromStream(Func<Stream, Task> readProc)
        {
            if (Stream.CanSeek)
            {
                Stream.Seek(0, SeekOrigin.Begin);
            }

            await readProc(Stream);
        }

        /// <inheritdoc/>
        public Task WriteToFile(Func<string, Task> writeProc)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task WriteToStream(Func<Stream, Task> writeProc)
        {
            throw new NotImplementedException();
        }
    }
}