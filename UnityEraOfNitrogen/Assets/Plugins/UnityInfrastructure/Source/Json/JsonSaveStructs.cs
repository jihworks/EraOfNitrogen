// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

#if INFRASTRUCTURE_USE_NEWTONSOFT_JSON

using Newtonsoft.Json;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Json
{
    [JsonObject]
    public struct JsonSaveVector2
    {
        [JsonProperty] public float X;
        [JsonProperty] public float Y;

        public JsonSaveVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2(JsonSaveVector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static implicit operator JsonSaveVector2(Vector2 v)
        {
            return new JsonSaveVector2(v.x, v.y);
        }
    }

    [JsonObject]
    public struct JsonSaveVector3
    {
        [JsonProperty] public float X;
        [JsonProperty] public float Y;
        [JsonProperty] public float Z;

        public JsonSaveVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Vector3(JsonSaveVector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        public static implicit operator JsonSaveVector3(Vector3 v)
        {
            return new JsonSaveVector3(v.x, v.y, v.z);
        }
    }

    [JsonObject]
    public struct JsonSaveVector4
    {
        [JsonProperty] public float X;
        [JsonProperty] public float Y;
        [JsonProperty] public float Z;
        [JsonProperty] public float W;

        public JsonSaveVector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static implicit operator Vector4(JsonSaveVector4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static implicit operator JsonSaveVector4(Vector4 v)
        {
            return new JsonSaveVector4(v.x, v.y, v.z, v.w);
        }
    }

    [JsonObject]
    public struct JsonSaveVector2Int
    {
        [JsonProperty] public int X;
        [JsonProperty] public int Y;

        public JsonSaveVector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2Int(JsonSaveVector2Int v)
        {
            return new Vector2Int(v.X, v.Y);
        }
        public static implicit operator JsonSaveVector2Int(Vector2Int v)
        {
            return new JsonSaveVector2Int(v.x, v.y);
        }
    }

    [JsonObject]
    public struct JsonSaveVector3Int
    {
        [JsonProperty] public int X;
        [JsonProperty] public int Y;
        [JsonProperty] public int Z;

        public JsonSaveVector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Vector3Int(JsonSaveVector3Int v)
        {
            return new Vector3Int(v.X, v.Y, v.Z);
        }
        public static implicit operator JsonSaveVector3Int(Vector3Int v)
        {
            return new JsonSaveVector3Int(v.x, v.y, v.z);
        }
    }

    [JsonObject]
    public struct JsonSaveMatrix4x4
    {
        [JsonProperty] public float M00;
        [JsonProperty] public float M10;
        [JsonProperty] public float M20;
        [JsonProperty] public float M30;

        [JsonProperty] public float M01;
        [JsonProperty] public float M11;
        [JsonProperty] public float M21;
        [JsonProperty] public float M31;

        [JsonProperty] public float M02;
        [JsonProperty] public float M12;
        [JsonProperty] public float M22;
        [JsonProperty] public float M32;

        [JsonProperty] public float M03;
        [JsonProperty] public float M13;
        [JsonProperty] public float M23;
        [JsonProperty] public float M33;

        public JsonSaveMatrix4x4(
            float m00, float m10, float m20, float m30,
            float m01, float m11, float m21, float m31,
            float m02, float m12, float m22, float m32,
            float m03, float m13, float m23, float m33)
        {
            M00 = m00;
            M10 = m10;
            M20 = m20;
            M30 = m30;

            M01 = m01;
            M11 = m11;
            M21 = m21;
            M31 = m31;

            M02 = m02;
            M12 = m12;
            M22 = m22;
            M32 = m32;

            M03 = m03;
            M13 = m13;
            M23 = m23;
            M33 = m33;
        }

        public static implicit operator Matrix4x4(JsonSaveMatrix4x4 m)
        {
            Matrix4x4 r;
            r.m00 = m.M00;
            r.m10 = m.M10;
            r.m20 = m.M20;
            r.m30 = m.M30;

            r.m01 = m.M01;
            r.m11 = m.M11;
            r.m21 = m.M21;
            r.m31 = m.M31;

            r.m02 = m.M02;
            r.m12 = m.M12;
            r.m22 = m.M22;
            r.m32 = m.M32;

            r.m03 = m.M03;
            r.m13 = m.M13;
            r.m23 = m.M23;
            r.m33 = m.M33;
            return r;
        }
        public static implicit operator JsonSaveMatrix4x4(Matrix4x4 m)
        {
            JsonSaveMatrix4x4 r;
            r.M00 = m.m00;
            r.M10 = m.m10;
            r.M20 = m.m20;
            r.M30 = m.m30;

            r.M01 = m.m01;
            r.M11 = m.m11;
            r.M21 = m.m21;
            r.M31 = m.m31;

            r.M02 = m.m02;
            r.M12 = m.m12;
            r.M22 = m.m22;
            r.M32 = m.m32;

            r.M03 = m.m03;
            r.M13 = m.m13;
            r.M23 = m.m23;
            r.M33 = m.m33;
            return r;
        }
    }

    [JsonObject]
    public struct JsonSaveRect
    {
        [JsonProperty] public float X;
        [JsonProperty] public float Y;
        [JsonProperty] public float Width;
        [JsonProperty] public float Height;

        public JsonSaveRect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static implicit operator Rect(JsonSaveRect rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static implicit operator JsonSaveRect(Rect rect)
        {
            return new JsonSaveRect(rect.x, rect.y, rect.width, rect.height);
        }
    }

    [JsonObject]
    public struct JsonSaveRectInt
    {
        [JsonProperty] public int X;
        [JsonProperty] public int Y;
        [JsonProperty] public int Width;
        [JsonProperty] public int Height;

        public JsonSaveRectInt(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static implicit operator RectInt(JsonSaveRectInt rect)
        {
            return new RectInt(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static implicit operator JsonSaveRectInt(RectInt rect)
        {
            return new JsonSaveRectInt(rect.x, rect.y, rect.width, rect.height);
        }
    }

    [JsonObject]
    public struct JsonSaveBounds
    {
        [JsonProperty] public JsonSaveVector3 Center;
        [JsonProperty] public JsonSaveVector3 Extents;

        public JsonSaveBounds(JsonSaveVector3 center, JsonSaveVector3 extents)
        {
            Center = center;
            Extents = extents;
        }

        public static implicit operator JsonSaveBounds(Bounds bounds)
        {
            return new JsonSaveBounds(bounds.center, bounds.extents);
        }
        public static implicit operator Bounds(JsonSaveBounds bounds)
        {
            return new Bounds(bounds.Center, bounds.Extents);
        }
    }

    [JsonObject]
    public struct JsonSaveBoundsInt
    {
        [JsonProperty] public JsonSaveVector3Int Position;
        [JsonProperty] public JsonSaveVector3Int Size;

        public JsonSaveBoundsInt(JsonSaveVector3Int position, JsonSaveVector3Int size)
        {
            Position = position;
            Size = size;
        }

        public static implicit operator JsonSaveBoundsInt(BoundsInt bounds)
        {
            return new JsonSaveBoundsInt(bounds.position, bounds.size);
        }
        public static implicit operator BoundsInt(JsonSaveBoundsInt bounds)
        {
            return new BoundsInt(bounds.Position, bounds.Size);
        }
    }
}

#endif
