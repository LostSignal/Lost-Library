//-----------------------------------------------------------------------
// <copyright file="DevelopmentBuildSetting.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEngine;

    [AppConfigSettingsOrder(10)]
    public class DevelopmentBuildSetting : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool isDevelopmentBuild;
        #pragma warning restore 0649

        public override string DisplayName => "Development Build";
        public override bool IsInline => true;

        public override void InitializeOnLoad(AppConfig.AppConfig appConfig)
        {
            // var settings = appConfig.GetSettings<DevelopmentBuildSetting>();
            // EditorUserBuildSettings.development = settings.isDevelopmentBuild;
        }

        public override BuildPlayerOptions ChangeBuildPlayerOptions(AppConfig.AppConfig appConfig, BuildPlayerOptions buildPlayerOptions)
        {
            // var settings = appConfig.GetSettings<DevelopmentBuildSetting>();
            //
            // if (settings.isDevelopmentBuild)
            // {
            //     buildPlayerOptions.options |= BuildOptions.Development;
            // }
            // else
            // {
            //     buildPlayerOptions.options &= ~BuildOptions.Development;
            // }
            //

            return buildPlayerOptions;
        }
    }
}
