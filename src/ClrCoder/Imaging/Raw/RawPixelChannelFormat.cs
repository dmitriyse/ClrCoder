// <copyright file="RawPixelChannelFormat.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Imaging.Raw
{
    /// <summary>
    /// The raw pixel channel format description.
    /// </summary>
    public class RawPixelChannelFormat
    {
        public RawPixelChannelFormat(
            RawPixelChannelType channelType,
            RawPixelChannelValuesFormat valueFormat,
            int bitsOffset,
            int bitsPerChannel)
        {
            ChannelType = channelType;
            ValueFromat = valueFormat;
            BitsOffset = bitsOffset;
            BitsPerChannel = bitsPerChannel;
        }

        public RawPixelChannelType ChannelType { get; }

        public int BitsPerChannel { get; }

        public int BitsOffset { get; }

        public RawPixelChannelValuesFormat ValueFromat { get; }
    }
}