// <copyright file="IxArgumentProvider.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
#pragma warning disable 1998
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Threading;

    /// <summary>
    /// Provider node emulator for special instances (<see cref="IxArgumentInstance"/>) that wraps resolve arguments.
    /// </summary>
    public class IxArgumentProvider : IxProviderNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxArgumentProvider"/> class.
        /// </summary>
        /// <param name="host">IndirectX host.</param>
        public IxArgumentProvider(IxHost host)
            : base(
                host,
                null,
                new IxStdProviderConfig
                    {
                        Identifier = new IxIdentifier(typeof(object))
                    },
                null,
                identifier => true,
                identifier => true,
                identifier => true,
                async (a, b, c, d, e) =>
                    {
                        Critical.Assert(false, "Not supported.");
                        return null;
                    },
                obj => TaskEx.CompletedTask)
        {
        }

        /// <inheritdoc/>
        public override async ValueTask<IIxInstanceLock> GetInstance(
            IIxInstance parentInstance,
            IxIdentifier identifier,
            IxHost.IxResolveContext context,
            [CanBeNull] IxResolveFrame frame)
        {
            Critical.Assert(false, "Not supported.");
            return null;
        }

        /// <summary>
        /// Tries to get argument value for the provided identifier.
        /// </summary>
        /// <param name="identifier">Argument identifier.</param>
        /// <param name="context">The resolve context.</param>
        /// <returns>Argument instance lock or null, if no argument found for the provided identifier.</returns>
        public IIxInstanceLock TryGetInstance(IxIdentifier identifier, IxHost.IxResolveContext context)
        {
            IxHost.IxResolveContext curContext = context;
            while (curContext != null)
            {
                if (context.Arguments.TryGetValue(identifier, out var result))
                {
                    return new IxInstancePinLock(new IxArgumentInstance(this, result));
                }

                curContext = context.ParentContext;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return null;
        }
    }
}