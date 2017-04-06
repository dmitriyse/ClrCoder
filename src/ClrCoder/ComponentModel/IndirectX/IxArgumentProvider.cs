// <copyright file="IxArgumentProvider.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
#pragma warning disable 1998
    using System.Threading.Tasks;

    public class IxArgumentProvider : IxProviderNode
    {
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
                (a, b, c, d, e) => Task.FromResult((IIxInstanceLock)null),
                obj => Task.CompletedTask)
        {
        }

        /// <inheritdoc/>
        public override async Task<IIxInstanceLock> GetInstance(
            IIxInstance parentInstance,
            IxIdentifier identifier,
            IxHost.IxResolveContext context,
            IxResolveFrame frame)
        {
            object result;
            IxHost.IxResolveContext curContext = context;
            while (curContext != null)
            {
                if (context.Arguments.TryGetValue(identifier, out result))
                {
                    return new IxInstanceTempLock(new IxArgumentInstance(this, result));
                }

                curContext = context.ParentContext;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return null;
        }
    }
}