// <copyright file="UnmanagedMemoryResource.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Validation;

    /// <summary>
    /// Represents unmanaged buffer as binary resource.
    /// </summary>
    public class UnmanagedMemoryResource : IBinaryResource
    {
        private readonly IntPtr _bufferPtr;

        private readonly long _length;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnmanagedMemoryResource"/> class.
        /// </summary>
        /// <param name="bufferPtr">The unmanaged memory buffer pointer.</param>
        /// <param name="length">The length of unmanaged memory buffer.</param>
        public UnmanagedMemoryResource(IntPtr bufferPtr, long length)
        {
            if (bufferPtr == IntPtr.Zero)
            {
                throw new ArgumentException("Buffer pointer should not be NULL.", nameof(bufferPtr));
            }

            if (length < 0)
            {
                throw new ArgumentException("Length should be positive.", nameof(length));
            }

            _bufferPtr = bufferPtr;
            _length = length;
        }

        /// <inheritdoc/>
        public ValueTask<(byte[], int)> ReadIntoBuffer(bool allowCaching = true, bool exactSize = false)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ReadFromFile(Func<string, Task> readProc)
        {
            VxArgs.NotNull(readProc, nameof(readProc));

            // TODO: Temporary file creation is required.
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task ReadFromMemory(Func<IntPtr, long, Task> readProc)
        {
            VxArgs.NotNull(readProc, nameof(readProc));
            await readProc(_bufferPtr, _length);
        }

        /// <inheritdoc/>
        public async Task ReadFromStream(Func<Stream, Task> readProc)
        {
            VxArgs.NotNull(readProc, nameof(readProc));

            await ReadOrWriteStreamInternal(readProc);
        }

        /// <inheritdoc/>
        public Task WriteToFile(Func<string, Task> writeProc)
        {
            // TODO: Temporary file creation required.
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task WriteToStream(Func<Stream, Task> writeProc)
        {
            VxArgs.NotNull(writeProc, nameof(writeProc));

            await ReadOrWriteStreamInternal(writeProc);
        }

        private async Task ReadOrWriteStreamInternal(Func<Stream, Task> readProc)
        {
#if !NETSTANDARD1_0 && !NETSTANDARD1_1
            Stream stream;
            unsafe
            {
                stream = new UnmanagedMemoryStream((byte*)_bufferPtr, _length);
            }

            using (stream)
            {
                await readProc(stream);
            }

#else
            throw new NotSupportedException();
#endif
        }
    }
}