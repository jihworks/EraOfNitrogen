// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure
{
    public static class DisposableEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dispose<T>(ref T? obj) where T : class, IDisposable
        {
            if (obj is null)
            {
                return;
            }
            obj.Dispose();
            obj = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeAll<T>(List<T> collection) where T : class, IDisposable
        {
            for (int i = 0; i < collection.Count; i++)
            {
                collection[i]?.Dispose();
            }
            collection.Clear();
        }
    }
}
