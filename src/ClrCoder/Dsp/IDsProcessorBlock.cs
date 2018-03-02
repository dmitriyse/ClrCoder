// <copyright file="IDsProcessorBlock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Dsp
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The DSP processor abstraction.
    /// </summary>
    /// <typeparam name="TInput">The type of the input items.</typeparam>
    public interface IDsProcessorBlock<TInput>
    {
        /// <summary>
        /// Processes provided in the spans information.
        /// </summary>
        /// <param name="s1">The first part of the input data.</param>
        /// <param name="s2">The second part of the input data.</param>
        /// <param name="processedCount">The number of processed items.</param>
        /// <returns>The task that becomes completed when processor become available to process next items.</returns>
        Task Process(ReadOnlySpan<TInput> s1, ReadOnlySpan<TInput> s2, out int processedCount);
    }
}