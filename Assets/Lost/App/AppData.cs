//-----------------------------------------------------------------------
// <copyright file="AppData.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Serialization;

    [CreateAssetMenu]
    public class AppData : ScriptableObject
    {
        #pragma warning disable 0649
        [SerializeField] private Version[] versions;
        [SerializeField] private DevicePlatform[] supportedPlatforms;

        [Header("Playfab")]
        [SerializeField] private bool usePlayfab;
        [SerializeField] private string playfabSecretKey;
        [SerializeField] private string playfabAppId;
        #pragma warning restore 0649

        private Version currentVersion;

        public Version[] Versions
        {
            get { return this.versions; }
        }

        public DevicePlatform[] SupoortedPlatforms
        {
            get { return this.supportedPlatforms; }
        }
        
        public Version CurrentVersion
        {
            get 
            {
                if (this.currentVersion == null)
                {
                    this.currentVersion = this.Versions.LastOrDefault();
                }

                return this.currentVersion;
            }
        }
        
        public bool UsePlayfab
        {
            get { return this.usePlayfab; }
        }
               
        public string PlayfabAppId
        {
            get { return this.playfabAppId; }
        }
        
        public string PlayfabSecretKey
        {
            get { return this.playfabSecretKey; }
        }
        
        //// TODO implement later when have custom property drawers
        //// public LocLanguage DefaultLanguage;
        //// public LocLanguage[] SupportedLanguages;

        [Serializable]
        public class Version
        {
            #pragma warning disable 0649
            [FormerlySerializedAs("VersionString")]
            [SerializeField] private string versionString;
            #pragma warning restore 0649

            public string VersionString
            {
                get { return this.versionString; }
            }

            //// TODO implement later when have custom property drawers
            //// public LocString Summary;
            //// public LocString[] Features;
            //// public DateTime ReleaseDate;
        }
    }
}
