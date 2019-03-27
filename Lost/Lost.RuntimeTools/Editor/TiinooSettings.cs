//-----------------------------------------------------------------------
// <copyright file="TiinooSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_TIINOO

namespace Lost
{
    using System.Collections.Generic;
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(54)]
    public class TiinooSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool loadAtStartup = true;
        #pragma warning restore 0649

        public override string DisplayName => "Tiinoo";
        public override bool IsInline => false;

        public override void GetRuntimeConfigSettings(AppConfig.AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            var settings = appConfig.GetSettings<TiinooSettings>();

            if (settings != null)
            {
                runtimeConfigSettings.Add(TiinooSettingsRuntime.InitializeAtStartupKey, settings.loadAtStartup ? "1" : "0");
            }
        }
    }
}

#endif
