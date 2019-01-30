//-----------------------------------------------------------------------
// <copyright file="AndroidKeystoreSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEngine;

    [AppConfigSettingsOrder(310)]
    public class AndroidKeystoreSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [Header("KeyStore")]
        [SerializeField] public string keystoreFile;  // relative path
        [SerializeField] public string keystorePassword;

        [Header("KeyAlias")]
        [SerializeField] public string keyAliasName;
        [SerializeField] public string keyAliasePassword;
        #pragma warning restore 0649

        public override string DisplayName => "Android Keystore";
        public override bool IsInline => false;

        public override void InitializeOnLoad(AppConfig.AppConfig appConfig)
        {
            // Key Store
            if (string.IsNullOrWhiteSpace(this.keystoreFile) == false)
            {
                PlayerSettings.Android.keystoreName = this.keystoreFile;
            }

            if (string.IsNullOrWhiteSpace(this.keystorePassword) == false)
            {
                PlayerSettings.Android.keystorePass = this.keystorePassword;
            }

            // Key Alias
            if (string.IsNullOrWhiteSpace(this.keyAliasName) == false)
            {
                PlayerSettings.Android.keyaliasName = this.keyAliasName;
            }

            if (string.IsNullOrWhiteSpace(this.keyAliasePassword) == false)
            {
                PlayerSettings.Android.keyaliasPass = this.keyAliasePassword;
            }
        }
    }
}
