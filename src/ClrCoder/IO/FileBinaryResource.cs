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

    using JetBrains.Annotations;

#if NETSTANDARD2_0
    using Mono.Unix;
    using Mono.Unix.Native;
#endif

    using Validation;

    //// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global

    /// <summary>
    /// The local file binary resource.
    /// </summary>
    [PublicAPI]
    public class FileBinaryResource : IBinaryResource
    {
        //// ReSharper disable once IdentifierTypo
        private static readonly bool MmapAvailable = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                                                     || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        private readonly string _fullFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBinaryResource"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
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

        //// ReSharper disable once IdentifierTypo
        //// ReSharper disable once CommentTypo

        /// <summary>
        /// Allows to use mmap directly.
        /// </summary>
        public static bool AllowUseMmap { get; set; } = true;

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
            if (MmapAvailable && AllowUseMmap)
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
                        using (var handle = mmv.SafeMemoryMappedViewHandle)
                        {
                            IntPtr mmfDataPtr;
                            unsafe
                            {
                                byte* mmfData = (byte*)IntPtr.Zero;
                                handle.AcquirePointer(ref mmfData);
                                mmfDataPtr = (IntPtr)mmfData;
                            }

                            try
                            {
                                await readProc(mmfDataPtr, mmv.Capacity);
                            }
                            finally
                            {
                                handle.ReleasePointer();
                            }
                        }
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