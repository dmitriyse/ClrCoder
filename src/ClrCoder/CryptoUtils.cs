// <copyright file="CryptoUtils.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System;
    using System.Security.Cryptography;

    using JetBrains.Annotations;

    using Text;

    /// <summary>
    /// Cryptography utils.
    /// </summary>
    public static class CryptoUtils
    {
        [ThreadStatic]
        private static Md5HashThreadVariables _md5Variables;

        /// <summary>
        /// Calculates MD5 hash for a UTF-16(no BOM) representation of the specified string.
        /// </summary>
        /// <param name="str">String to calculate hash.</param>
        /// <returns>MD5 hash stored in a Guid.</returns>
        [PublicAPI]
        public static Guid Md5Hash(this string str)
        {
            if (str == null)
            {
                return Guid.Empty;
            }

            if (_md5Variables == null)
            {
                _md5Variables = new Md5HashThreadVariables();
            }

            byte[] strBytes = EncodingEx.UnicodeNoBom.GetBytes(str);
            return new Guid(_md5Variables.Md5.ComputeHash(strBytes));
        }

        /// <summary>
        /// Calculates MD5 hash for the provided binary data.
        /// </summary>
        /// <param name="data">Binary data.</param>
        /// <returns>MD5 hash stored in a Guid.</returns>
        [PublicAPI]
        public static Guid Md5Hash(this byte[] data)
        {
            if (data == null)
            {
                return Guid.Empty;
            }

            if (_md5Variables == null)
            {
                _md5Variables = new Md5HashThreadVariables();
            }

            return new Guid(_md5Variables.Md5.ComputeHash(data));
        }

        /// <summary>
        /// Calculates MD5 hash for the provided string and converts data to base64 string.
        /// </summary>
        /// <param name="str">The string to calculate hash.</param>
        /// <returns>MD5 hash encoded in the base 64.</returns>
        [PublicAPI]
        public static string Md5HashBase64([CanBeNull]this string str)
        {
            if (str == null)
            {
                var zeroBytes = new byte[16];
                return Convert.ToBase64String(zeroBytes);
            }

            if (_md5Variables == null)
            {
                _md5Variables = new Md5HashThreadVariables();
            }

            byte[] strBytes = EncodingEx.UnicodeNoBom.GetBytes(str);
            byte[] hashBytes = _md5Variables.Md5.ComputeHash(strBytes);
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Calculates MD5 hash for the provided binary data.
        /// </summary>
        /// <param name="data">Binary data.</param>
        /// <returns>MD5 hash stored in the array.</returns>
        [PublicAPI]
        public static byte[] Md5HashBytes(this byte[] data)
        {
            if (data == null)
            {
                return new byte[8];
            }

            if (_md5Variables == null)
            {
                _md5Variables = new Md5HashThreadVariables();
            }

            return _md5Variables.Md5.ComputeHash(data);
        }

        private class Md5HashThreadVariables
        {
            public Md5HashThreadVariables()
            {
                Md5 = MD5.Create();
                Md5.Initialize();
            }

            public MD5 Md5 { get; private set; }
        }
    }
#endif
}