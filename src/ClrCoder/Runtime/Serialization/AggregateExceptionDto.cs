// <copyright file="AggregateExceptionDto.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using MoreLinq;

    /// <summary>
    /// Data transferring <c>object</c> for <see cref="AggregateException"/>.
    /// </summary>
    public class AggregateExceptionDto : ExceptionDto
    {
        private static readonly HashSet<string> ExceptionBasePropertyNames;

        /// <summary>
        /// Initializes static members of the <see cref="AggregateExceptionDto"/> class.
        /// </summary>
        static AggregateExceptionDto()
        {
            ExceptionBasePropertyNames =
                typeof(AggregateException).GetTypeInfo().DeclaredProperties.Select(x => x.Name).ToHashSet();
        }

        /// <summary>
        /// Converts <see cref="AggregateException"/> to DTO.
        /// </summary>
        public static new DumpObjectFilterDelegate DumpFilter { get; } =
            (queriedType, dump, target, dumper) =>
                {
                    if (dump == null)
                    {
                        dump = dumper.Filter(typeof(Exception), new AggregateExceptionDto(), target);
                    }

                    if (!(dump is AggregateExceptionDto))
                    {
                        throw new NotSupportedException("Dump should be based on AggregateExceptionDto");
                    }

                    var ex = target as AggregateException;
                    if (ex == null)
                    {
                        throw new NotSupportedException("Exception should be derived from AggregateException.");
                    }

                    var dto = dump as AggregateExceptionDto;

                    dto.InnerException = null;

                    dto.InnerExceptions = new List<ExceptionDto>();
                    foreach (Exception innerException in ex.InnerExceptions)
                    {
                        dto.InnerExceptions.Add(dumper.Dump<ExceptionDto>(innerException));
                    }

                    // TODO: Move this copy past to some other place.
                    foreach (PropertyInfo declaredProperty in
                        queriedType.GetTypeInfo().DeclaredProperties
                            .Where(x => !ExceptionBasePropertyNames.Contains(x.Name))
                            .Where(x => x.CanRead))
                    {
                        try
                        {
                            object propertyValue = declaredProperty.GetValue(ex);
                            if (propertyValue != null)
                            {
                                dto.SetExtensionData(declaredProperty.Name, propertyValue);
                            }
                        }
                        catch (Exception e)
                        {
                            if (!e.IsProcessable())
                            {
                                throw;
                            }
                        }
                    }

                    return dto;
                };

        /// <summary>
        /// Inner exceptions.
        /// </summary>
        [CanBeNull]
        public List<ExceptionDto> InnerExceptions { get; set; }
    }
}