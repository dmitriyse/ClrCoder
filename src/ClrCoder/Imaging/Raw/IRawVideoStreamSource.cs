// <copyright file="IRawVideoStreamSource.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_3
namespace ClrCoder.Imaging.Raw
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The raw video stream source.
    /// </summary>
    public interface IRawVideoStreamSource
    {
        /// <summary>
        /// The video format of the source.
        /// </summary>
        RawVideoStreamFormat VideoFormat { get; }

        /// <summary>
        /// Starts reading from the source.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can stop current operation or can completes reader channel.</param>
        /// <returns>The reader structure.</returns>
        ValueTask<IRawVideoStreamReader> StartRead(CancellationToken cancellationToken);
    }
}
#endif