// <copyright file="UPath.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    [Immutable]
    public class UPath
    {
        //// ReSharper disable once StringLiteralTypo
        private static readonly HashSet<char> ValidDriveChars =
            new HashSet<char>("ABCDEFGHIJKLMNOPQURSTUVWXYZ".ToCharArray());

        private readonly char? _drive;

        private readonly string _uniformPath;

        public UPath(string uniformPath)
        {
            if (uniformPath == null)
            {
                throw new ArgumentNullException(nameof(uniformPath));
            }

            uniformPath = uniformPath.Trim();

            if (uniformPath.Length >= 2 && uniformPath[1] == ':')
            {
                _drive = char.ToUpperInvariant(uniformPath[0]);

                if (!ValidDriveChars.Contains(_drive.Value))
                {
                    throw new UPathFormatException($"Invalid drive char: {_drive}");
                }

                uniformPath = uniformPath.Substring(2, uniformPath.Length - 2);
            }

            // TODO: Parse and validate.
            _uniformPath = uniformPath.Replace("\\", "/");
        }

        public string ToPlatformPath()
        {
            if (Path.DirectorySeparatorChar == '/')
            {
                return ToUnixPath();
            }

            return ToWindowsPath();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (_drive != null)
            {
                return $"{_drive}:{_uniformPath}";
            }

            return _uniformPath;
        }

        public string ToUnixPath()
        {
            // TODO: Remove drive path.
            return _uniformPath;
        }

        public string ToWindowsPath()
        {
            if (_drive != null)
            {
                return $"{_drive}:{_uniformPath.Replace("/", "\\")}";
            }

            if (_uniformPath.StartsWith("/"))
            {
                return $"C:{_uniformPath.Replace("/", "\\")}";
            }

            return _uniformPath.Replace("/", "\\");
        }
    }
}