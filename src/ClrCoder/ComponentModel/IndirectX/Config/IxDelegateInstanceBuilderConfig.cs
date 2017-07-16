// <copyright file="IxDelegateInstanceBuilderConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Threading.Tasks;

    using Validation;

    /// <summary>
    /// Delegate factory config.
    /// </summary>
    public class IxDelegateInstanceBuilderConfig : IIxInstanceBuilderConfig
    {
        private IxDelegateInstanceBuilderConfig(Delegate func)
        {
            Func = func;
        }

        /// <summary>
        /// Factory function.
        /// </summary>
        public Delegate Func { get; }

        public static IxDelegateInstanceBuilderConfig New<T, TResult>(Func<T, Task<TResult>> func)
        {
            VxArgs.NotNull(func, nameof(func));
            return new IxDelegateInstanceBuilderConfig(func);
        }

        public static IxDelegateInstanceBuilderConfig New<T1, T2, TResult>(Func<T1, T2, Task<TResult>> func)
        {
            VxArgs.NotNull(func, nameof(func));
            return new IxDelegateInstanceBuilderConfig(func);
        }

        public static IxDelegateInstanceBuilderConfig New<T1, T2, T3, TResult>(
            Func<T1, T2, T3, Task<TResult>> func)
        {
            VxArgs.NotNull(func, nameof(func));
            return new IxDelegateInstanceBuilderConfig(func);
        }

        public static IxDelegateInstanceBuilderConfig New<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, Task<TResult>> func)
        {
            VxArgs.NotNull(func, nameof(func));
            return new IxDelegateInstanceBuilderConfig(func);
        }

        public static IxDelegateInstanceBuilderConfig New<T1, T2, T3, T4, T5, TResult>(
            Func<T1, T2, T3, T4, T5, Task<TResult>> func)
        {
            VxArgs.NotNull(func, nameof(func));
            return new IxDelegateInstanceBuilderConfig(func);
        }
    }
}