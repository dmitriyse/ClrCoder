// <copyright file="HttpRequestScopeConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    using System;
    using System.Collections.Generic;

    using ComponentModel.IndirectX;

    public class HttpRequestScopeConfig<TRequestScope> : IxStdProviderConfig,
                                                         IIxStdProviderConfig,
                                                         IIxBasicIdentificationConfig
        where TRequestScope : HttpRequestScope
    {
        Type IIxBasicIdentificationConfig.ContractType => typeof(HttpRequestScope);

        IIxInstanceBuilderConfig IIxStdProviderConfig.InstanceBuilder =>
            new IxClassInstanceBuilderConfig<TRequestScope>();

        IIxMultiplicityConfig IIxStdProviderConfig.Multiplicity => new IxPerResolveMultiplicityConfig();

        ICollection<IIxProviderNodeConfig> IIxProviderNodeConfig.Nodes =>
            new HashSet<IIxProviderNodeConfig>(Nodes)
                {
                    new IxStdProviderConfig
                        {
                            ContractType = typeof(IHttpRequestScope),
                            InstanceBuilder = IxDelegateInstanceBuilderConfig.New(
                                async (HttpRequestScope requestScope) => (IHttpRequestScope)requestScope)
                        }
                };
    }
}