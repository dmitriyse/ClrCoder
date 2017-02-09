// <copyright file="FileAppenderLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    using System;
    using System.IO;

    using JetBrains.Annotations;

    using Text;

    using Threading;

    /// <summary>
    /// Appends log entries to a file.
    /// </summary>
    public class FileAppenderLogger : IJsonLogger
    {
        [NotNull]
        private readonly string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAppenderLogger"/> class.
        /// </summary>
        /// <param name="fileName">File name to append log entries to.</param>
        /// <param name="asyncHandler">Asynchronous handler for logs processing.</param>
        public FileAppenderLogger([NotNull] string fileName, [NotNull] IAsyncHandler asyncHandler)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            _fileName = fileName;
            AsyncHandler = asyncHandler;
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler { get; }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            var entryString = entry as string;
            if (entryString != null)
            {
                if (entryString.Contains("\n"))
                {
                    throw new NotSupportedException("Serialized json strings should be formatted in a one line.");
                }

                File.AppendAllLines(_fileName, new[] { entryString }, EncodingEx.UTF8NoBom);
            }
            else
            {
                throw new NotSupportedException("Log entries should be serialized.");
            }
        }
    }
}