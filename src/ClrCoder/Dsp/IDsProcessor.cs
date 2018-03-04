// <copyright file="IDsProcessor.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Dsp
{
    using System;

    /// <summary>
    /// The processor that can produce output data to the provided buffers.
    /// </summary>
    /// <typeparam name="TOutput">The output data type.</typeparam>
    public interface IDsProcessor<TOutput>
    {
        /// <summary>
        /// Processes directly the input data to the output data.
        /// </summary>
        /// <param name="t1">The first part of the output data.</param>
        /// <param name="t2">The second part of the output data.</param>
        /// <param name="outputProcessed">The number of items written to the output.</param>
        void Process(
            Span<TOutput> t1,
            Span<TOutput> t2,
            out int outputProcessed);
    }
}