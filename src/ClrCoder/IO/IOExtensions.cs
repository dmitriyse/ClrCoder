// <copyright file="IOExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    //// ReSharper disable once InconsistentNaming

    /// <summary>
    /// Extensions related to IO <see langword="namespace"/>.
    /// </summary>
    [PublicAPI]
    public static class IOExtensions
    {
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
        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream)
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
    }
}