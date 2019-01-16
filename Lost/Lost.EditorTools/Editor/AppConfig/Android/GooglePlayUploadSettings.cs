//-----------------------------------------------------------------------
// <copyright file="GooglePlayUploadSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(350)]
    public class GooglePlayUploadSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private string username;
        [SerializeField] private string password;
        #pragma warning restore 0649

        public override string DisplayName => "GoolePlay Uplaod";
        public override bool IsInline => false;
    }
}
