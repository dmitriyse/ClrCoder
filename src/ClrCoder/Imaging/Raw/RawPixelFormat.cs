// <copyright file="RawPixelFormat.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Imaging.Raw
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// The raw pixel format.
    /// </summary>
    [PublicAPI]
    public class RawPixelFormat
    {
        static RawPixelFormat()
        {
            Registry = new Dictionary<RawPixelFormatCode, RawPixelFormat>
                           {
                               [RawPixelFormatCode.Gray8] = new RawPixelFormat(
                                   RawPixelFormatCode.Gray8,
                                   1,
                                   new[]
                                       {
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Gray,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               0,
                                               8)
                                       }),

                               [RawPixelFormatCode.RGB8] = new RawPixelFormat(
                                   RawPixelFormatCode.RGB8,
                                   3,
                                   new[]
                                       {
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Red,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               0,
                                               8),
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Green,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               8,
                                               8),
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Blue,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               16,
                                               8),
                                       }),
                               [RawPixelFormatCode.RGBX8] = new RawPixelFormat(
                                   RawPixelFormatCode.RGBX8,
                                   4,
                                   new[]
                                       {
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Red,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               0,
                                               8),
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Green,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               8,
                                               8),
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Blue,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               16,
                                               8),
                                       }),
                               [RawPixelFormatCode.RGBA8] = new RawPixelFormat(
                                   RawPixelFormatCode.RGBA8,
                                   4,
                                   new[]
                                       {
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Red,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               0,
                                               8),
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Green,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               8,
                                               8),
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Blue,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               16,
                                               8),
                                           new RawPixelChannelFormat(
                                               RawPixelChannelType.Alpha,
                                               RawPixelChannelValuesFormat.LinearInt,
                                               24,
                                               8),
                                       }),
                           };
        }

        protected RawPixelFormat(
            RawPixelFormatCode code,
            int bytesPerPixel,
            IReadOnlyList<RawPixelChannelFormat> channels)
        {
            Code = code;
            BytesPerPixel = bytesPerPixel;
            Channels = channels;
        }

        /// <summary>
        /// The formats registry.
        /// </summary>
        public static IReadOnlyDictionary<RawPixelFormatCode, RawPixelFormat> Registry { get; }

        /// <summary>
        /// The pixel format channels collection.
        /// </summary>
        public IReadOnlyList<RawPixelChannelFormat> Channels { get; }

        /// <summary>
        /// The pixel format well-known code.
        /// </summary>
        public RawPixelFormatCode Code { get; }

        /// <summary>
        /// The amount of bytes per one raw pixel.
        /// </summary>
        public int BytesPerPixel { get; }
    }
}