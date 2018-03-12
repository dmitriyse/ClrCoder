// <copyright file="IDataFlowConsumer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.Threading.Channels
{
    using System;

    /// <summary>
    /// The data flow consumer.
    /// </summary>
    /// <typeparam name="T">Specifies the type of data that may be written to the consumer.</typeparam>
    public interface IDataFlowConsumer<T>
    {
        /// <summary>
        /// Opens the new writer.
        /// </summary>
        /// <remarks>
        /// Producer can use some load balancing logic between known open writers. You can close a writer through the
        /// <see cref="IDisposable"/> interface.
        /// </remarks>
        /// <returns>The new writer.</returns>
        IChannelWriter<T> OpenWriter();
    }
}
#endif