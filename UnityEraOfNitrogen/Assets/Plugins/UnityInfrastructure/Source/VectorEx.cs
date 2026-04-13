// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Jih.Unity.Infrastructure
{
    public static class VectorEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CreateUniform2(float value)
        {
            return new Vector2(value, value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 CreateUniform3(float value)
        {
            return new Vector3(value, value, value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 CreateUniform4(float value)
        {
            return new Vector4(value, value, value, value);
        }
    }
}
