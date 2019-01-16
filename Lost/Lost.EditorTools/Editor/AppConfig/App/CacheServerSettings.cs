//-----------------------------------------------------------------------
// <copyright file="CacheServerSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    //// TODO [bgish]: Maybe some day specify project cache server here and it will auto connect to it for you (if available),
    ////               instead of forcing the user to manually apply it in the Preferences.
    //// Cache Server  (https://github.com/MattRix/UnityDecompiled/blob/master/UnityEditor/UnityEditor/CacheServerPreferences.cs)
    //// bool useCacheServer
    //// string ip;

    [AppConfigSettingsOrder(15)]
    public class CacheServerSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private string cacheServerIp;
        #pragma warning restore 0649

        public override string DisplayName => "Cache Server";
        public override bool IsInline => true;
    }
}
