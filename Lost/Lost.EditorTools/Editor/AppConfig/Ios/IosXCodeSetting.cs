//-----------------------------------------------------------------------
// <copyright file="IosXCodeSetting.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(400)]
    public class IosXCodeSetting : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private string xcodeVersion;
        #pragma warning restore 0649

        public override string DisplayName => "XCode Version";
        public override bool IsInline => true;
    }
}
