//-----------------------------------------------------------------------
// <copyright file="LostAnalyticsSettingsRuntime.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost{
    using Lost.AppConfig;
    using UnityEngine;    public static class LostAnalyticsSettingsRuntime    {
        public static readonly string InitializeAtStartupKey = "Lost.Analytics.InitializeAtStartup";
        public static readonly string RegisterPlayFabAnalyticsProvider = "Lost.Analytics.RegisterPlayFabProvider";

        [RuntimeInitializeOnLoadMethod]
        public static void OnStartup()
        {
            if (RuntimeAppConfig.Instance.GetBool(InitializeAtStartupKey))
            {
                AnalyticsManager.Initialize();

                #if USING_PLAYFAB_SDK
                if (RuntimeAppConfig.Instance.GetBool(RegisterPlayFabAnalyticsProvider))
                {
                    AnalyticsManager.Instance.RegisterAnalyticsProvider(new PlayFabAnalyticsProvider());
                }
                #endif
            }
        }    }}