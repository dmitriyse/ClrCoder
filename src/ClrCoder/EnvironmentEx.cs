// <copyright file="EnvironmentEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
    using System.IO;

    using JetBrains.Annotations;

    /// <summary>
    /// Extended environment functions.
    /// </summary>
    [PublicAPI]
    public static class EnvironmentEx
    {
        /// <summary>
        /// Initializes static members of the <see cref="EnvironmentEx"/> class.
        /// </summary>
        static EnvironmentEx()
        {
            if (Path.DirectorySeparatorChar == '\\')
            {
                OSFamily = OSFamilyTypes.Windows;
            }
            else
            {
                // TOOD: Add support for different families.
                OSFamily = OSFamilyTypes.Linux;
            }
        }

        /// <summary>
        /// Directory with binary files.
        /// </summary>
        public static string BinPath => AppContext.BaseDirectory;

        /// <summary>
        /// Checks if we are running in a mono runtime.
        /// </summary>
        public static bool IsMonoRuntime => Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Maximal directory name length for the current environment.
        /// </summary>
        /// <remarks>TODO: Test linux.</remarks>
        public static int MaxDirectoryNameLength
        {
            get
            {
#if NET46
                return 244;
#else
                return 255;
#endif
            }
        }

        /// <summary>
        /// Maximal file name length for the current environment.
        /// </summary>
        /// <remarks>TODO: Test linux.</remarks>
        public static int MaxFileNameLength
        {
            get
            {
#if NET46
                return 197;
#else
                return 255;
#endif
            }
        }

        /// <summary>
        /// Maximal path length for the current environment.
        /// </summary>
        public static int MaxPathLength
        {
            get
            {
#if NET46
                return 247;
#else
                return (OSFamily & OSFamilyTypes.Posix) != 0 ? 4095 : 32739;
#endif
            }
        }

        //// ReSharper disable once InconsistentNaming

        /// <summary>
        /// Checks if application executed under Linux OS.
        /// </summary>
        public static OSFamilyTypes OSFamily { get; private set; }

        /// <summary>
        /// Gets filesystem root directory.
        /// </summary>
        /// <returns>For windows: "%SystemDrive%\", for posix = "/".</returns>
        public static string GetFileSystemRoot()
        {
            string result;
            if (OSFamily.HasFlag(OSFamilyTypes.Windows))
            {
                string systemDrive = Environment.GetEnvironmentVariable("SystemDrive");
                if (string.IsNullOrWhiteSpace(systemDrive))
                {
                    result = "C:\\";
                }
                else
                {
                    result = systemDrive;
                    if (result.Length == 2)
                    {
                        result += "\\";
                    }
                }
            }
            else
            {
                return "/";
            }

            return result;
        }
    }
}