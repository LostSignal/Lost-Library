//-----------------------------------------------------------------------
// <copyright file="AndroidKeystoreSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(310)]
    public class AndroidKeystoreSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] public string keystoreFile;  // relative path
        [SerializeField] public string keystoreAlias;
        [SerializeField] public string keystorePassword;
        #pragma warning restore 0649

        public override string DisplayName => "Android Keystore";
        public override bool IsInline => false;
    }
}
