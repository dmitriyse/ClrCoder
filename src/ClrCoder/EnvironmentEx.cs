// <copyright file="EnvironmentEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder
{
    using JetBrains.Annotations;

    /// <summary>
    /// Extended environment functions.
    /// </summary>
    [PublicAPI]
    public static class EnvironmentEx
    {
        private static bool _isInitialized = false;

        //// ReSharper disable once InconsistentNaming

        /// <summary>
        /// Checks if application executed under Linux OS.
        /// </summary>
        public static OSFamilyTypes OSFamily { get; private set; }

#if PCL

        /// <summary>
        /// Initializes environment.
        /// </summary>
        /// <param name="osFamily">Specified OS family.</param>
        public static void InitEnvironment(OSFamilyTypes? osFamily)
        {
            if (_isInitialized)
            {
                return;
            }

            OSFamily = osFamily ?? default(OSFamilyTypes);
        }

#endif
    }
}