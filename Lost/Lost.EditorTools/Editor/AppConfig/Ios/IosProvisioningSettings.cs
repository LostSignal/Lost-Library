//-----------------------------------------------------------------------
// <copyright file="IosProvisioningSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(410)]
    public class IosProvisioningSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private string provisitionProfile; // relative path
        [Tooltip("Can be found here: https://developer.apple.com/account/#/membership")]
        [SerializeField] private string teamId;

        // These may not be needed for signing
        [SerializeField] private string p12File;   // relative path
        [SerializeField] private string password;
        #pragma warning restore 0649

        public override string DisplayName => "iOS Provisioning";
        public override bool IsInline => false;
    }
}
