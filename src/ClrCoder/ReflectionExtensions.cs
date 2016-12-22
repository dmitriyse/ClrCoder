// <copyright file="ReflectionExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// Reflection extension methods.
    /// </summary>
    [PublicAPI]
    public static class ReflectionExtensions
    {
        //// TODO: Add methods to working with structs.
        //// TODO: Add tests for static data members.
        private static readonly ConcurrentDictionary<Type, DataMemberInfoesCacheEntry> DataMemberInfoesCache =
            new ConcurrentDictionary<Type, DataMemberInfoesCacheEntry>();

        /// <summary>
        /// First part of the "get data member value" syntax.
        /// </summary>
        /// <typeparam name="TObject"><c>Object</c> type.</typeparam>
        /// <param name="obj"><c>Object</c> to get data member value from.</param>
        /// <param name="memberName"><c>Object</c> data member(field or property) name.</param>
        /// <returns><c>Object</c> data member value.</returns>
        public static ReflectedClassDataMember<TObject> GetMember<TObject>(this TObject obj, string memberName)
        {
            var type = typeof(TObject);
            if (type.GetTypeInfo().IsClass)
            {
                // ReSharper disable once CompareNonConstrainedGenericWithNull
                if (obj == null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }

                type = obj.GetType();
            }

            if (memberName == null)
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            return new ReflectedClassDataMember<TObject>(type, obj, memberName);
        }

        //// TODO: add struct enumerable to improve performance.

        /// <summary>
        /// First part of the "get data member value" syntax.
        /// </summary>
        /// <param name="obj"><c>Object</c> to get data member value from.</param>
        /// <returns><c>Object</c> data member value.</returns>
        public static IReadOnlyCollection<ReflectedClassDataMember<object>> GetMembers(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var type = obj.GetType();
            var membersCache = GetDataMemberInfoesCache(type);

            return
                membersCache.Fields.Values.Where(x => !x.Name.StartsWith("<") && !x.IsStatic)
                    .Union(
                        membersCache.Properties.Values.Where(
                                x => (!x.CanRead || !x.GetMethod.IsStatic) && (!x.CanWrite || !x.SetMethod.IsStatic))
                            .Cast<MemberInfo>())
                    .Select(x => new ReflectedClassDataMember<object>(type, obj, x.Name))
                    .ToList();
        }

        /// <summary>
        /// Gets type member name from the specified <c>expression</c>.
        /// </summary>
        /// <example>
        /// <code>
        /// Type t = typeof(Type);
        /// string name = t.NameOf(_=>_.Name);
        /// </code>
        /// </example>
        /// <typeparam name="TObject"><c>Object</c> type.</typeparam>
        /// <typeparam name="TResult">Member type.</typeparam>
        /// <param name="obj"><c>Object</c>, used only for type inferring.</param>
        /// <param name="expression"><c>Expression</c> with <c>object</c> member access.</param>
        /// <returns><c>Object</c> member name.</returns>
        public static string NameOf<TObject, TResult>(this TObject obj, Expression<Func<TObject, TResult>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new NotSupportedException("Expression does not contains type member access.");
            }

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Sets <c>object</c> member <c>value</c>.
        /// </summary>
        /// <typeparam name="TObject">Object type.</typeparam>
        /// <typeparam name="TValue">Member <c>value</c> type.</typeparam>
        /// <param name="obj"><c>Object</c> to set member <c>value</c> to.</param>
        /// <param name="memberName"><c>Object</c> member (property or field) name.</param>
        /// <param name="value">Value to set.</param>
        public static void SetMemberValue<TObject, TValue>(this TObject obj, string memberName, TValue value)
        {
            var type = typeof(TObject);
            if (typeof(TObject).GetTypeInfo().IsClass)
            {
                // ReSharper disable once CompareNonConstrainedGenericWithNull
                if (obj == null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }

                type = obj.GetType();
            }

            if (memberName == null)
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            var setter = SetterCache<TObject, TValue>.Setters.GetOrAdd(
                new ValuedTuple<Type, string>(type, memberName),
                typeAndName =>
                    {
                        var cache = GetDataMemberInfoesCache(type);

                        PropertyInfo propertyInfo;
                        if (cache.Properties.TryGetValue(typeAndName.Item2, out propertyInfo))
                        {
                            // TODO: Replace to expression compilation.
                            return (o, v) => propertyInfo.SetValue(o, v);
                        }

                        FieldInfo fieldInfo;
                        if (!cache.Fields.TryGetValue(typeAndName.Item2, out fieldInfo))
                        {
                            throw new KeyNotFoundException("Cannot find property or field with the specified name.");
                        }

                        // TODO: Replace to expression compilation.
                        return (o, v) => fieldInfo.SetValue(o, v);
                    });

            setter.Invoke(obj, value);
        }

        private static DataMemberInfoesCacheEntry GetDataMemberInfoesCache(Type type)
        {
            return DataMemberInfoesCache.GetOrAdd(
                type,
                t =>
                    new DataMemberInfoesCacheEntry
                        {
                            Fields = type.GetRuntimeFields().ToDictionary(x => x.Name),
                            Properties = type.GetRuntimeProperties().ToDictionary(x => x.Name)
                        });
        }

        /// <summary>
        /// Second half of the "get member value" syntax.
        /// </summary>
        /// <typeparam name="TObject">Object type.</typeparam>
        [PublicAPI]
        public struct ReflectedClassDataMember<TObject>
        {
            private readonly TObject _obj;

            private readonly string _name;

            private readonly Type _type;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReflectedClassDataMember{TObject}"/> struct.
            /// </summary>
            /// <param name="type">Reflection API type.</param>
            /// <param name="obj">Object to get member value from.</param>
            /// <param name="name">Member name.</param>
            internal ReflectedClassDataMember(Type type, TObject obj, string name)
            {
                this._obj = obj;
                this._name = name;
                this._type = type;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ReflectedClassDataMember{TObject}"/> struct.
            /// </summary>
            /// <param name="type">Type to get member of.</param>
            /// <param name="name">Member name.</param>
            internal ReflectedClassDataMember(Type type, string name)
            {
                this._type = type;
                this._name = name;
                this._obj = default(TObject);
            }

            /// <summary>
            /// Reflection API member info.
            /// </summary>
            public MemberInfo Info
            {
                get
                {
                    var cache = GetDataMemberInfoesCache(_type);

                    PropertyInfo propertyInfo;
                    if (cache.Properties.TryGetValue(_name, out propertyInfo))
                    {
                        return propertyInfo;
                    }

                    FieldInfo fieldInfo;
                    if (!cache.Fields.TryGetValue(_name, out fieldInfo))
                    {
                        throw new KeyNotFoundException("Cannot find property or field with the specified name.");
                    }

                    return fieldInfo;
                }
            }

            /// <summary>
            /// Data member name.
            /// </summary>
            public string Name => _name;

            /// <summary>
            /// Data member type.
            /// </summary>
            public Type Type
            {
                get
                {
                    var cache = GetDataMemberInfoesCache(_type);

                    PropertyInfo propertyInfo;
                    if (cache.Properties.TryGetValue(_name, out propertyInfo))
                    {
                        return propertyInfo.PropertyType;
                    }

                    FieldInfo fieldInfo;
                    if (!cache.Fields.TryGetValue(_name, out fieldInfo))
                    {
                        throw new KeyNotFoundException("Cannot find property or field with the specified name.");
                    }

                    return fieldInfo.FieldType;
                }
            }

            /// <summary>
            /// Data member accessibility.
            /// </summary>
            public bool IsPublic
            {
                get
                {
                    var cache = GetDataMemberInfoesCache(this._type);

                    PropertyInfo propertyInfo;
                    if (cache.Properties.TryGetValue(this._name, out propertyInfo))
                    {
                        return propertyInfo.CanRead && propertyInfo.GetMethod.IsPublic;
                    }

                    FieldInfo fieldInfo;
                    if (!cache.Fields.TryGetValue(this._name, out fieldInfo))
                    {
                        throw new KeyNotFoundException("Cannot find property or field with the specified name.");
                    }

                    return fieldInfo.IsPublic;
                }
            }

            /// <summary>
            /// Gets value member value.
            /// </summary>
            /// <typeparam name="TValue">Value type.</typeparam>
            /// <returns>Value of the <c>object</c> member.</returns>
            public TValue Value<TValue>()
            {
                var typeLocal = this._type;
                var getter =
                    GetterCache<TObject, TValue>.Getters.GetOrAdd(
                        new ValuedTuple<Type, string>(typeLocal, this._name),
                        typeAndName =>
                            {
                                var cache = GetDataMemberInfoesCache(typeLocal);

                                PropertyInfo propertyInfo;
                                if (cache.Properties.TryGetValue(typeAndName.Item2, out propertyInfo))
                                {
                                    // TODO: Replace to expression compilation.
                                    return o => (TValue)propertyInfo.GetValue(o);
                                }

                                FieldInfo fieldInfo;
                                if (!cache.Fields.TryGetValue(typeAndName.Item2, out fieldInfo))
                                {
                                    throw new KeyNotFoundException(
                                              "Cannot find property or field with the specified name.");
                                }

                                // TODO: Replace to expression compilation.
                                return o => (TValue)fieldInfo.GetValue(o);
                            });

                return getter.Invoke(this._obj);
            }

            /// <summary>
            /// Gets untyped value.
            /// </summary>
            /// <returns>Member value.</returns>
            public dynamic Value()
            {
                return this.Value<object>();
            }
        }

        private struct DataMemberInfoesCacheEntry
        {
            public Dictionary<string, PropertyInfo> Properties;

            public Dictionary<string, FieldInfo> Fields;
        }

        private static class GetterCache<TObject, TValue>
        {
            public static readonly ConcurrentDictionary<ValuedTuple<Type, string>, Func<TObject, TValue>> Getters =
                new ConcurrentDictionary<ValuedTuple<Type, string>, Func<TObject, TValue>>();
        }

        private static class SetterCache<TObject, TValue>
        {
            public static readonly ConcurrentDictionary<ValuedTuple<Type, string>, Action<TObject, TValue>> Setters =
                new ConcurrentDictionary<ValuedTuple<Type, string>, Action<TObject, TValue>>();
        }
    }
}