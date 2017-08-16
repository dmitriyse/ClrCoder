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
        static TaskEx()
        {
            CompletedTask = ((Func<Task>)(async () => { }))();
        }

        /// <summary>
        /// Alternative to Task.CompletedTask (for .Net Standard 1.0).
        /// </summary>
        public static Task CompletedTask { get; }
    }
}