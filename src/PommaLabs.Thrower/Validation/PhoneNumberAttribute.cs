﻿// File name: PhoneNumberAttribute.cs
//
// Author(s): Alessio Parma <alessio.parma@gmail.com>
//
// The MIT License (MIT)
//
// Copyright (c) 2013-2018 Alessio Parma <alessio.parma@gmail.com>
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
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if !(NET35 || NETSTD10)

using System;
using System.ComponentModel.DataAnnotations;

namespace PommaLabs.Thrower.Validation
{
    /// <summary>
    ///   Validates phone numbers stored as <see cref="string"/> members.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PhoneNumberAttribute : DataTypeAttribute
    {
        /// <summary>
        ///   Default constructor.
        /// </summary>
        public PhoneNumberAttribute() : base(DataType.PhoneNumber)
        {
            ErrorMessage = "Field '{0}' is not a valid phone number.";
        }

        /// <summary>
        ///   Validates the phone number stored in <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The phone number that should be validated.</param>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>True if given phone number is valid or null, false otherwise.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                // Null value is valid. If a member should be required, then it should also be
                // decorated with the Required attribute.
                return ValidationResult.Success;
            }

            var str = value as string;
            if (str != null)
            {
                return PhoneNumberValidator.Validate(str)
                    ? ValidationResult.Success
                    : new ValidationResult($"Given string '{str}' is not a valid phone number", new[] { validationContext.MemberName });
            }

            return new ValidationResult("Given object is not a valid phone number", new[] { validationContext.MemberName });
        }
    }
}

#endif