// <copyright file="RawPixelChannelValuesFormat.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Imaging.Raw
{
    /// <summary>
    /// The raw pixel channel values format type.
    /// </summary>
    public enum RawPixelChannelValuesFormat
    {
        /// <summary>
        /// Linear integer representation.
        /// </summary>
        LinearInt = 1,

        /// <summary>
        /// 16 bit float format.
        /// </summary>
        Ieee754 = 2
    }
}