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
        public static Vector2 SetX(this Vector2 lhs, float val)
        {
            lhs.x = val;
            return lhs;
        }
        
        public static Vector2 SetY(this Vector2 lhs, float val)
        {
            lhs.y = val;
            return lhs;
        }
        
        public static Vector2 ConvertPixelSpaceToOrthographicWorldSpace(this Vector2 lhs)
        {
            return Camera.main.ScreenToWorldPoint(lhs);
        }
        
        public static float ConvertPixelSpaceToLengthInInches(this Vector2 lhs)
        {
            float dpi = Screen.dpi;

            if (dpi == 0)
            {
                dpi = 100.0f;
            }

            return lhs.magnitude / dpi;
        }
    }
}
