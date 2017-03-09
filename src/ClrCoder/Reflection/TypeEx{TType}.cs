// <copyright file="TypeEx{TType}.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Reflection
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// Extensions methods for the BCL reflection API.
    /// </summary>
    /// <typeparam name="TType"><c>Type</c> argument for extension methods.</typeparam>
    [PublicAPI]
    public static class TypeEx<TType>
    {
        /// <summary>
        /// Creates type constructor <see langword="delegate"/>.<br/>
        /// TODO: Add result caching.
        /// </summary>
        /// <exception cref="InvalidOperationException">Type does not have parameterless instance constructor.</exception>
        /// <returns>Type constructor delegate.</returns>
        public static Func<TType> CreateConstructorDelegate()
        {
            Type type = typeof(TType);

            ConstructorInfo ci =
                type.GetTypeInfo()
                    .DeclaredConstructors.Where(x => !x.GetParameters().Any() && !x.IsStatic)
                    .OrderBy(x => x.IsPublic ? 0 : 1)
                    .FirstOrDefault();
            if (ci == null)
            {
                throw new InvalidOperationException("Type does not have parameterless instance constructor.");
            }

            NewExpression body = Expression.New(ci);
            return Expression.Lambda<Func<TType>>(body).Compile();
        }
    }
}