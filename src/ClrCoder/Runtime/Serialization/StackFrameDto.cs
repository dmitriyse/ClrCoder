// <copyright file="StackFrameDto.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Runtime.Serialization
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using JetBrains.Annotations;

    using Newtonsoft.Json;

    /// <summary>
    /// Data transferring <c>object</c> for the <see cref="StackFrame"/>.
    /// </summary>
    [PublicAPI]
    public class StackFrameDto
    {
        /// <summary>
        /// Method information.
        /// </summary>
        [CanBeNull]
        public MethodInfoDto Method { get; set; }

        /// <summary>
        /// Offset from the start of the IL code for the
        /// method being executed.  This offset may be approximate depending
        /// on whether the jitter is generating debuggable code or not.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The same name from BCL")]
        public int ILOffset { get; set; }

        /// <summary>
        /// Returns the file name containing the code being executed.  This
        /// information is normally extracted from the debugging symbols
        /// for the executable.
        /// </summary>
        [CanBeNull]
        public string FileName { get; set; }

        /// <summary>
        /// Returns the line number in the file containing the code being executed.
        /// This information is normally extracted from the debugging symbols
        /// for the executable.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int FileLineNumber { get; set; }

        /// <summary>
        /// Returns the column number in the line containing the code being executed.
        /// This information is normally extracted from the debugging symbols
        /// for the executable.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int FileColumnNumber { get; set; }

        /// <summary>
        /// Converts <see cref="StackFrame"/> to DTO.
        /// </summary>
        /// <param name="stackFrame">Stack frame to convert.</param>
        /// <returns>Stack frame DTO.</returns>
        public static StackFrameDto FromStackFrame(StackFrame stackFrame)
        {
            var result = new StackFrameDto
                             {
                                 FileColumnNumber = stackFrame.GetFileColumnNumber(),
                                 FileLineNumber = stackFrame.GetFileLineNumber(),
                                 FileName = stackFrame.GetFileName(),
                                 ILOffset = stackFrame.GetILOffset(),
                             };
            MethodBase method = stackFrame.GetMethod();

            if (method != null)
            {
                result.Method = MethodInfoDto.FromMethodBase(method);
            }

            return result;
        }
    }
}