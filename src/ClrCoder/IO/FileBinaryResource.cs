// <copyright file="FileBinaryResource.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0 && !NETSTANDARD1_1
namespace ClrCoder.IO
{
    using System;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

#if NETSTANDARD2_0
    using Mono.Unix;
    using Mono.Unix.Native;
#endif

    using Validation;

    /// <summary>
    /// The local file binary resource.
    /// </summary>
    public class FileBinaryResource : IBinaryResource
    {
        private static readonly bool UseSyscall = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        private readonly string _fullFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBinaryResource"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="useCopyForMemoryAccess">Shows that ReadMemory should be performed through copy file content to the memory.</param>
        public FileBinaryResource(UPath filePath)
        {
            VxArgs.NotNull(filePath, nameof(filePath));

            _fullFilePath = filePath.Normalize().ToPlatformPath();

            if (!filePath.FileExists())
            {
                string platformFilePath = filePath.ToPlatformPath();
                throw new FileNotFoundException($"{platformFilePath} does not exists.", platformFilePath);
            }
        }

        /// <inheritdoc/>
        public virtual Task ReadFromFile(Func<string, Task> readProc)
        {
            VxArgs.NotNull(readProc, nameof(readProc));
            return readProc(_fullFilePath);
        }

        /// <inheritdoc/>
        public virtual async Task ReadFromMemory(Func<IntPtr, long, Task> readProc)
        {
            VxArgs.NotNull(readProc, nameof(readProc));
#if NETSTANDARD2_0
            if (UseSyscall)
            {
                var fileId = Syscall.open(_fullFilePath, OpenFlags.O_RDONLY);
                VerifyUnixSuccess(fileId);

                VerifyUnixSuccess(Syscall.fstat(fileId, out var sb));
                long size = sb.st_size;

                try
                {
                    // ReSharper disable once IdentifierTypo
                    IntPtr mmapPtr = Syscall.mmap(
                        IntPtr.Zero,
                        (ulong)size,
                        MmapProts.PROT_READ,
                        MmapFlags.MAP_PRIVATE | MmapFlags.MAP_POPULATE,
                        fileId,
                        0);

                    if (mmapPtr == (IntPtr)(-1))
                    {
                        throw new UnixIOException();
                    }

                    try
                    {
                        await readProc(mmapPtr, size);
                    }
                    finally
                    {
                        Syscall.munmap(mmapPtr, (ulong)size);
                    }
                }
                finally
                {
                    Syscall.close(fileId);
                }
            }
            else
#endif
            {
                using (var mmf = MemoryMappedFile.CreateFromFile(_fullFilePath, FileMode.Open))
                {
                    using (var mmv = mmf.CreateViewAccessor(0L, 0L, MemoryMappedFileAccess.Read))
                    {
                        IntPtr mmfDataPtr;
                        unsafe
                        {
                            byte* mmfData = (byte*)IntPtr.Zero;
                            mmv.SafeMemoryMappedViewHandle.AcquirePointer(ref mmfData);
                            mmfDataPtr = (IntPtr)mmfData;
                        }

                        await readProc(mmfDataPtr, mmv.Capacity);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public virtual async Task ReadFromStream(Func<Stream, Task> readProc)
        {
            VxArgs.NotNull(readProc, nameof(readProc));

            using (var fileStream = File.OpenRead(_fullFilePath))
            {
                await readProc(fileStream);
            }
        }

        /// <inheritdoc/>
        public async Task WriteToFile(Func<string, Task> writeProc)
        {
            await writeProc(_fullFilePath);
        }

        /// <inheritdoc/>
        public async Task WriteToStream(Func<Stream, Task> writeProc)
        {
            VxArgs.NotNull(writeProc, nameof(writeProc));

            using (var fileStream = File.OpenRead(_fullFilePath))
            {
                await writeProc(fileStream);
            }
        }

#if NETSTANDARD2_0
        private void VerifyUnixSuccess(int result)
        {
            if (result == -1)
            {
                throw new UnixIOException();
            }
        }

#endif
    }
}
#endif