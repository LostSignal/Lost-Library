//-----------------------------------------------------------------------
// <copyright file="Matrix4x4Extensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public static class Matrix4x4Extensions
    {
        public static Quaternion GetRotation(this Matrix4x4 matrix)
        {
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        }

        public static Vector3 GetPosition(this Matrix4x4 matrix)
        {
            return new Vector3(matrix.m03, matrix.m13, matrix.m23);
        }

        public static Vector3 GetScale(this Matrix4x4 m)
        {
            var x = Mathf.Sqrt((m.m00 * m.m00) + (m.m01 * m.m01) + (m.m02 * m.m02));
            var y = Mathf.Sqrt((m.m10 * m.m10) + (m.m11 * m.m11) + (m.m12 * m.m12));
            var z = Mathf.Sqrt((m.m20 * m.m20) + (m.m21 * m.m21) + (m.m22 * m.m22));

            return new Vector3(x, y, z);
        }
    }
}
