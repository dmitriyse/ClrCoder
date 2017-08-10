// <copyright file="IAsyncDisposable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Threading
{
    using Tasks;

    /// <summary>
    /// Asynchronous disposable. Contract, required for the future "await using" operator (Probably C# 8.0).
    /// https://github.com/dotnet/roslyn/issues/114
    /// https://github.com/dotnet/csharplang/issues/43
    /// </summary>
    public interface IAsyncDisposable
    {
        /// <summary>
        /// Initiates async disposing, allowed to be called multiple times. Should never <see langword="throw"/> an exception.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        Task DisposeAsync();
    }
}