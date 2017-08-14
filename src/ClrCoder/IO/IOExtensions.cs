// <copyright file="IOExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.IO
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Validation;

    //// ReSharper disable once InconsistentNaming

    /// <summary>
    /// Extensions related to IO <see langword="namespace"/>.
    /// </summary>
    [PublicAPI]
    public static class IOExtensions
    {
        /// <summary>
        /// Copies data from stream to stream with limit on maximal written data.
        /// </summary>
        /// <param name="stream">Source stream.</param>
        /// <param name="targetStream">Target stream.</param>
        /// <param name="maxWriteAmount">Maximal allwed amount to write.</param>
        /// <exception cref="IOException">Maximal allowed write reached.</exception>
        public static void CopyToWithLimit(this Stream stream, Stream targetStream, long maxWriteAmount)
        {
            VxArgs.NotNull(stream, nameof(stream));
            VxArgs.NotNull(targetStream, nameof(targetStream));
            if (maxWriteAmount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxWriteAmount),
                    "Maximal write amount should be positive.");
            }

            stream.CopyTo(new LimitWriteStream(targetStream, maxWriteAmount));
        }

        /// <summary>
        /// Copies data from stream to stream with limit on maximal written data.
        /// </summary>
        /// <param name="stream">Source stream.</param>
        /// <param name="targetStream">Target stream.</param>
        /// <param name="maxWriteAmount">Maximal allwed amount to write.</param>
        /// <returns>Async execution TPL task.</returns>
        /// <exception cref="IOException">Maximal allowed write reached.</exception>
        public static async Task CopyToWithLimitAsync(this Stream stream, Stream targetStream, long maxWriteAmount)
        {
            VxArgs.NotNull(stream, nameof(stream));
            VxArgs.NotNull(targetStream, nameof(targetStream));
            if (maxWriteAmount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxWriteAmount),
                    "Maximal write amount should be positive.");
            }

            await stream.CopyToAsync(new LimitWriteStream(targetStream, maxWriteAmount));
        }

        /// <summary>
        /// Gets length of a <c>stream</c> if it is supported.
        /// </summary>
        /// <param name="stream">Stream to get length for.</param>
        /// <returns>Length of a <c>stream</c> or <see langword="null"/> if it is not supported.</returns>
        public static long? GetLengthSafe(this Stream stream)
        {
            // TODO: Write tests for me.
            try
            {
                return stream.Length;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Reads all bytes from <c>stream</c> from current position until the end.
        /// </summary>
        /// <param name="stream">Stream to read.</param>
        /// <returns>Read bytes.</returns>
        public static async ValueTask<byte[]> ReadAllBytesAsync(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            long? length = stream.GetLengthSafe();

            MemoryStream bufferStream = length != null
                                            ? new MemoryStream((int)(length.Value - stream.Position))
                                            : new MemoryStream();

            await stream.CopyToAsync(bufferStream);
            return bufferStream.ToArray();
        }

        private class LimitWriteStream : Stream
        {
            private readonly Stream _inner;

            private readonly long _maxWriteAmount;

            private long _written;

            public LimitWriteStream(Stream inner, long maxWriteAmount)
            {
                _inner = inner;
                _maxWriteAmount = maxWriteAmount;
            }

            public override bool CanRead
            {
                get
                {
                    return _inner.CanRead;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    return _inner.CanSeek;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    return _inner.CanWrite;
                }
            }

            public override long Length
            {
                get
                {
                    return _inner.Length;
                }
            }

            public override long Position
            {
                get
                {
                    return _inner.Position;
                }

                set
                {
                    _inner.Position = value;
                }
            }

            public override void Flush()
            {
                _inner.Flush();
            }

            public override int Read([NotNull] byte[] buffer, int offset, int count)
            {
                return _inner.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _inner.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _inner.SetLength(value);
            }

            public override void Write([NotNull] byte[] buffer, int offset, int count)
            {
                if (Volatile.Read(ref _written) >= _maxWriteAmount)
                {
                    throw new IOException("Maximal allowed write reached.");
                }

                _inner.Write(buffer, offset, count);
                Interlocked.Add(ref _written, count);
            }

            public override async Task WriteAsync(
                [NotNull] byte[] buffer,
                int offset,
                int count,
                CancellationToken cancellationToken)
            {
                if (Volatile.Read(ref _written) >= _maxWriteAmount)
                {
                    throw new IOException("Maximal allowed write reached.");
                }

                await _inner.WriteAsync(buffer, offset, count, cancellationToken);
                Interlocked.Add(ref _written, count);
            }
        }
    }
}