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
#if PCL
        private static bool _isInitialized = false;

#else

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

#endif

        //// ReSharper disable once InconsistentNaming

        /// <summary>
        /// Checks if application executed under Linux OS.
        /// </summary>
        public static OSFamilyTypes OSFamily { get; private set; }

#if !PCL

        /// <summary>
        /// Directory with binary files.
        /// </summary>
        public static string BinPath => AppContext.BaseDirectory;

        /// <summary>
        /// Checks that mono runtime.
        /// </summary>
        public static bool IsMonoRuntime => Type.GetType("Mono.Runtime") != null;
#endif
#if PCL

/// <summary>
/// Initializes environment.
/// </summary>
/// <param name="osFamily">Specified OS family.</param>
        public static void InitEnvironment(OSFamilyTypes? osFamily)
        {
            if (_isInitialized)
            {
                return;
            }

            OSFamily = osFamily ?? default(OSFamilyTypes);
        }
#endif
    }
}