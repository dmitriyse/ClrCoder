// <copyright file="IxResolveFrame.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using JetBrains.Annotations;

    /// <summary>
    /// The resolve frame in the dependency resolution sequence.
    /// </summary>
    public class IxResolveFrame
    {
        public IxResolveFrame(
            [CanBeNull] IxResolveFrame parentFrame,
            IIxInstance instance)
        {
            ParentFrame = parentFrame;
            Instance = instance;
        }

        public IxResolveFrame ParentFrame { get; }

        public IIxInstance Instance { get; }
    }
}