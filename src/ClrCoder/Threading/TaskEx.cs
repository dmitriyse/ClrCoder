// <copyright file="TaskEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
#pragma warning disable 1998
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Task related extensions.
    /// </summary>
    [PublicAPI]
    public static class TaskEx
    {
        /// <summary>
        /// Use this variable in a performance critical places, because readonly field is slightly faster that get-only property.
        /// </summary>
        public static readonly Task CompletedTaskValue;

        static TaskEx()
        {
            CompletedTask = ((Func<Task>)(async () => { }))();
            CompletedTaskValue = CompletedTask;
        }

        /// <summary>
        /// Alternative to Task.CompletedTask (for .Net Standard 1.0).
        /// </summary>
        public static Task CompletedTask { get; }

        /// <summary>
        /// The Task.FromException polyfill for the netstandard 1.0 
        /// </summary>
        /// <param name="ex">The exception to wrap to the task.</param>
        /// <returns>The task, completed with the provided exception.</returns>
        public static Task FromException(Exception ex)
        {
#if  NETSTANDARD1_0 || NETSTANDARD1_1
            return FromExceptionHelper(ex).EnsureStarted();
#else
            return Task.FromException(ex);
#endif
        }

        private static async Task FromExceptionHelper(Exception ex)
        {
            throw ex;
        }
    }
}