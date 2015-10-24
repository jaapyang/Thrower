﻿// File name: ObjectValidator.cs
// 
// Author(s): Alessio Parma <alessio.parma@gmail.com>
// 
// The MIT License (MIT)
// 
// Copyright (c) 2013-2016 Alessio Parma <alessio.parma@gmail.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using PommaLabs.Thrower.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PommaLabs.Thrower.Validation
{
    /// <summary>
    ///   Validates an object public properties that have been decorated with the
    ///   <see cref="ValidateAttribute"/> custom attribute.
    /// </summary>
    public static class ObjectValidator
    {
        /// <summary>
        ///   The placeholder used to indicate the starting object.
        /// </summary>
        public const string RootPlaceholder = "$";

        private static readonly ValidateAttribute DefaultValidation = new ValidateAttribute();

        private static readonly HashSet<Type> AlwaysValidTypes = new HashSet<Type>
        {
            typeof(bool),
            typeof(char),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            // ???
            typeof(string)
        };

        /// <summary>
        ///   Validates given object using information contained in the <see cref="ValidateAttribute"/> custom attribute.
        /// </summary>
        /// <param name="obj">The object to be validated.</param>
        /// <param name="validationErrors">All validation errors found.</param>
        /// <returns>True if object is valid, false otherwise.</returns>
        public static bool Validate(object obj, out IList<ValidationError> validationErrors)
        {
            validationErrors = new List<ValidationError>();
            return ValidateInternal(obj, RootPlaceholder, DefaultValidation, validationErrors);
        }

        private static bool ValidateInternal(object obj, string path, ValidateAttribute validation, IList<ValidationError> validationErrors)
        {
            if (validation.Required && ReferenceEquals(obj, null))
            {
                validationErrors.Add(new ValidationError { Path = path, Reason = "Object is required, found null" });
                return false;
            }

            var objType = obj.GetType();

            if (AlwaysValidTypes.Contains(objType) || PortableTypeInfo.IsEnum(objType))
            {
                return true;
            }

            var isValueType = PortableTypeInfo.IsValueType(objType);

            // Check whether this type is nullable.
            if (validation.Required && isValueType && PortableTypeInfo.IsGenericType(objType) && objType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nullableProps = PortableTypeInfo.GetPublicProperties(objType);
                var nullableHasValueProp = nullableProps.First(p => p.Name == nameof(Nullable<bool>.HasValue));

                if ((bool) PortableTypeInfo.GetPublicPropertyValue(obj, nullableHasValueProp))
                {
                    validationErrors.Add(new ValidationError { Path = path, Reason = "Object is required, found null" });
                    return false;
                }

                var nullableValueProp = nullableProps.First(p => p.Name == nameof(Nullable<bool>.Value));
                var nullableValue = PortableTypeInfo.GetPublicPropertyValue(obj, nullableValueProp);
                return ValidateInternal(nullableValue, path, validation, validationErrors);
            }

            if (PortableTypeInfo.IsClass(objType) || isValueType)
            {
                var props = PortableTypeInfo.GetPublicProperties(objType);
                var reqProps = from p in props
                               from a in PortableTypeInfo.GetCustomAttributes(p, false)
                               let v = a as ValidateAttribute
                               where v != null
                               select new { PropertyInfo = p, ValidateAttribute = v };

#if !PORTABLE
                var typeAccessor = Reflection.FastMember.TypeAccessor.Create(objType);
#endif

                foreach (var rp in reqProps)
                {
                    var propertyInfo = rp.PropertyInfo;

#if PORTABLE
                    var propertyValue = PortableTypeInfo.GetPublicPropertyValue(obj, propertyInfo);
#else
                    var propertyValue = PortableTypeInfo.GetPublicPropertyValue(typeAccessor, obj, propertyInfo);
#endif

                    var newPath = $"{path}.{propertyInfo.Name}";
                    ValidateInternal(propertyValue, newPath, rp.ValidateAttribute, validationErrors);

                    var collection = propertyValue as ICollection;
                    if (collection != null)
                    {

                    }

                    var enumerable = propertyValue as IEnumerable;
                    if (enumerable != null && rp.ValidateAttribute.Enumerate)
                    {
                        var index = 0;
                        foreach (var item in enumerable)
                        {
                            var indexedNewPath = $"{newPath}[{index}]";
                            ValidateInternal(item, indexedNewPath, DefaultValidation, validationErrors);
                        }
                    }
                }

                return validationErrors.Count == 0;
            }

            // Non dovrei mai finire qui!
            return true;
        }
    }
}