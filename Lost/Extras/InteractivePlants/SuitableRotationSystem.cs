//-----------------------------------------------------------------------
// <copyright file="SuitableRotationSystem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlantGenerator
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Helper class for picking rotations, since purely random never feels/looks right.
    /// </summary>
    public class SuitableRotationSystem
    {
        /// <summary>
        /// Stores whether or not a specific integer angle is already taken (0 -> 359).
        /// </summary>
        private bool[] suitableRotation = new bool[360];

        /// <summary>
        /// The random instance to use when generating random values.
        /// </summary>
        private System.Random random;

        /// <summary>
        /// Creates a new SuitableRotationSystem object using the given random object.
        /// </summary>
        /// <param name="random">The random object to use when generating random values.</param>
        public SuitableRotationSystem(System.Random random)
        {
            this.random = random;
        }

        /// <summary>
        /// If the given rotation isn't taken, then it returns the given rotation, else
        /// it will search for the next suitable/non-taken rotation it finds.  If all else
        /// fails it will return a random one +/- 30 degrees of the given rotation.
        /// </summary>
        /// <param name="rotation">The rotation we want to use.</param>
        /// <returns>The next best suitable rotation.</returns>
        public float GetSuitibleRotation(float rotation)
        {
            // making sure the given rotation is positive and between 0 and 359
            rotation = (rotation + 360) % 360;

            // getting it's integer index of this rotation
            int rotationIndex = Mathf.RoundToInt(rotation);

            // seeing if the given rotation is suitable
            if (suitableRotation[rotationIndex])
            {
                return rotation;
            }
            else 
            {
                // this given rotation wasn't suitable so looking up and down to find the next suitable one
                for (int i = 0; i < 180; i++)
                {
                    int newRotationIndexUp = (rotationIndex + i) % 360;
                    int newRotationIndexDown = (rotationIndex + 360 - i) % 360;

                    if (suitableRotation[newRotationIndexUp])
                    {
                        return (float)newRotationIndexUp;
                    }
                    else if (suitableRotation[newRotationIndexDown])
                    {
                        return (float)newRotationIndexDown;
                    }
                }

                // no suitable angle found, so returning a random one
                return (rotation + RandomUtil.IntRangeInclusive(random, -30, 30) + 360) % 360;
            }
        }

        /// <summary>
        /// Makes sure that the centerAngle and all angles +/- extendsAngles are no
        /// longer suitable rotations to be picked.
        /// </summary>
        /// <param name="centerAngle">The exact angle that was picked and no longer suitable.</param>
        /// <param name="extentsAngle">The number of angles +/- centerAngle to mark as not suitable.</param>
        public void AddDeadSpot(float centerAngle, int extentsAngle)
        {
            int rotationIndex = Mathf.RoundToInt(centerAngle) % 360;
            suitableRotation[rotationIndex] = false;

            for (int i = 0; i < extentsAngle; i++)
            {
                int newRotationIndexUp = (rotationIndex + i) % 360;
                int newRotationIndexDown = (rotationIndex + 360 - i) % 360;
                suitableRotation[newRotationIndexUp] = false;
                suitableRotation[newRotationIndexDown] = false;
            }
        }
    }
}