﻿// File name: ObjectDisposedExceptionTests.cs
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

using NUnit.Framework;
using Shouldly;
using System;

namespace PommaLabs.Thrower.UnitTests.ExceptionHandlers
{
    internal sealed class ObjectDisposedExceptionTests : AbstractTests
    {
        [Test]
        public void If_TrueShouldThrow()
        {
            Should.Throw<ObjectDisposedException>(() => Raise.ObjectDisposedException.If(true, nameof(ObjectDisposedExceptionTests)));
        }

        [Test]
        public void If_TrueShouldThrow_WithMessage()
        {
            Should.Throw<ObjectDisposedException>(() => Raise.ObjectDisposedException.If(true, nameof(ObjectDisposedExceptionTests), TestMessage));
        }

        [Test]
        public void If_FalseShouldNotThrow()
        {
            Raise.ObjectDisposedException.If(false, nameof(ObjectDisposedExceptionTests));
        }

        [Test]
        public void If_FalseShouldNotThrow_WithMessage()
        {
            Raise.ObjectDisposedException.If(false, nameof(ObjectDisposedExceptionTests), TestMessage);
        }
    }
}