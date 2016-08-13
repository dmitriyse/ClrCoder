// <copyright file="CallerInfo.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder
{
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// Aggregates caller information provided by compiler. See <see cref="CallerFilePathAttribute"/>.
    /// </summary>
    [PublicAPI]
    public struct CallerInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallerInfo"/> struct.
        /// </summary>
        /// <param name="filePath">Caller file path.</param>
        /// <param name="memberName">Caller member name.</param>
        /// <param name="lineNumber">Caller line number.</param>
        public CallerInfo(string filePath, string memberName, int lineNumber)
        {
            FilePath = filePath;
            MemberName = memberName;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Caller file path.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Caller member name.
        /// </summary>
        public string MemberName { get; private set; }

        /// <summary>
        /// Caller line number.
        /// </summary>
        public int LineNumber { get; private set; }
    }
}