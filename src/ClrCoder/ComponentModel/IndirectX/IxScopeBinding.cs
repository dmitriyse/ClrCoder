// <copyright file="IxScopeBinding.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    public class IxScopeBinding
    {
        public static IxScopeBinding PerQuery => new IxScopeBinding();

        public static IxScopeBinding PerResolve => new IxScopeBinding();

        public static IxScopeBinding Registration => new IxScopeBinding();

        public static IxScopeBinding ResolveOrigin => new IxScopeBinding();
    }
}