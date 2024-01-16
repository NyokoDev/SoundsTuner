﻿/*
The MIT License (MIT)

Copyright (c) 2015 Sebastian Schöner

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.


The original version can be found at:
 * https://github.com/sschoener/cities-skylines-detour/blob/master/CitiesSkylinesDebugInformation/RedirectionHelper.cs
This is a slightly edited version of the edited version by CBeTHaX that can be found at:
 * https://github.com/CBeTHaX/Skylines-Traffic-Manager/blob/master/TLM/TLM/RedirectionHelper.cs

Many thanks to those guys!

*/

using System;
using System.Reflection;

namespace AmbientSoundsTuner2.CommonShared.Utils
{
    /// <summary>
    /// A struct that represents a detour calls state.
    /// </summary>
    public struct DetourCallsState
    {
        /// <summary>
        /// A byte representing the state.
        /// </summary>
        public byte a, b, c, d, e;

        /// <summary>
        /// A unsigned long represeting the state.
        /// </summary>
        public ulong f;
    }

    /// <summary>
    /// Helper class to deal with detours. This version is for Unity 5 x64 on Windows.
    /// We provide three different methods of detouring.
    /// </summary>
    public static class DetourUtils
    {
        /// <summary>
        /// Redirects all calls from method 'from' to method 'to'.
        /// </summary>
        /// <param name="from">The method to redirect.</param>
        /// <param name="to">The method to redirect to.</param>
        public static DetourCallsState RedirectCalls(MethodInfo from, MethodInfo to)
        {
            // GetFunctionPointer enforces compilation of the method.
            var fptr1 = from.MethodHandle.GetFunctionPointer();
            var fptr2 = to.MethodHandle.GetFunctionPointer();
            return PatchJumpTo(fptr1, fptr2);
        }

        /// <summary>
        /// Reverts a redirect.
        /// </summary>
        /// <param name="from">The method to revert the redirect.</param>
        /// <param name="state">The state.</param>
        public static void RevertRedirect(MethodInfo from, DetourCallsState state)
        {
            var fptr1 = from.MethodHandle.GetFunctionPointer();
            RevertJumpTo(fptr1, state);
        }

        /// <summary>
        /// Primitive patching. Inserts a jump to 'target' at 'site'. Works even if both methods'
        /// callers have already been compiled.
        /// </summary>
        /// <param name="site"></param>
        /// <param name="target"></param>
        private static DetourCallsState PatchJumpTo(IntPtr site, IntPtr target)
        {
            DetourCallsState state = new DetourCallsState();

            // R11 is volatile.
            unsafe
            {
                byte* sitePtr = (byte*)site.ToPointer();
                state.a = *sitePtr;
                state.b = *(sitePtr + 1);
                state.c = *(sitePtr + 10);
                state.d = *(sitePtr + 11);
                state.e = *(sitePtr + 12);
                state.f = *((ulong*)(sitePtr + 2));

                *sitePtr = 0x49; // mov r11, target
                *(sitePtr + 1) = 0xBB;
                *((ulong*)(sitePtr + 2)) = (ulong)target.ToInt64();
                *(sitePtr + 10) = 0x41; // jmp r11
                *(sitePtr + 11) = 0xFF;
                *(sitePtr + 12) = 0xE3;
            }

            return state;
        }

        private static void RevertJumpTo(IntPtr site, DetourCallsState state)
        {
            unsafe
            {
                byte* sitePtr = (byte*)site.ToPointer();
                *sitePtr = state.a; // mov r11, target
                *(sitePtr + 1) = state.b;
                *((ulong*)(sitePtr + 2)) = state.f;
                *(sitePtr + 10) = state.c; // jmp r11
                *(sitePtr + 11) = state.d;
                *(sitePtr + 12) = state.e;
            }
        }
    }
}