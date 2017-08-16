// <copyright file="ExceptionDto.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using MoreLinq;

    using Newtonsoft.Json;
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System.Diagnostics;

#endif

    /// <summary>
    /// Data transferring <c>object</c> for <see cref="Exception"/>.
    /// </summary>
    [PublicAPI]
    public class ExceptionDto
    {
        private static readonly HashSet<string> ExceptionBasePropertyNames;

        /// <summary>
        /// Initializes static members of the <see cref="ExceptionDto"/> class.
        /// </summary>
        static ExceptionDto()
        {
            ExceptionBasePropertyNames =
                typeof(Exception).GetTypeInfo().DeclaredProperties.Select(x => x.Name).ToHashSet();
        }

        /// <summary>
        /// Converts <see cref="Exception"/> to DTO.
        /// </summary>
        public static DumpObjectFilterDelegate DumpFilter { get; } =
            (queriedType, dump, target, dumper) =>
                {
                    if (dump == null)
                    {
                        dump = dumper.Filter(typeof(object), new ExceptionDto(), target);
                    }

                    if (!(dump is ExceptionDto))
                    {
                        throw new NotSupportedException("Dump should be based on ExceptionDto");
                    }

                    var ex = target as Exception;
                    if (ex == null)
                    {
                        throw new NotSupportedException("Exception should be derived from  System.Exception.");
                    }

                    var dto = dump as ExceptionDto;

                    dto.TypeFullName = ex.GetType().FullName;
                    dto.Message = ex.Message;
                    dto.ToStringOutput = ex.ToString();
                    dto.HResult = ex.HResult;
                    dto.HelpLink = ex.HelpLink;

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
                    var stackTrace = new StackTrace(ex, true);
                    dto.StackTrace = stackTrace.GetFrames().Select(StackFrameDto.FromStackFrame).ToList();
#else
                    dto.StackTrace = ex.StackTrace;
#endif

                    foreach (DictionaryEntry dataentry in ex.Data)
                    {
                        dto.SetExtensionData(dataentry.Key.ToString(), dataentry.Value);
                    }

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
                        catch (Exception e) when (e.IsProcessable())
                        {
                            // Do nothing.
                        }
                    }

                    if (ex.InnerException != null)
                    {
                        dto.InnerException = dumper.Dump<ExceptionDto>(ex.InnerException);
                    }

                    return dto;
                };

        /// <summary>
        /// Type full name.
        /// </summary>
        [CanBeNull]
        public string TypeFullName { get; set; }

        /// <summary>
        /// Copy of <see cref="Exception.Message"/>.
        /// </summary>
        [CanBeNull]
        public string Message { get; set; }

        /// <summary>
        /// Inner exception.
        /// </summary>
        [CanBeNull]
        public ExceptionDto InnerException { get; set; }

        /// <summary>
        /// Copy of <see cref="Exception.HResult"/>.
        /// </summary>
        [DefaultValue(0)]
        public int HResult { get; set; }

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
        /// <summary>
        /// Exception stack trace.
        /// </summary>
        [CanBeNull]
        [ItemNotNull]
        public List<StackFrameDto> StackTrace { get; set; }
#else /// <summary>
/// Exception stack trace.
/// </summary>
        [CanBeNull]
        [ItemNotNull]
        public string StackTrace { get; set; }
#endif
        /// <summary>
        /// Copy of <see cref="Exception.HelpLink"/>.
        /// </summary>
        [CanBeNull]
        public string HelpLink { get; set; }

        /// <summary>
        /// Output of <see cref="Exception.ToString"/> method.
        /// </summary>
        [CanBeNull]
        public string ToStringOutput { get; set; }

        /// <summary>
        /// Extension data contains non specified in class properties.
        /// </summary>
        [JsonExtensionData]
        [CanBeNull]
        public virtual Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Sets extension data property.
        /// </summary>
        /// <param name="key">Extension data property name.</param>
        /// <param name="value">Extension data <c>value</c>.</param>
        public void SetExtensionData([NotNull] string key, [CanBeNull] object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (ExtensionData == null)
            {
                ExtensionData = new Dictionary<string, object>();
            }

            ExtensionData[key] = value;
        }
    }
}