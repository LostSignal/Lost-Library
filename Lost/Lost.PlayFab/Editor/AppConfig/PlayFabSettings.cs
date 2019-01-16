//-----------------------------------------------------------------------
// <copyright file="PlayFabSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(500)]
    public class PlayFabSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private string titleId;
        [SerializeField] private string catalogVersion;
        [SerializeField] private int cloudScriptRevision;
        [SerializeField] private string secretKey;
        #pragma warning restore 0649

        public override string DisplayName => "PlayFab";
        public override bool IsInline => false;

        public string TitleId => this.titleId;
        public string CatalogVersion => this.catalogVersion;
        public int CloudScriptRevision => this.cloudScriptRevision;
        public string SecretKey => this.secretKey;

        public override void GetRuntimeConfigSettings(AppConfig.AppConfig buildConfig, Dictionary<string, string> runtimeConfigSettings)
        {
            var playFabSettings = buildConfig.GetSettings<PlayFabSettings>();

            if (playFabSettings == null)
            {
                return;
            }

            runtimeConfigSettings.Add(PlayFabConfigExtensions.TitleId, playFabSettings.titleId);
            runtimeConfigSettings.Add(PlayFabConfigExtensions.CatalogVersion, playFabSettings.catalogVersion);
            runtimeConfigSettings.Add(PlayFabConfigExtensions.CloudScriptRevision, playFabSettings.cloudScriptRevision.ToString());
        }
    }
}
