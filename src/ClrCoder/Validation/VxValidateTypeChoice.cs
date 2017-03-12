// <copyright file="VxValidateTypeChoice.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Validation
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// TypeChoice validation fluent syntax.
    /// </summary>
    /// <typeparam name="T">Type of value to check.</typeparam>
    public struct VxValidateTypeChoice<T>
    {
        [CanBeNull]
        private readonly T _value;

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="VxValidateTypeChoice{T}"/> struct.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The parameter name.</param>
        internal VxValidateTypeChoice([CanBeNull] T value, string name)
        {
            _value = value;
            _name = name;
        }

        /// <summary>
        /// Validates that provided is one of the types: <typeparamref name="T1"/>.
        /// </summary>
        /// <typeparam name="T1">Type choice 1.</typeparam>
        public void Valid<T1>()
            where T1 : T
        {
            if (_value == null)
            {
                return;
            }

            if (!(_value is T1))
            {
                throw new ArgumentException($"{_name} should be one of: {typeof(T1).Name}");
            }
        }

        /// <summary>
        /// Validates that provided is one of the types: <typeparamref name="T1"/>, <typeparamref name="T2"/>.
        /// </summary>
        /// <typeparam name="T1">Type choice 1.</typeparam>
        /// <typeparam name="T2">Type choice 2.</typeparam>
        public void Valid<T1, T2>()
            where T1 : T
            where T2 : T
        {
            if (_value == null)
            {
                return;
            }

            if (!(_value is T1) && !(_value is T2))
            {
                throw new ArgumentException($"{_name} should be one of: {typeof(T1).Name}, {typeof(T2).Name}");
            }
        }
    }
}