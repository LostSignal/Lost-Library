//-----------------------------------------------------------------------
// <copyright file="WaitForUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class WaitForUtil
    {
        private static Dictionary<float, WaitForSeconds> waitForSecondsCache = new Dictionary<float, WaitForSeconds>();
        private static Dictionary<float, WaitForSecondsRealtime> waitForSecondsRealtimeCache = new Dictionary<float, WaitForSecondsRealtime>();

        public static readonly WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();

        public static WaitForSeconds Seconds(float time)
        {
            WaitForSeconds waitForSeconds = null;

            if (waitForSecondsCache.TryGetValue(time, out waitForSeconds) == false)
            {
                waitForSeconds = new WaitForSeconds(time);
                waitForSecondsCache.Add(time, waitForSeconds);
            }

            return waitForSeconds;
        }

        public static WaitForSecondsRealtime RealtimeSeconds(float time)
        {
            WaitForSecondsRealtime waitForSecondsRealtime = null;

            if (waitForSecondsRealtimeCache.TryGetValue(time, out waitForSecondsRealtime) == false)
            {
                waitForSecondsRealtime = new WaitForSecondsRealtime(time);
                waitForSecondsRealtimeCache.Add(time, waitForSecondsRealtime);
            }

            return waitForSecondsRealtime;
        }
    }
}
