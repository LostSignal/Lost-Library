//-----------------------------------------------------------------------
// <copyright file="LimitScreenSizeSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(52)]
    public class LimitScreenSizeSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool limitScreenSize = true;
        #pragma warning restore 0649

        public override string DisplayName => "Limit Screen Size";
        public override bool IsInline => false;

        public override void GetRuntimeConfigSettings(AppConfig.AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            var settings = appConfig.GetSettings<LimitScreenSizeSettings>();

            if (settings != null)
            {
                runtimeConfigSettings.Add(LimitScreenSizeSettingsRuntime.LimitScreenSizeKey, settings.limitScreenSize ? "1" : "0");
            }
        }
    }
}
