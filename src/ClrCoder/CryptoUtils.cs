// <copyright file="CryptoUtils.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder
{
    using System;
    using System.Security.Cryptography;

    using JetBrains.Annotations;

    /// <summary>
    /// Cryptography utils.
    /// </summary>
    public static class CryptoUtils
    {
        [ThreadStatic]
        private static Md5HashTheadVariables _md5Variables;

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
                _md5Variables = new Md5HashTheadVariables();
            }

            var strBytes = EncodingEx.UnicodeNoBom.GetBytes(str);
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
                _md5Variables = new Md5HashTheadVariables();
            }

            return new Guid(_md5Variables.Md5.ComputeHash(data));
        }

        private class Md5HashTheadVariables
        {
            public Md5HashTheadVariables()
            {
                Md5 = MD5.Create();
                Md5.Initialize();
            }

            public MD5 Md5 { get; private set; }
        }
    }
}