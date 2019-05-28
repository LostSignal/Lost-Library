//-----------------------------------------------------------------------
// <copyright file="DebugMenuSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(50)]
    public class DebugMenuSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool initializeAtStartup = true;

        [Header("Settings")]
        [SerializeField] private DebugMenu.DebugMenuSettings settings = new DebugMenu.DebugMenuSettings();

        [Header("Overlay Options")]
        [SerializeField] private bool showAppVersionToLowerLeft = true;
        [SerializeField] private bool showPlayFabIdInLowerRight = true;

        [Header("Debug Menu Options")]
        [SerializeField] private bool showTestAd = true;
        [SerializeField] private bool showToggleFps = true;
        [SerializeField] private bool showPrintAdsInfo = true;
        #pragma warning restore 0649

        public override string DisplayName => "Lost Debug Menu";
        public override bool IsInline => false;

        public override void GetRuntimeConfigSettings(AppConfig.AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            var settings = appConfig.GetSettings<DebugMenuSettings>();

            if (settings != null)
            {
                runtimeConfigSettings.Add(DebugMenuSettingsRuntime.InitializeAtStartupKey, settings.initializeAtStartup ? "1" : "0");

                // Settings
                runtimeConfigSettings.Add(DebugMenuSettingsRuntime.SettingsKey, JsonUtility.ToJson(this.settings));

                // Overlay
                runtimeConfigSettings.Add(DebugMenuSettingsRuntime.ShowAppVersionInLowerLeftKey, settings.showAppVersionToLowerLeft ? "1" : "0");
                runtimeConfigSettings.Add(DebugMenuSettingsRuntime.ShowPlayFabIdInLowerRightKey, settings.showPlayFabIdInLowerRight ? "1" : "0");

                // Menu Options
                runtimeConfigSettings.Add(DebugMenuSettingsRuntime.ShowTestAdKey, settings.showTestAd ? "1" : "0");
                runtimeConfigSettings.Add(DebugMenuSettingsRuntime.ShowToggleFpsKey, settings.showToggleFps ? "1" : "0");
                runtimeConfigSettings.Add(DebugMenuSettingsRuntime.ShowPrintAdsInfoKey, settings.showPrintAdsInfo ? "1" : "0");
            }
        }
    }
}
