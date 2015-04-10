using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ClrCoder.System
{
    /// <summary>
    /// Reflection extension methods.
    /// </summary>
    public static class ReflectionExtensions
    {
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
        public static string NameOf<TObject, TResult>(
            this TObject obj,
            Expression<Func<TObject, TResult>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new NotSupportedException(
                    "Expression does not contains type member access.");
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
            if (typeof(TObject).GetTypeInfo().IsClass)
            {
                // ReSharper disable once CompareNonConstrainedGenericWithNull
                if (obj == null)
                {
                    throw new ArgumentNullException("obj");
                }
            }

            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }

            Action<TObject, TValue> setter = SetterCache<TObject, TValue>.Setters.GetOrAdd(
                memberName,
                name =>
                    {
                        Type objType = typeof(TObject);
                        PropertyInfo propertyInfo = objType.GetRuntimeProperty(name);
                        if (propertyInfo != null)
                        {
                            // TODO: Replace to expression compilation.
                            return (o, v) => propertyInfo.SetValue(o, v);
                        }

                        FieldInfo fieldInfo = objType.GetRuntimeField(name);
                        if (fieldInfo == null)
                        {
                            throw new KeyNotFoundException("name");
                        }

                        // TODO: Replace to expression compilation.
                        return (o, v) => fieldInfo.SetValue(o, v);
                    });

            setter.Invoke(obj, value);
        }

        /// <summary>
        /// First part of the "get member value" syntax.
        /// </summary>
        /// <typeparam name="TObject"><c>Object</c> type.</typeparam>
        /// <param name="obj"><c>Object</c> to get member value from.</param>
        /// <param name="memberName"><c>Object</c> member(field or property) name.</param>
        /// <returns><c>Object</c> member value.</returns>
        public static ReflectedClassMember<TObject> GetMember<TObject>(this TObject obj, string memberName)
        {
            if (typeof(TObject).GetTypeInfo().IsClass)
            {
                // ReSharper disable once CompareNonConstrainedGenericWithNull
                if (obj == null)
                {
                    throw new ArgumentNullException("obj");
                }
            }

            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }

            return new ReflectedClassMember<TObject>(obj, memberName);
        }

        /// <summary>
        /// Second half of the "get member value" syntax.
        /// </summary>
        /// <typeparam name="TObject">Object type.</typeparam>
        public struct ReflectedClassMember<TObject>
        {
            private readonly TObject _obj;

            private readonly string _memberName;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReflectedClassMember{TObject}"/> struct.
            /// </summary>
            /// <param name="obj">Object to get member value from.</param>
            /// <param name="memberName">Member name.</param>
            internal ReflectedClassMember(TObject obj, string memberName)
            {
                _obj = obj;
                _memberName = memberName;
            }

            /// <summary>
            /// Gets value member value.
            /// </summary>
            /// <typeparam name="TValue">Value type.</typeparam>
            /// <returns>Value of the object member.</returns>
            public TValue Value<TValue>()
            {
                Func<TObject, TValue> getter = GetterCache<TObject, TValue>.Getters.GetOrAdd(
                    _memberName,
                    name =>
                        {
                            Type objType = typeof(TObject);
                            PropertyInfo propertyInfo = objType.GetRuntimeProperty(name);

                            if (propertyInfo != null)
                            {
                                // TODO: Replace to expression compilation.
                                return o => (TValue)propertyInfo.GetValue(o);
                            }

                            FieldInfo fieldInfo = objType.GetRuntimeField(name);
                            if (fieldInfo == null)
                            {
                                // TODO: Replace to expression compilation.
                                throw new KeyNotFoundException("name");
                            }

                            return o => (TValue)fieldInfo.GetValue(o);
                        });

                return getter.Invoke(_obj);
            }
        }

        private static class SetterCache<TObject, TValue>
        {
            public static readonly ConcurrentDictionary<string, Action<TObject, TValue>> Setters =
                new ConcurrentDictionary<string, Action<TObject, TValue>>();
        }

        private static class GetterCache<TObject, TValue>
        {
            public static readonly ConcurrentDictionary<string, Func<TObject, TValue>> Getters =
                new ConcurrentDictionary<string, Func<TObject, TValue>>();
        }
    }
}