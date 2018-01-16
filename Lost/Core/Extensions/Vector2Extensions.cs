//-----------------------------------------------------------------------
// <copyright file="Vector2Extensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    
    public static class Vector2Extensions
    {
        public static Vector2 SetX(this Vector2 vector, float x)
        {
            vector.x = x;
            return vector;
        }
        
        public static Vector2 SetY(this Vector2 vector, float y)
        {
            vector.y = y;
            return vector;
        }

        public static Vector3 AddZ(this Vector2 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }
        
        public static Vector2 ConvertPixelSpaceToOrthographicWorldSpace(this Vector2 vector)
        {
            return Camera.main.ScreenToWorldPoint(vector);
        }
        
        public static float ConvertPixelSpaceToLengthInInches(this Vector2 vector)
        {
            float dpi = Screen.dpi;

            if (dpi == 0)
            {
                dpi = 100.0f;
            }

            return vector.magnitude / dpi;
        }
    }
}
