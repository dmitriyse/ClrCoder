// <copyright file="RawPixelChannelType.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Imaging.Raw
{
    /// <summary>
    /// The raw image channel type.
    /// </summary>
    public enum RawPixelChannelType
    {
        /// <summary>
        /// The alpha channal.
        /// </summary>
        Alpha = 1,

        /// <summary>
        /// The light channel in a grayscale format.
        /// </summary>
        Gray = 2,

        /// <summary>
        /// The red color channel.
        /// </summary>
        Red = 3,

        /// <summary>
        /// The green color channel.
        /// </summary>
        Green = 4,

        /// <summary>
        /// The blue color channel
        /// </summary>
        Blue = 5,

        /// <summary>
        /// The luminance channel in the YUV
        /// </summary>
        Y = 6,

        /// <summary>
        /// U channel in the YUV format.
        /// </summary>
        U = 7,

        /// <summary>
        /// V channel in the YUV format.
        /// </summary>
        V = 8
    }
}