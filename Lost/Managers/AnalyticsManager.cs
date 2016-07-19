//-----------------------------------------------------------------------
// <copyright file="AnalyticsManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_ANALYTICS

namespace Lost
{
    public class AnalyticsManager : Lost.SingletonGameObject<AnalyticsManager>
    {
        private void Start()
        {
            //// UnityEngine.Analytics.Analytics.Transaction("12345abcde", 0.99m, "USD", null, null);
            //// 
            //// UnityEngine.Analytics.Analytics.CustomEvent("Estimated Revenue", new System.Collections.Generic.Dictionary<string, object>
            //// {
            ////     { "Type", "Ad Watched" },
            ////     { "Value", 0.012 },
            //// });
            //// 
            //// UnityEngine.Analytics.Analytics.SetUserGender(UnityEngine.Analytics.Gender.Male);
            //// UnityEngine.Analytics.Analytics.SetUserBirthYear(1983);
        }
    }
}

#endif
