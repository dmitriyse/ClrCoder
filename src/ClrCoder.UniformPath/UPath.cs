// <copyright file="UPath.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// The uniform path. Linux path in general, plus special form like "D:/some/path" for windows.
    /// </summary>
    [Immutable]
    public class UPath : IEquatable<UPath>
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

        /// <summary>
        /// Compares equality of two paths.
        /// </summary>
        /// <param name="left">Left equality comparison operand.</param>
        /// <param name="right">Right equality comparison operand.</param>
        /// <returns><see langword="true"/>, if operands are equals, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(UPath left, UPath right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares inequality of two paths.
        /// </summary>
        /// <param name="left">Left inequality comparison operand.</param>
        /// <param name="right">Right inequality comparison operand.</param>
        /// <returns><see langword="true"/>, if operand are unequal, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(UPath left, UPath right)
        {
            return !Equals(left, right);
        }

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

        /// <inheritdoc/>
        public bool Equals([CanBeNull] UPath other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (_drive == other._drive) && string.Equals(_uniformPath, other._uniformPath);
        }

        /// <inheritdoc/>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((UPath)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (_drive.GetHashCode() * 397) ^ _uniformPath.GetHashCode();
            }
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

        public UPath Combine(UPath joinPart)
        {
            if (joinPart == null)
            {
                throw new ArgumentNullException(nameof(joinPart));
            }

            if (joinPart.IsAbsolute())
            {
                return joinPart;
            }
            else
            {
                return new UPath(System.IO.Path.Combine(_uniformPath, joinPart.Path), _drive);
            }
        }

    }
}