// <copyright file="MethodInfoDto.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Runtime.Serialization
{
    using System;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// Data transferring <c>object</c> for the <see cref="MemberInfo"/> type.
    /// </summary>
    public class MethodInfoDto
    {
        /// <summary>
        /// Method name.
        /// </summary>
        [CanBeNull]
        public string Name { get; set; }

        /// <summary>
        /// Type full name.
        /// </summary>
        [CanBeNull]
        public string TypeFullName { get; set; }

        /// <summary>
        /// Converts <see cref="MethodBase"/> to DTO.
        /// </summary>
        /// <param name="methodInfo">Method base to convert.</param>
        /// <returns>Method info DTO.</returns>
        public static MethodInfoDto FromMethodBase(MethodBase methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            // ReSharper disable once ConstantConditionalAccessQualifier
            return new MethodInfoDto
                       {
                           Name = methodInfo.Name,
                           TypeFullName = methodInfo.DeclaringType?.FullName
                       };
        }
    }
}