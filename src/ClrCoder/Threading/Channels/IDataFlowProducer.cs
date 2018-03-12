// <copyright file="IDataFlowProducer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;

    /// <summary>
    /// The data flow producer.
    /// </summary>
    /// <typeparam name="T">
    /// Specifies the type of data that may be read from the producer.
    /// </typeparam>
    public interface IDataFlowProducer<T>
    {
        /// <summary>
        /// Opens the new reader.
        /// </summary>
        /// <remarks>
        /// Producer can use some load balancing logic between known open readers. You can close a reader through the
        /// <see cref="IDisposable"/> interface.
        /// </remarks>
        /// <returns>The new reader.</returns>
        IChannelReader<T> OpenReader();
    }
}
#endif