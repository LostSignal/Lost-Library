//-----------------------------------------------------------------------
// <copyright file="UnityEngine.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY

using System;

namespace UnityEngine
{
    public class SerializeFieldAttribute : System.Attribute
    {
    }

    public class HeaderAttribute : System.Attribute
    {
        public HeaderAttribute(string name)
        {
        }
    }

    public static class Debug
    {
        public static Action<string> OnLog;
        public static Action<string> OnLogError;
        public static Action<string> OnLogAssert;

        public static void Assert(bool condition, string message)
        {
            if (condition == false)
            {
                if (OnLogAssert != null)
                {
                    OnLogAssert(message);
                }
            }
        }

        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            if (condition == false)
            {
                Assert(condition, string.Format(format, args));
            }
        }

        public static void LogError(string message)
        {
            if (OnLogError != null)
            {
                OnLogError(message);
            }
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            LogError(string.Format(format, args));
        }

        public static void Log(string message)
        {
            if (OnLog!= null)
            {
                OnLog(message);
            }
        }

        public static void LogFormat(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }
    }

    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }

    public struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    public struct Color32
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public Color32(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    public struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }

    public struct Rect
    {
        public float xMin;
        public float yMin;
        public float width;
        public float height;

        public Rect(float xMin, float yMin, float width, float height)
        {
            this.xMin = xMin;
            this.yMin = yMin;
            this.width = width;
            this.height = height;
        }
    }

    public struct Plane
    {
        public Vector3 normal;
        public float distance;

        public Plane(Vector3 normal, float distance)
        {
            this.normal = normal;
            this.distance = distance;
        }
    }

    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }
}

#endif
