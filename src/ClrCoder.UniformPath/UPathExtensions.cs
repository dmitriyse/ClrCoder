// <copyright file="UPathExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
    using System.IO;

    /// <summary>
    /// UPath related extension methods.
    /// </summary>
    public static class UPathExtensions
    {
        public static string Extension(this UPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            // TODO: Create valid regex pattern.
            return Path.GetExtension(path.ToString()).Remove(0, 1);
        }

        public static bool FileExists(this UPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return File.Exists(path.ToString());
        }

        public static bool IsAbsolute(this UPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return path.Path.StartsWith("/");
        }

        public static UPath Normalize(this UPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return new UPath(Path.GetFullPath(path.ToString()));
        }

        public static UPath ParentDir(this UPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return new UPath(Path.GetDirectoryName(path.ToString()));
        }

        public static string FileNameWithoutExtension(this UPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return Path.GetFileNameWithoutExtension(path.ToString());
        }
    }
}