// <copyright file="IContainerNodeConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel
{
    using ObjectModel;

    /// <summary>
    /// <c>Container</c> node configuration.
    /// </summary>
    /// <remarks> Should be immutable and cloneable. </remarks>
    public interface IContainerNodeConfig : IDeepCloneable<IContainerNodeConfig>, IFreezable
    {
    }
}