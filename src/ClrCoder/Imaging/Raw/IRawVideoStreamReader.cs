// <copyright file="IRawVideoStreamReader.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_3
namespace ClrCoder.Imaging.Raw
{
    using System.Threading;

    using NodaTime;

    using Threading.Channels;

    /// <summary>
    /// The video stream reader interface.
    /// </summary>
    public interface IRawVideoStreamReader : IChannelReader<IRawVideoFrame>, IAsyncDisposable
    {
        /// <summary>
        /// Binding of the video stream to the real world.
        /// </summary>
        Instant? FirstFrameInstant { get; }
    }
}

#endif