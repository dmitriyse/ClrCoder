// <copyright file="IDsTarget.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Dsp
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The DSP processing target (receives data from a processor).
    /// </summary>
    /// <remarks>TODO: Add back pressure capability!</remarks>
    /// <typeparam name="TOutput">The type or processing output items.</typeparam>
    public interface IDsTarget<TOutput>
    {
        /// <summary>
        /// Pass-through processing to the provided <see cref=""/> with output buffers provision.
        /// </summary>
        /// <typeparam name="TProcessor">
        /// The type of the direct processor. (Use <see langword="struct"/> to get maximal
        /// performance).
        /// </typeparam>
        /// <param name="processor">The processor that produces output data.</param>
        /// <returns>The task that becomes completed when processor become available to process next items.</returns>
        Task Process<TProcessor>(
            TProcessor processor)
            where TProcessor : IDsProcessor<TOutput>;

        /// <summary>
        /// Pass-through processing to the provided <see cref="directProcessor"/> with output buffers provision.
        /// </summary>
        /// <typeparam name="TDirectProcessor">
        /// The type of the direct processor. (Use <see langword="struct"/> to get maximal
        /// performance).
        /// </typeparam>
        /// <typeparam name="TInput">The type of the input data.</typeparam>
        /// <param name="directProcessor">The direct input to output processor.</param>
        /// <param name="s1">The first part of the input data.</param>
        /// <param name="s2">The second part of the input data.</param>
        /// <param name="inputProcessed">The number of inputProcessed items.</param>
        /// <returns>The task that becomes completed when processor become available to process next items.</returns>
        Task ProcessDirect<TDirectProcessor, TInput>(
            TDirectProcessor directProcessor,
            ReadOnlySpan<TInput> s1,
            ReadOnlySpan<TInput> s2,
            out int inputProcessed)
            where TDirectProcessor : IDsDirectProcessor<TInput, TOutput>;
    }
}