// <copyright file="IRawVideoFrame.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_3
namespace ClrCoder.Imaging.Raw
{
    using System;

    using NodaTime;

    /// <summary>
    /// The video frame of a raw video stream.
    /// </summary>
    public interface IRawVideoFrame : IDisposable
    {
        /// <summary>
        /// The raw image access to the frame.
        /// </summary>
        RawImage Image { get; }

        /// <summary>
        /// The duration between the frame and the start of the stream.
        /// </summary>
        Duration Time { get; }
    }
}

#endif