// <copyright file="RawPixelFormatCode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Imaging.Raw
{
    using JetBrains.Annotations;

    //// ReSharper disable InconsistentNaming

    /// <summary>
    /// The pixel format identifiers enumeration.
    /// </summary>
    [PublicAPI]
    public enum RawPixelFormatCode
    {
        /// <summary>
        /// 8 bit per color Red, Green, Blue, Alpha format.
        /// </summary>
        RGBA8,

        /// <summary>
        /// 8 bit per color Red, Green, Blue format with 8 bit padding.
        /// </summary>
        RGBX8,

        /// <summary>
        /// 8 bit per color (Red, Green, Bule) format.
        /// </summary>
        RGB8,

        /// <summary>
        /// 8 bit grayscale format.
        /// </summary>
        Gray8
    }

    //// ReSharper restore InconsistentNaming
}