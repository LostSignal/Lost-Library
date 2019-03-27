//-----------------------------------------------------------------------
// <copyright file="LostAdsSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(40)]
    public class LostAdsSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool initializeAtStartup = true;
        [SerializeField] private bool registerUnityAdsProvider = true;
        [SerializeField] private string appleAppStoreId = null;
        [SerializeField] private string googlePlayAppStoreId = null;
        #pragma warning restore 0649

        public override string DisplayName => "Lost Ads";
        public override bool IsInline => false;

        public override void GetRuntimeConfigSettings(AppConfig.AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            var settings = appConfig.GetSettings<LostAdsSettings>();

            if (settings != null)
            {
                runtimeConfigSettings.Add(LostAdsSettingsRuntime.InitializeAtStartupKey, settings.initializeAtStartup ? "1" : "0");
                runtimeConfigSettings.Add(LostAdsSettingsRuntime.RegisterUnityAdsProviderKey, settings.registerUnityAdsProvider ? "1" : "0");
                runtimeConfigSettings.Add(LostAdsSettingsRuntime.AppleAppStoreIdKey, settings.appleAppStoreId);
                runtimeConfigSettings.Add(LostAdsSettingsRuntime.GooglePlayAppStoreIdKey, settings.googlePlayAppStoreId);
            }
        }
    }
}
