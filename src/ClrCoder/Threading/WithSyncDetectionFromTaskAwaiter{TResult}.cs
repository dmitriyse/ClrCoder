// <copyright file="WithSyncDetectionFromTaskAwaiter{TResult}.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// Proxy awaiter that allows to detect synchronous continuation.
    /// </summary>
    /// <typeparam name="TResult">Await result type.</typeparam>
    public struct WithSyncDetectionFromTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly TaskAwaiter<TResult> _awaiter;

        private readonly Action<bool> _handleDetectionResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="WithSyncDetectionFromTaskAwaiter{TResult}"/> struct.
        /// </summary>
        /// <param name="awaiter">Original awaiter.</param>
        /// <param name="handleDetectionResult">Action that receives detection result.</param>
        public WithSyncDetectionFromTaskAwaiter(TaskAwaiter<TResult> awaiter, Action<bool> handleDetectionResult)
        {
            _awaiter = awaiter;
            _handleDetectionResult = handleDetectionResult;
        }

        /// <summary>
        /// Gets await result.
        /// </summary>
        /// <returns>Await result.</returns>
        [UsedImplicitly]
        public TResult GetResult()
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return _awaiter.GetResult();
        }

        /// <inheritdoc/>
        public void OnCompleted([NotNull] Action continuation)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            _awaiter.OnCompleted(continuation);
        }

        /// <inheritdoc/>
        public void UnsafeOnCompleted([NotNull] Action continuation)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            _awaiter.UnsafeOnCompleted(continuation);
        }

        /// <summary>
        /// Checks if awaitable operation is already completed.
        /// </summary>
        [UsedImplicitly]
        public bool IsCompleted
        {
            get
            {
                // We needs to cache this state.
                bool isCompleted = _awaiter.IsCompleted;

                // And return the same value to async method state machine 
                // and to detect result handler.
                _handleDetectionResult(isCompleted);
                return isCompleted;
            }
        }
    }
}