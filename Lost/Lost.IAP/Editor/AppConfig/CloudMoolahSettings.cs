//-----------------------------------------------------------------------
// <copyright file="CloudMoolahSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(510)]
    public class CloudMoolahSettings : AppConfigSettings
    {
        // TODO [bgish]: Need to print warning if Target Store is cloud moolah, but no cloud moolah config found

        #pragma warning disable 0649
        [SerializeField] private string appKey;                            // Needs Runtime Access
        [SerializeField] private string hashKey;                           // Needs Runtime Access
        [SerializeField] private string notificationURL;                   // Needs Runtime Access
        [SerializeField] private bool useDebugSetingsForDevelopmentBuilds; // Needs Runtime Access
        #pragma warning restore 0649

        public override string DisplayName => "Cloud Moolah";
        public override bool IsInline => false;

        //// var module = StandardPurchasingModule.Instance();
        //// var builder = ConfigurationBuilder.Instance(module);
        ////
        //// // Configure CloudMoolah (https://docs.unity3d.com/Manual/UnityIAPMoolahMooStore.html)
        //// builder.Configure<IMoolahConfiguration>().appKey = "d93f4564c41d463ed3d3cd207594ee1b";
        //// builder.Configure<IMoolahConfiguration>().hashKey = "cc";
        //// builder.Configure<IMoolahConfiguration>().notificationURL = "https://gameserver.example.com/callback";
        //// builder.Configure<IMoolahConfiguration>().SetMode(CloudMoolahMode.Production);
        //// builder.Configure<IMoolahConfiguration>().SetMode(CloudMoolahMode.AlwaysSucceed); // TESTING: auto-approves all transactions
        //// builder.Configure<IMoolahConfiguration>().SetMode(CloudMoolahMode.AlwaysFailed); // TESTING: always fails all transactions
    }
}
