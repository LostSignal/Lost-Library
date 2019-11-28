//-----------------------------------------------------------------------
// <copyright file="RealtimeMessagingSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(600)]
    public class RealtimeMessagingSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool initializeAtStartup = true;
        [SerializeField] private string ablyKey;
        #pragma warning restore 0649

        public override string DisplayName => "Realtime Messaging";
        public override bool IsInline => false;

        public override void GetRuntimeConfigSettings(AppConfig.AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            var settings = appConfig.GetSettings<RealtimeMessagingSettings>();

            if (settings != null)
            {
                runtimeConfigSettings.Add(RealtimeMessagingSettingsRuntime.InitializeAtStartupKey, settings.initializeAtStartup ? "1" : "0");
                runtimeConfigSettings.Add(RealtimeMessagingSettingsRuntime.AblyKeyKey, settings.ablyKey);
            }
        }
    }
}
