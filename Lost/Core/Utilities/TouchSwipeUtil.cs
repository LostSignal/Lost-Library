//-----------------------------------------------------------------------
// <copyright file="TouchSwipeUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
    }

    public static class TouchSwipeUtil
    {
        public static bool WasSwiped(SwipeDirection direction, Vector2 swipingVector, double cosDegrees)
        {
            Vector3 dir;

            if (direction == SwipeDirection.Up)
            {
                dir = Vector2.up;
            }
            else if (direction == SwipeDirection.Down)
            {
                dir = -Vector2.up;
            }
            else if (direction == SwipeDirection.Left)
            {
                dir = -Vector2.right;
            }
            else if (direction == SwipeDirection.Right)
            {
                dir = Vector2.right;
            }
            else if (direction == SwipeDirection.UpLeft)
            {
                dir = new Vector2(-0.707f, 0.707f);
            }
            else if (direction == SwipeDirection.UpRight)
            {
                dir = new Vector2(0.707f, 0.707f);
            }
            else if (direction == SwipeDirection.DownLeft)
            {
                dir = new Vector2(-0.707f, -0.707f);
            }
            else if (direction == SwipeDirection.DownRight)
            {
                dir = new Vector2(0.707f, -0.707f);
            }
            else
            {
                throw new NotImplementedException("Found unknown SwipeDirection!");
            }

            return Vector2.Dot(dir, swipingVector) > cosDegrees;
        }
    }
}
