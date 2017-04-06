// <copyright file="IxHost.IxResolveContext.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <content>Resolve context implementation.</content>
    public partial class IxHost
    {
        /// <summary>
        /// Resolve context.
        /// </summary>
        public class IxResolveContext
        {
            [CanBeNull]
            private readonly IxResolveContext _parentContext;

            private readonly IxResolveContext _rootContext;

            public IxResolveContext(
                [CanBeNull] IxResolveContext parentContext,
                IReadOnlyDictionary<IxIdentifier, object> arguments)
            {
                Arguments = arguments;
                _parentContext = parentContext;
                _rootContext = parentContext ?? this;
            }

            [NotNull]
            public IReadOnlyDictionary<IxIdentifier, object> Arguments { get; }
        }
    }
}