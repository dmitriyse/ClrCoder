// <copyright file="RawImage.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0
namespace ClrCoder.Imaging.Raw
{
    using System;
    using System.Drawing;

    /// <summary>
    /// The binary access interface to a raw image.
    /// </summary>
    public class RawImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawImage"/> class. 
        /// </summary>
        /// <param name="pixelFormat">The pixel format of the image.</param>
        /// <param name="size">The actual size of the image.</param>
        /// <param name="writableData">The raw image memory that is allowed to be modified, or null.</param>
        /// <param name="rowLengthInBytes">The length in bytes of a one row.</param>
        public RawImage(RawPixelFormatCode pixelFormat, Size size, Memory<byte> writableData, int rowLengthInBytes)
        {
            // TODO: Add verification.
            PixelFormat = pixelFormat;
            Size = size;
            WritableData = writableData;
            Data = writableData;
            RowLengthInBytes = rowLengthInBytes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawImage"/> class. 
        /// </summary>
        /// <param name="pixelFormat">The pixel format of the image.</param>
        /// <param name="size">The actual size of the image.</param>
        /// <param name="data">The raw image memory with readonly access.</param>
        /// <param name="rowLengthInBytes">The length in bytes of a one row.</param>
        public RawImage(RawPixelFormatCode pixelFormat, Size size, ReadOnlyMemory<byte> data, int rowLengthInBytes)
        {
            PixelFormat = pixelFormat;
            Size = size;
            Data = data;
            RowLengthInBytes = rowLengthInBytes;
        }

        /// <summary>
        /// The actual size of the image.
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// The pixel format of the image.
        /// </summary>
        public RawPixelFormatCode PixelFormat { get; }

        /// <summary>
        /// The length in bytes of a one row.
        /// </summary>
        public int RowLengthInBytes { get; }

        /// <summary>
        /// The raw image memory with readonly access.
        /// </summary>
        public ReadOnlyMemory<byte> Data { get; }

        /// <summary>
        /// The raw image memory that is allowed to be modified, or null.
        /// </summary>
        public Memory<byte>? WritableData { get; }
    }
}
#endif