// <copyright file="RawVideoStreamFormat.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if !NETSTANDARD1_0
namespace ClrCoder.Imaging.Raw
{
    using System;
    using System.Drawing;

    /// <summary>
    /// The format of the raw video stream.
    /// </summary>
    public class RawVideoStreamFormat : IEquatable<RawVideoStreamFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawVideoStreamFormat"/> class.
        /// </summary>
        /// <param name="imagePixelFormat">The pixel format of frames.</param>
        /// <param name="resolution">The raw video resolution.</param>
        /// <param name="fps">The frames per second for equidistant video stream or null for others.</param>
        public RawVideoStreamFormat(RawPixelFormatCode imagePixelFormat, Size resolution, double? fps)
        {
            ImagePixelFormat = imagePixelFormat;
            Resolution = resolution;
            Fps = fps;
        }

        /// <summary>
        /// The pixel format of frames.
        /// </summary>
        public RawPixelFormatCode ImagePixelFormat { get; }

        /// <summary>
        /// The raw video resolution.
        /// </summary>
        public Size Resolution { get; }

        /// <summary>
        /// The frames per second for equidistant video stream or null for others.
        /// </summary>
        public double? Fps { get; }

        /// <inheritdoc/>
        public bool Equals(RawVideoStreamFormat other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (ImagePixelFormat == other.ImagePixelFormat) && Resolution.Equals(other.Resolution)
                                                                && Fps.Equals(other.Fps);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
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

            return Equals((RawVideoStreamFormat)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)ImagePixelFormat;
                hashCode = (hashCode * 397) ^ Resolution.GetHashCode();
                hashCode = (hashCode * 397) ^ Fps.GetHashCode();
                return hashCode;
            }
        }
    }
}
#endif