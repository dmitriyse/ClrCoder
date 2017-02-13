// <copyright file="WithSyncDetectionFromTaskAwaitable{TResult}.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// Helps to await in syntax with "WithSyncDetection".
    /// </summary>
    public struct WithSyncDetectionFromTaskAwaitable<TResult>
    {
        private readonly TaskAwaiter<TResult> _awaiter;

        private readonly Action<bool> _handleDetectionResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="WithSyncDetectionFromTaskAwaitable"/> struct.
        /// </summary>
        /// <param name="awaiter">Original awaiter.</param>
        /// <param name="handleDetectionResult">Action that receives detection result.</param>
        internal WithSyncDetectionFromTaskAwaitable(TaskAwaiter<TResult> awaiter, Action<bool> handleDetectionResult)
        {
            _awaiter = awaiter;
            _handleDetectionResult = handleDetectionResult;
        }

        /// <summary>
        /// Used by C# compiler in await syntax.
        /// </summary>
        /// <returns><c>Awaiter</c> implementation.</returns>
        [UsedImplicitly]
        public WithSyncDetectionFromTaskAwaiter<TResult> GetAwaiter()
        {
            return new WithSyncDetectionFromTaskAwaiter<TResult>(_awaiter, _handleDetectionResult);
        }
    }
}