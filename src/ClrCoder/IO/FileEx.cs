// <copyright file="FileEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.IO
{
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using JetBrains.Annotations;

    /// <summary>
    /// File related utility methods.
    /// </summary>
    [PublicAPI]
    public class FileEx
    {
        /// <summary>Creates a symbolic link using command line tools</summary>
        /// <param name="linkPath">The existing file</param>
        /// <param name="targetPath"></param>
        /// <param name="isDirectory"></param>
        public static bool CreateSymbolicLink(string linkPath, string targetPath, bool isDirectory)
        {
            var symLinkProcess = new Process();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                symLinkProcess.StartInfo.FileName = "cmd";
                symLinkProcess.StartInfo.Arguments = string.Format(
                    "/c mklink{0} \"{1}\" \"{2}\"",
                    isDirectory ? " /D" : "",
                    linkPath,
                    targetPath);
            }
            else
            {
                symLinkProcess.StartInfo.FileName = "/bin/ln";
                symLinkProcess.StartInfo.Arguments = string.Format("-s \"{0}\" \"{1}\"", targetPath, linkPath);
            }
            symLinkProcess.StartInfo.UseShellExecute = false;
            symLinkProcess.StartInfo.RedirectStandardOutput = true;
            symLinkProcess.Start();

            if (symLinkProcess != null)
            {
                symLinkProcess.WaitForExit();
                return 0 == symLinkProcess.ExitCode;
            }
            else
            {
                return false;
            }
        }
    }
#endif
}