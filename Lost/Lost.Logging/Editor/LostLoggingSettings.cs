//-----------------------------------------------------------------------
// <copyright file="LostLoggingSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(42)]
    public class LostLoggingSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool initializeAtStartup = true;
        [SerializeField] private bool forwardLoggingAsAnalyticsEvents = true;
        [SerializeField] private bool dontforwardInfoLevelLogging = true;
        #pragma warning restore 0649

        public override string DisplayName => "Lost Logging";
        public override bool IsInline => false;

        public override void GetRuntimeConfigSettings(AppConfig.AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            var settings = appConfig.GetSettings<LostLoggingSettings>();

            if (settings != null)
            {
                runtimeConfigSettings.Add(LostLoggingSettingsRuntime.InitializeAtStartupKey, settings.initializeAtStartup ? "1" : "0");
                runtimeConfigSettings.Add(LostLoggingSettingsRuntime.ForwardLoggingAsAnalyticsEventsKey, settings.forwardLoggingAsAnalyticsEvents ? "1" : "0");
                runtimeConfigSettings.Add(LostLoggingSettingsRuntime.DontforwardInfoLevelLoggingKey, settings.dontforwardInfoLevelLogging ? "1" : "0");
            }
        }
    }
}
