// <copyright file="Vx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    using Validation;

    /// <summary>
    /// Validation error <c>throw</c> tool.
    /// </summary>
    public static partial class Vx
    {
        /// <summary>
        /// Throws error for method with zero arguments.
        /// </summary>
        /// <param name="callerFilePath">Calling member file path.</param>
        /// <param name="callerLineNumber">Calling member line number.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [ContractAnnotation("=>halt")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument", Justification = "This is very special case.")]
        public static void Throw(
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            VxCore.Throw(callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Throws error for method with zero arguments.
        /// </summary>
        /// <typeparam name="T">First argument type.</typeparam>
        /// <param name="arg1">First argument value.</param>
        /// <param name="callerFilePath">Calling member file path.</param>
        /// <param name="callerLineNumber">Calling member line number.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [ContractAnnotation("=>halt")]
        public static void Throw<T>(
            T arg1,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            VxCore.Throw(callerFilePath, callerLineNumber, arg1);
        }

        /// <summary>
        /// Throws error for method with zero arguments.
        /// </summary>
        /// <typeparam name="T1">First argument type.</typeparam>
        /// <typeparam name="T2">Second argument type.</typeparam>
        /// <param name="arg1">First argument value.</param>
        /// <param name="arg2">Second argument value.</param>
        /// <param name="callerFilePath">Calling member file path.</param>
        /// <param name="callerLineNumber">Calling member line number.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [ContractAnnotation("=>halt")]
        public static void Throw<T1, T2>(
            T1 arg1,
            T2 arg2,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            VxCore.Throw(callerFilePath, callerLineNumber, arg1, arg2);
        }

        /// <summary>
        /// Throws error for method with zero arguments.
        /// </summary>
        /// <typeparam name="T1">First argument type.</typeparam>
        /// <typeparam name="T2">Second argument type.</typeparam>
        /// <typeparam name="T3">Third argument type.</typeparam>
        /// <param name="arg1">First argument value.</param>
        /// <param name="arg2">Second argument value.</param>
        /// <param name="arg3">Third argument value.</param>
        /// <param name="callerFilePath">Calling member file path.</param>
        /// <param name="callerLineNumber">Calling member line number.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [ContractAnnotation("=>halt")]
        public static void Throw<T1, T2, T3>(
            T1 arg1,
            T2 arg2,
            T3 arg3,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            VxCore.Throw(callerFilePath, callerLineNumber, arg1, arg2, arg3);
        }
    }
}