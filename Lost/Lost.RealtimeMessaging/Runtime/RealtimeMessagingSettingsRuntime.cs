//-----------------------------------------------------------------------
// <copyright file="RealtimeMessagingSettingsRuntime.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    public static class RealtimeMessagingSettingsRuntime
    {
        public static readonly string InitializeAtStartupKey = "Lost.RealtimeMessaging.InitializeAtStartup";
        public static readonly string AblyKeyKey = "Lost.RealtimeMessaging.RegisterUnityAdsProvider";

        [RuntimeInitializeOnLoadMethod]
        public static void OnStartup()
        {
            if (RuntimeAppConfig.Instance.GetBool(InitializeAtStartupKey))
            {
                RealtimeMessageManager.Initialize();
            }
        }
    }
}
