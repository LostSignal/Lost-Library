//-----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Serialization;

    [CreateAssetMenu]
    public class AppSettings : SingletonScriptableObjectResource<AppSettings>
    {
        #pragma warning disable 0649
        [SerializeField] private Version[] versions;
        [SerializeField] private DevicePlatform[] supportedPlatforms;
        [SerializeField] private List<Define> customDefines = new List<Define>();
        [SerializeField] private List<Define> lostDefines = new List<Define>();

        [Header("Playfab")]
        [SerializeField] private bool usePlayfab;
        [SerializeField] private string playfabSecretKey;
        [SerializeField] private string playfabAppId;
        #pragma warning restore 0649
        
        private bool configuredLostDefines;
        private Version currentVersion;

        public List<Define> LostDefines
        {
            get
            {
                if (this.configuredLostDefines == false)
                {
                    this.ConfigureLostDefines();
                }

                return this.lostDefines;
            }
        }

        public List<Define> CustomDefines
        {
            get { return this.customDefines; }
        }

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

        private void ConfigureLostDefines()
        {
            this.AddLostDefine(new Define("UNITY"));
            this.AddLostDefine(new Define("USE_PLAYFAB_SDK"));
            this.AddLostDefine(new Define("USE_PLAYFAB_ANDROID_SDK"));
            this.AddLostDefine(new Define("USE_FACEBOOK_SDK"));
            this.AddLostDefine(new Define("USE_TEXTMESH_PRO"));
            this.configuredLostDefines = true;
        }

        private void AddLostDefine(Define define)
        {
            if (this.lostDefines.FirstOrDefault(x => x.Name == define.Name) == null)
            {
                this.lostDefines.Add(define);
            }
        }

        [Serializable]
        public class Define
        {
            #pragma warning disable 0649
            [SerializeField] private string name;
            [SerializeField] private bool enabled;
            #pragma warning restore 0649
            
            public Define(string name)
            {
                this.name = name;
            }

            public string Name
            {
                get { return this.name; }
                
                #if UNITY_EDITOR
                set { this.name = value; }
                #endif
            }

            public bool Enabled
            {
                get { return this.enabled; }

                #if UNITY_EDITOR
                set { this.enabled = value; }
                #endif
            }
        }
    }
}
