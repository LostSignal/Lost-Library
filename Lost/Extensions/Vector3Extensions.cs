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
        
        public static Vector3 SetZ(this Vector3 lhs, float val)
        {
            lhs.z = val;
            return lhs;
        }
        
        public static Vector3 OffsetX(this Vector3 lhs, float val)
        {
            lhs.x += val;
            return lhs;
        }
        
        public static Vector3 OffsetY(this Vector3 lhs, float val)
        {
            lhs.y += val;
            return lhs;
        }
        
        public static Vector3 OffsetZ(this Vector3 lhs, float val)
        {
            lhs.z += val;
            return lhs;
        }
    }
}
