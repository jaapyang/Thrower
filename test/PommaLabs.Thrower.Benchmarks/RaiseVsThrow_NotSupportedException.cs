﻿// File name: RaiseVsThrow_NotSupportedException.cs
//
// Author(s): Alessio Parma <alessio.parma@gmail.com>
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

using BenchmarkDotNet.Attributes;
using System;
using System.Runtime.CompilerServices;

#pragma warning disable CC0091 // Use static method

namespace PommaLabs.Thrower.Benchmarks
{
    [Config(typeof(Program.Config))]
    public class RaiseVsThrow_NotSupportedException
    {
        private static readonly Random Rnd = new Random();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static T Identity<T>(T value) => value;

        [Benchmark(Baseline = true)]
        public Exception Raise()
        {
            try
            {
                var b = Identity(Rnd.Next() % 2 == 0);
                Thrower.Raise.NotSupportedException.If(b, nameof(RaiseVsThrow_NotSupportedException));
            }
            catch (Exception ex)
            {
                return ex;
            }
            return default(Exception);
        }

        [Benchmark]
        public Exception RaiseGeneric()
        {
            try
            {
                var b = Identity(Rnd.Next() % 2 == 0);
                Raise<NotSupportedException>.If(b, nameof(RaiseVsThrow_NotSupportedException));
            }
            catch (Exception ex)
            {
                return ex;
            }
            return default(Exception);
        }

        [Benchmark]
        public Exception Throw()
        {
            try
            {
                var b = Identity(Rnd.Next() % 2 == 0);
                if (b)
                {
                    throw new NotSupportedException(nameof(RaiseVsThrow_NotSupportedException));
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            return default(Exception);
        }
    }
}

#pragma warning restore CC0091 // Use static method