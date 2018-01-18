// <copyright file="CamelCaseExceptDictionaryKeysResolver.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Json
{
    using System;

    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Standard camel case contract resolver fix. Dictionary keys case not modified.
    /// </summary>
    public class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
    {
        /// <inheritdoc/>
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);

            contract.DictionaryKeyResolver = propertyName => propertyName;

            return contract;
        }
    }
}