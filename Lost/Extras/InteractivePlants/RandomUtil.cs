//-----------------------------------------------------------------------
// <copyright file="RandomUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlantGenerator
{
    using System;

    /// <summary>
    /// Extension methods for the Syste.Random class.
    /// </summary>
    public static class RandomUtil
    {
        /// <summary>
        /// Returns a random float between min (inclusive) and max (exclusive).
        /// </summary>
        /// <param name="random">The random object to use.</param>
        /// <param name="min">The min value to return.</param>
        /// <param name="max">The max value to return.</param>
        /// <returns>A random number between min and max.</returns>
        public static float FloatRangeExclusive(Random random, float min, float max)
        {
            return (float)(min + ((max - min) * random.NextDouble()));
        }

        /// <summary>
        /// Returns a random integer between min (inclusive) and max (inclusive).
        /// </summary>
        /// <param name="random">The random object to use.</param>
        /// <param name="min">The min value to return.</param>
        /// <param name="max">The max value to return.</param>
        /// <returns>A random number between min and max.</returns>
        public static int IntRangeInclusive(Random random, int min, int max)
        {
            return random.Next(min - 1, max) + 1;
        }
    }
}
