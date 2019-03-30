//-----------------------------------------------------------------------
// <copyright file="LostAnalyticsSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(41)]
    public class LostAnalyticsSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool initializeLostAnalyticsAtStartup = true;

        #if USING_PLAYFAB_SDK
        [SerializeField] private bool registerPlayFabAnalyticsProvider = true;
        #endif
        #pragma warning restore 0649

        public override string DisplayName => "Lost Analytics";
        public override bool IsInline => false;

        public override void GetRuntimeConfigSettings(AppConfig.AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            var settings = appConfig.GetSettings<LostAnalyticsSettings>();

            if (settings != null)
            {
                runtimeConfigSettings.Add(LostAnalyticsSettingsRuntime.InitializeAtStartupKey, settings.initializeLostAnalyticsAtStartup ? "1" : "0");

                #if USING_PLAYFAB_SDK
                runtimeConfigSettings.Add(LostAnalyticsSettingsRuntime.RegisterPlayFabAnalyticsProvider, settings.registerPlayFabAnalyticsProvider ? "1" : "0");
                #endif
            }
        }
    }
}
