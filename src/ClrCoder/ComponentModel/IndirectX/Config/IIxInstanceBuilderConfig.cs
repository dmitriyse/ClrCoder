// <copyright file="IIxInstanceBuilderConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    /// <summary>
    /// Instance builder configuration contract.
    /// </summary>
    /// <remarks>
    /// Instance Builder - constructs some how instance, may require dependencies, do some proxying.
    /// Instance Factory - is just async method that performs instantiation using already prepared instance prerequisites.
    /// </remarks>
    public interface IIxInstanceBuilderConfig
    {
    }
}