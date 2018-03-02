// <copyright file="IDsDirectProcessor.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Dsp
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The processor that can directly transform the input data to the output data.
    /// </summary>
    /// <typeparam name="TInput">The type of the input data.</typeparam>
    /// <typeparam name="TOutput">The type of the output data.</typeparam>
    public interface IDsDirectProcessor<TInput, TOutput>
    {
        /// <summary>
        /// Processes directly the input data to the output data.
        /// </summary>
        /// <param name="s1">The first part of the input data.</param>
        /// <param name="s2">The second part of the input data.</param>
        /// <param name="t1">The first part of the output data.</param>
        /// <param name="t2">The second part of the output data.</param>
        /// <param name="inputProcessed">The number of items consumed from the input.</param>
        /// <param name="outputProcessed">The number of items written to the output.</param>
        /// <returns>The task that becomes completed when processor become available to process next items.</returns>
        Task Process(
            ReadOnlySpan<TInput> s1,
            ReadOnlySpan<TInput> s2,
            Span<TOutput> t1,
            Span<TOutput> t2,
            out int inputProcessed,
            out int outputProcessed);
    }
}