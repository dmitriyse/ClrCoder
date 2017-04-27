// <copyright file="UPathFormatException.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;

    public class UPathFormatException : FormatException
    {
        public UPathFormatException()
        {
        }

        public UPathFormatException(string message) : base(message)
        {
        }

        public UPathFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}