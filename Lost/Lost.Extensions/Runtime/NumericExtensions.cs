//-----------------------------------------------------------------------
// <copyright file="NumericExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public static class NumericExtensions
    {
        private const double PiOverOneEighty = System.Math.PI / 180.0;
        private const double OneEightyOverPie = 180.0 / System.Math.PI;

        public static double ToRadians(this double val)
        {
            return val * PiOverOneEighty;
        }

        public static double ToDegrees(this double val)
        {
            return val * OneEightyOverPie;
        }

        public static float ToRadians(this float val)
        {
            return (float)(val * PiOverOneEighty);
        }

        public static float ToDegrees(this float val)
        {
            return (float)(val * OneEightyOverPie);
        }

        public static int BetweenInclusive(this int val, int minValue, int maxValue)
        {
            if (val < minValue)
            {
                return minValue;
            }
            else if (val > maxValue)
            {
                return maxValue;
            }
            else
            {
                return val;
            }
        }

        public static float BetweenInclusive(this float val, float minValue, float maxValue)
        {
            if (val < minValue)
            {
                return minValue;
            }
            else if (val > maxValue)
            {
                return maxValue;
            }
            else
            {
                return val;
            }
        }
    }
}
