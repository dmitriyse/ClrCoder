// <copyright file="DsBufferBase.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Dsp
{
    using System;
    using System.Threading.Tasks;

    using Threading;

    /// <summary>
    /// The base implementation of inter-processor blocks buffer.
    /// </summary>
    /// <typeparam name="T">The type of the processing items.</typeparam>
    public class DsBufferBase<T> : ActiveWorker, IDsTarget<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DsBufferBase{T}"/> class.
        /// </summary>
        public DsBufferBase(
        )
            : base(null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task Process<TProcessor>(TProcessor processor)
            where TProcessor : IDsProcessor<T>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ProcessDirect<TDirectProcessor, TInput>(
            TDirectProcessor directProcessor,
            ReadOnlySpan<TInput> s1,
            ReadOnlySpan<TInput> s2,
            out int inputProcessed)
            where TDirectProcessor : IDsDirectProcessor<TInput, T>
        {
            throw new NotImplementedException();
        }
    }
}