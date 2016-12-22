// <copyright file="AlternativeResult.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Helper exception for alternative flow pattern.
    /// </summary>
    /// <typeparam name="T"><c>Type</c> of a value, to be used in alternative flow.</typeparam>
    [PublicAPI]
    public class AlternativeResult<T> : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlternativeResult{T}"/> class.
        /// </summary>
        /// <param name="result">Value, that should be handled in alternative flow.</param>
        public AlternativeResult(T result)
        {
            Result = result;
        }

        /// <summary>
        /// Value, that should be handled in alternative flow.
        /// </summary>
        public T Result { get; private set; }
    }
}