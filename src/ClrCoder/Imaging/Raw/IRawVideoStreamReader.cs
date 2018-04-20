// <copyright file="IRawVideoStreamReader.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_3
namespace ClrCoder.Imaging.Raw
{
    using NodaTime;

    using Threading.Channels;

    /// <summary>
    /// The video stram reader interface.
    /// </summary>
    public interface IRawVideoStreamReader : IChannelReader<IRawVideoFrame>
    {
        /// <summary>
        /// Binding of the video stream to the real world.
        /// </summary>
        Instant? FirstFrameInstant { get; }
    }
}

#endif