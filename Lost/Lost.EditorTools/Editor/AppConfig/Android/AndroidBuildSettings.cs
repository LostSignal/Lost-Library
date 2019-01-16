//-----------------------------------------------------------------------
// <copyright file="AndroidBuildSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(300)]
    public class AndroidBuildSettings : AppConfigSettings
    {
        public enum BuildType
        {
            Gradle,
            Internal,
        }

        #pragma warning disable 0649
        [SerializeField] private BuildType buildType;
        [SerializeField] private bool splitApksByTargetArchitecture;
        [SerializeField] private bool generateArmV7;
        [SerializeField] private bool generateArm64;
        [SerializeField] private bool generateX86;
        #pragma warning restore 0649

        public override string DisplayName => "Android Build Ouput";
        public override bool IsInline => false;
    }
}
