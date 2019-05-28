//-----------------------------------------------------------------------
// <copyright file="Vector3Extensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public static class Vector3Extensions
    {
        public static Vector3 SetX(this Vector3 lhs, float val)
        {
            lhs.x = val;
            return lhs;
        }

        public static Vector3 SetY(this Vector3 lhs, float val)
        {
            lhs.y = val;
            return lhs;
        }

        public static Vector3 SetXY(this Vector3 lhs, Vector2 val)
        {
            lhs.x = val.x;
            lhs.y = val.y;
            return lhs;
        }

        public static Vector3 SetXY(this Vector3 lhs, float x, float y)
        {
            lhs.x = x;
            lhs.y = y;
            return lhs;
        }

        public static Vector3 SetXZ(this Vector3 lhs, float x, float z)
        {
            lhs.x = x;
            lhs.z = z;
            return lhs;
        }

        public static Vector3 SetZ(this Vector3 lhs, float val)
        {
            lhs.z = val;
            return lhs;
        }

        public static Vector3 AddToX(this Vector3 lhs, float val)
        {
            lhs.x += val;
            return lhs;
        }

        public static Vector3 AddToY(this Vector3 lhs, float val)
        {
            lhs.y += val;
            return lhs;
        }

        public static Vector3 AddToZ(this Vector3 lhs, float val)
        {
            lhs.z += val;
            return lhs;
        }
    }
}
