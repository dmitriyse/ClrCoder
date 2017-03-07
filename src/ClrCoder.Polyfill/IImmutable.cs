// <copyright file="IImmutable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IImmutable<T>
    {
        bool IsImmutable { get; }

        bool IsShallowImmutable { get; }
    }
}