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
        [SerializeField] private bool useCustomKeystore = true;

        [Header("KeyStore")]
        [SerializeField] private string keystoreFile;  // relative path
        [SerializeField] private string keystorePassword;

        [Header("KeyAlias")]
        [SerializeField] private string keyAliasName;
        [SerializeField] private string keyAliasePassword;
        #pragma warning restore 0649

        public override string DisplayName => "Android Keystore";
        public override bool IsInline => false;

        public override void InitializeOnLoad(AppConfig.AppConfig appConfig)
        {
            var settings = appConfig.GetSettings<AndroidKeystoreSettings>();

            if (settings != null)
            {
                PlayerSettings.Android.useCustomKeystore = settings.useCustomKeystore;
                PlayerSettings.Android.keystoreName = settings.keystoreFile;
                PlayerSettings.Android.keystorePass = settings.keystorePassword;
                PlayerSettings.Android.keyaliasName = settings.keyAliasName;
                PlayerSettings.Android.keyaliasPass = settings.keyAliasePassword;
            }
        }
    }
}
