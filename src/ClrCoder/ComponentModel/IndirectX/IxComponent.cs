// <copyright file="IxComponent.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using JetBrains.Annotations;

    public class IxComponent : IxScope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxComponent"/> class.
        /// </summary>
        /// <param name="host"></param>
        public IxComponent(
            IxHost host,
            [CanBeNull] IxScopeBase parentNode,
            IxComponentConfig config)
            : base(host, parentNode, config)
        {
        }
    }
}