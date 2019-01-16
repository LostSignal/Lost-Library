//-----------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;

    public static class DateTimeExtensions
    {
        public static string ToISO8601(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public static bool IsEqualTo(this DateTime dateTime, DateTime other)
        {
            return Math.Abs(dateTime.Subtract(other).TotalMilliseconds) < 1;
        }
    }
}
