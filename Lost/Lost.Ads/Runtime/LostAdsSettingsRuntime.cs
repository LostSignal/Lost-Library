//-----------------------------------------------------------------------
// <copyright file="LostAdsSettingsRuntime.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost{
    using Lost.AppConfig;
    using UnityEngine;    public static class LostAdsSettingsRuntime    {
        public static readonly string InitializeAtStartupKey = "Lost.Ads.InitializeAtStartup";
        public static readonly string RegisterUnityAdsProviderKey = "Lost.Ads.RegisterUnityAdsProvider";
        public static readonly string AppleAppStoreIdKey = "Lost.Ads.AppleStoreId";
        public static readonly string GooglePlayAppStoreIdKey = "Lost.Ads.GooglePlayStoreId";

        [RuntimeInitializeOnLoadMethod]
        public static void OnStartup()
        {
            if (RuntimeAppConfig.Instance.GetBool(InitializeAtStartupKey))
            {
                AdsManager.Initialize();

                #if USING_UNITY_ADS
                if (RuntimeAppConfig.Instance.GetBool(RegisterUnityAdsProviderKey))
                {
                    AdsManager.Instance.SetAdProvider(new UnityAdsProvider(
                        RuntimeAppConfig.Instance.GetString(AppleAppStoreIdKey),
                        RuntimeAppConfig.Instance.GetString(GooglePlayAppStoreIdKey)));
                }
                #endif
            }
        }    }}