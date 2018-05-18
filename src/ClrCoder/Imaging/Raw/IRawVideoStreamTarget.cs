// <copyright file="IRawVideoStreamTarget.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_3
namespace ClrCoder.Imaging.Raw
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The raw video stream target.
    /// </summary>
    public interface IRawVideoStreamTarget : IAsyncDisposable
    {
        /// <summary>
        /// The video format of the source.
        /// </summary>
        RawVideoStreamFormat VideoFormat { get; }

        /// <summary>
        /// Opens a raw video writer.
        /// </summary>
        /// <returns>The video writer interface.</returns>
        ValueTask<IRawVideoStreamWriter> OpenWriter();
    }
}
#endif