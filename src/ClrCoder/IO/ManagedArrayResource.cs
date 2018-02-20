// <copyright file="ManagedArrayResource.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.IO
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Validation;

    /// <summary>
    /// Represents managed array as a binary resource.
    /// This class can be improved after C# 7.3 (see https://github.com/dotnet/csharplang/issues/187).
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    public class ManagedArrayResource<T> : IBinaryResource
        where T : struct
    {
        private readonly Func<UnmanagedMemoryAsyncAction, Task> _unmanagedAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedArrayResource{T}"/> class.
        /// </summary>
        /// <param name="array">The array to represent as a resource.</param>
        /// <param name="unmanagedAccessor">The accessor to the unmanaged memory, parameters</param>
        internal ManagedArrayResource(T[] array, Func<UnmanagedMemoryAsyncAction, Task> unmanagedAccessor)
        {
            VxArgs.NotNull(array, nameof(array));
            VxArgs.NotNull(unmanagedAccessor, nameof(array));

            Array = array;
            _unmanagedAccessor = unmanagedAccessor;
        }

        /// <summary>
        /// The underlying array.
        /// </summary>
        public T[] Array { get; }

        /// <inheritdoc/>
        public ValueTask<(byte[], int)> ReadIntoBuffer(bool allowCaching = true, bool exactSize = false)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ReadFromFile(Func<string, Task> readProc)
        {
            // TODO: Implement temporary file access.
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task ReadFromMemory(Func<IntPtr, long, Task> readProc)
        {
            await _unmanagedAccessor(
                async (bufferPtr, bufferLength) => { await readProc(bufferPtr, bufferLength); });
        }

        /// <inheritdoc/>
        public async Task ReadFromStream(Func<Stream, Task> readProc)
        {
            VxArgs.NotNull(readProc, nameof(readProc));
            await ReadWriteStream(readProc);
        }

        /// <inheritdoc/>
        public Task WriteToFile(Func<string, Task> writeProc)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task WriteToStream(Func<Stream, Task> writeProc)
        {
            VxArgs.NotNull(writeProc, nameof(writeProc));
            await ReadWriteStream(writeProc);
        }

        private async Task ReadWriteStream(Func<Stream, Task> readOrWriteProc)
        {
#if !NETSTANDARD1_0 && !NETSTANDARD1_1
            await _unmanagedAccessor(
                async (bufferPtr, bufferLength) =>
                    {
                        Stream stream;
                        unsafe
                        {
                            stream = new UnmanagedMemoryStream((byte*)bufferPtr, bufferLength);
                        }

                        using (stream)
                        {
                            await readOrWriteProc(stream);
                        }
                    });
#else
            throw new NotSupportedException();
#endif
        }
    }

    /// <summary>
    /// Async action on an unmanaged buffer.
    /// </summary>
    /// <param name="bufferPtr">The unmanaged buffer pointer.</param>
    /// <param name="bufferLength">The length of the unmanaged buffer.</param>
    /// <returns>The async execution TPL task.</returns>
    public delegate Task UnmanagedMemoryAsyncAction(IntPtr bufferPtr, long bufferLength);

    //// ReSharper disable once StyleCop.SA1402

    /// <summary>
    /// Managed array resource constructors.
    /// </summary>
    [PublicAPI]
    public static class ManagedArrayResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedArrayResource{Byte}"/> class.
        /// </summary>
        /// <param name="array">The array to represent as a binary resource.</param>
        /// <returns>The binary resource instance.</returns>
        public static ManagedArrayResource<byte> CreateByteArrayResource(byte[] array)
        {
            return new ManagedArrayResource<byte>(
                array,
                async unmanagedMemoryAsyncAction =>
                    {
                        VxArgs.NotNull(unmanagedMemoryAsyncAction, "item1");
                        unsafe
                        {
                            fixed (byte* bufferPtr = array)
                            {
                                unmanagedMemoryAsyncAction((IntPtr)bufferPtr, array.Length).ConfigureAwait(false)
                                    .GetAwaiter().GetResult();
                            }
                        }
                    });
        }
    }
}