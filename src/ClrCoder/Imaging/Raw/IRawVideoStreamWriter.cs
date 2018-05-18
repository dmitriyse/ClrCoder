// <copyright file="IRawVideoStreamWriter.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_3
namespace ClrCoder.Imaging.Raw
{
    using System.Threading;

    using Threading.Channels;

    /// <summary>
    /// The raw video writer contract.
    /// </summary>
    public interface IRawVideoStreamWriter : IChannelWriter<IRawVideoFrame>, IAsyncDisposable
    {
    }
}
#endif