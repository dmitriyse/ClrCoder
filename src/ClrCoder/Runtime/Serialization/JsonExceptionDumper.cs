// <copyright file="JsonExceptionDumper.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Basic exceptions dumper suitable for consequent json serialization.
    /// </summary>
    public class JsonExceptionDumper : ObjectDumper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonExceptionDumper"/> class.
        /// </summary>
        public JsonExceptionDumper()
            : base(DefaultFilters)
        {
        }

        /// <summary>
        /// Filters that are used for dumping. Feel free to extend them.
        /// </summary>
        public static Dictionary<Type, DumpObjectFilterDelegate> DefaultFilters { get; set; } =
            new Dictionary<Type, DumpObjectFilterDelegate>
                {
                    { typeof(Exception), ExceptionDto.DumpFilter },
                    { typeof(AggregateException), AggregateExceptionDto.DumpFilter }
                };
    }
}