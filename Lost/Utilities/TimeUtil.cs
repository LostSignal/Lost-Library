//-----------------------------------------------------------------------
// <copyright file="TimeUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;
    
    public static class TimeUtil
    {
        public static float DeltaTime
        {
            get { return Time.deltaTime; }
        }

        /// <summary>
        /// Gets either Time.deltaTime or 1/24 of a second.  Which ever is smaller.
        /// </summary>
        /// <value>Time.deltaTime or 1/24.</value>
        public static float DeltaTimeCapped
        {
            get { return Math.Min(Time.deltaTime, 0.0416667f); }
        }
    }
}
