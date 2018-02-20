// <copyright file="IBinaryResource.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// The binary resource abstraction.
    /// </summary>
    [PublicAPI]
    public interface IBinaryResource
    {
        /// <summary>
        /// Provides read access to the binary resource through a file.
        /// </summary>
        /// <param name="readProc">The read proc.</param>
        /// <returns>The async execution TPL task.</returns>
        Task ReadFromFile(Func<string, Task> readProc);

        /// <summary>
        /// Provides read access to the binary resource through unmanaged memory.
        /// </summary>
        /// <param name="readProc">The read proc.</param>
        /// <returns>The async execution TPL task.</returns>
        Task ReadFromMemory(Func<IntPtr, long, Task> readProc);

        /// <summary>
        /// Provides read access to the binary resource through the .Net stream abstraction.
        /// </summary>
        /// <param name="readProc">The read proc.</param>
        /// <returns>The async execution TPL task.</returns>
        Task ReadFromStream(Func<Stream, Task> readProc);

        /// <summary>
        /// Provides write access to the binary resource through the file.
        /// It's possible that the file with the provided name does not exists yet.
        /// </summary>
        /// <param name="writeProc">The write proc.</param>
        /// <returns>The async execution TPL task.</returns>
        Task WriteToFile(Func<string, Task> writeProc);

        /// <summary>
        /// Provides write access to the binary resource through the .Net stream abstraction.
        /// </summary>
        /// <param name="writeProc">The write proc.</param>
        /// <returns>The async execution TPL task.</returns>
        Task WriteToStream(Func<Stream, Task> writeProc);
    }
}