// <copyright file="LoggerExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using JetBrains.Annotations;

    /// <summary>
    /// Json logger subsystem extension methods.
    /// </summary>
    [PublicAPI]
    public static class LoggerExtensions
    {
        private static readonly JsonLogEntryInterceptor DoNothingInterceptor = builder => builder;

        /// <summary>
        /// Replaces <see langword="null"/> passed to the <paramref name="interceptor"/> with the "do nothing" interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor or null.</param>
        /// <returns>Always not null interceptor.</returns>
        public static JsonLogEntryInterceptor DoNothingIfNull([CanBeNull] this JsonLogEntryInterceptor interceptor)
        {
            return interceptor ?? DoNothingInterceptor;
        }
    }
}