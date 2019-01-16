//-----------------------------------------------------------------------
// <copyright file="RectExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public static class RectExtensions
    {
        public static Rect SetWidth(this Rect lhs, float width)
        {
            lhs.width = width;
            return lhs;
        }

        public static Rect SetHeight(this Rect lhs, float height)
        {
            lhs.height = height;
            return lhs;
        }
    }
}
