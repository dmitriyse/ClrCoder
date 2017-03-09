// <copyright file="PathEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.IO
{
    using System;
    using System.IO;

    using JetBrains.Annotations;

    /// <summary>
    /// Path related utility methods.
    /// </summary>
    [PublicAPI]
    public static class PathEx
    {
        /// <summary>
        /// Transforms posix path to current OSFamily type (windows/unix) path.
        /// </summary>
        /// <param name="posixPath">Posix path (relative or absolute).</param>
        /// <returns>
        /// The same string for posix OS. For windows it uses the same transform rule as Windows. Slash will be changed
        /// in a relative path and current drive will be used to replace posix root.
        /// </returns>
        [NotNull]
        public static string PosixToPlatformPath([NotNull] string posixPath)
        {
            if (posixPath == null)
            {
                throw new ArgumentNullException(nameof(posixPath));
            }

            if (EnvironmentEx.OSFamily != OSFamilyTypes.Windows)
            {
                return posixPath;
            }

            string trimmedPath = posixPath.Trim();
            string resultPath;

            if (trimmedPath.StartsWith("/"))
            {
                trimmedPath = trimmedPath.TrimStart('/');

                string rootPath = Path.GetPathRoot(Path.GetFullPath(Directory.GetCurrentDirectory()));

                resultPath = rootPath + trimmedPath.Replace("/", "\\");
            }
            else
            {
                resultPath = trimmedPath.Replace("/", "\\");
            }

            return resultPath;
        }
    }
}