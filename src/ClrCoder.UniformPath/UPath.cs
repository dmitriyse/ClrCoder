// <copyright file="UPath.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
    using System.Collections.Generic;

    [Immutable]
    public class UPath
    {
        //// ReSharper disable once StringLiteralTypo
        private static readonly HashSet<char> ValidDriveChars =
            new HashSet<char>("ABCDEFGHIJKLMNOPQURSTUVWXYZ".ToCharArray());

        private readonly char? _drive;

        private readonly string _uniformPath;

        public UPath(string path, UPathParseMode parseMode = UPathParseMode.AllowIncorrectFormat)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            ParseUPathInternal(path, parseMode, true, out _drive, out _uniformPath);
        }

        private UPath(string uniformPath, char? drive)
        {
            _uniformPath = uniformPath;
            _drive = drive;
        }

        public char? Drive => _drive;

        public string Path => _uniformPath;

        public static bool TryParse(string path, UPathParseMode parseMode, out UPath uPath)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (ParseUPathInternal(path, parseMode, false, out char? drive, out string uniformPath))
            {
                uPath = new UPath(uniformPath, drive);
                return true;
            }

            uPath = null;
            return false;
        }

        private static bool ParseUPathInternal(
            string path,
            UPathParseMode parseMode,
            bool throwFormatException,
            out char? drive,
            out string uniformPath)
        {
            path = path.Trim();
            drive = null;

            if ((path.Length >= 2) && (path[1] == ':'))
            {
                drive = char.ToUpperInvariant(path[0]);

                if (!ValidDriveChars.Contains(drive.Value))
                {
                    if (throwFormatException)
                    {
                        throw new UPathFormatException($"Invalid drive char: {drive}");
                    }

                    uniformPath = null;
                    return false;
                }

                path = path.Substring(2, path.Length - 2);
            }

            // TODO: Parse and validate.
            uniformPath = path.Replace("\\", "/");
            return true;
        }

        public string ToPlatformPath()
        {
            if (System.IO.Path.DirectorySeparatorChar == '/')
            {
                return ToUnixPath();
            }

            return ToWindowsPath();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Drive != null)
            {
                return $"{Drive}:{Path}";
            }

            return Path;
        }

        public string ToUnixPath()
        {
            // TODO: Remove drive path.
            return Path;
        }

        public string ToWindowsPath()
        {
            if (Drive != null)
            {
                return $"{Drive}:{Path.Replace("/", "\\")}";
            }

            if (Path.StartsWith("/"))
            {
                return $"C:{Path.Replace("/", "\\")}";
            }

            return Path.Replace("/", "\\");
        }
    }
}