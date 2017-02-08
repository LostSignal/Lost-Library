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

    //// TODO [bgish]: Maybe some day specify project cache server here and it will auto connect to it for you (if available),  
    ////               instead of forcing the user to manually apply it in the Preferences.
    //// Cache Server  (https://github.com/MattRix/UnityDecompiled/blob/master/UnityEditor/UnityEditor/CacheServerPreferences.cs)
    //// bool useCacheServer
    //// string ip;

    public enum AssetBundleUploadType
    {
        S3,
        PlayFab,
        FTP
    }

    public enum LineEndings
    {
        Windows,
        Unix
    }

    public enum SourceControlType
    {
        None,
        Perforce
    }

    [CreateAssetMenu]
    public class AppSettings : SingletonScriptableObjectResource<AppSettings>
    {
        private static BuildConfig activeConfig = null;

        #pragma warning disable 0649
        [SerializeField] private string version;
        [SerializeField] private DevicePlatform supportedPlatforms;
        [SerializeField] private List<BuildConfig> buildConfigs = new List<BuildConfig>();
        [SerializeField] private List<Define> lostDefines = new List<Define>();
        [SerializeField] private List<string> projectDefines = new List<string>();
        
        // project
        [SerializeField] private bool warningsAsErrors = true;
        [SerializeField] private LineEndings projectLineEndings = LineEndings.Windows;
        [SerializeField] private bool overrideTemplateCShardFiles = true;

        // asset bundles
        [SerializeField] private bool buildAssetBundles;
        [SerializeField] private bool copyToStreamingAssets;
        [SerializeField] private bool uploadAssetBundles;
        [SerializeField] private AssetBundleUploadType uploadType;
        
        // Source Control
        [SerializeField] private SourceControlType sourceControl = SourceControlType.None;

        // Perforce
        [SerializeField] private bool useP4IgnoreFile = true;
        [SerializeField] private string p4IgnoreFileName = ".p4ignore";
        [SerializeField] private bool setP4IgnoreVariableAtStartup = true;
        
        //// TODO [bgish]:  Need add these settings
        //// ftp -  (string assetBundleRelativeDirectory, string ftpUrl, string username, string password)
        //// s3 -   (string accessKey, string secretKey, string bucket, string folder, string key)
        //// playfab - (string directoryName)
        #pragma warning restore 0649

        private bool configuredLostDefines;
        
        public static BuildConfig ActiveConfig
        {
            get
            {
                if (activeConfig == null || activeConfig.IsActive == false)
                {
                    activeConfig = AppSettings.Instance.BuildConfigs.FirstOrDefault(x => x.IsActive);
                }

                return activeConfig;
            }
        }

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
        
        public string Version
        {
            get { return this.version; }

            #if UNITY_EDITOR
            set { this.version = value; }
            #endif
        }
        
        public List<string> ProjectDefines
        {
            get { return this.projectDefines; }

            #if UNITY_EDITOR
            set { this.projectDefines = value; }
            #endif
        }

        public List<BuildConfig> BuildConfigs
        {
            get { return this.buildConfigs; }
        }

        public DevicePlatform SupoortedPlatforms
        {
            get { return this.supportedPlatforms; }
            
            #if UNITY_EDITOR
            set { this.supportedPlatforms = value; }
            #endif
        }

        public bool WarningsAsErrors
        {
            get { return this.warningsAsErrors; }

            #if UNITY_EDITOR
            set { this.warningsAsErrors = value; }
            #endif
        }
        
        public LineEndings ProjectLineEndings
        {
            get { return this.projectLineEndings; }

            #if UNITY_EDITOR
            set { this.projectLineEndings = value; }
            #endif
        }
        
        public bool OverrideTemplateCShardFiles
        {
            get { return this.overrideTemplateCShardFiles; }

            #if UNITY_EDITOR
            set { this.overrideTemplateCShardFiles = value; }
            #endif
        }

        // Source Control
        public SourceControlType SourceControl
        {
            get { return this.sourceControl; }

            #if UNITY_EDITOR
            set { this.sourceControl = value; }
            #endif
        }
        
        public bool UseP4IgnoreFile
        {
            get { return this.useP4IgnoreFile; }

            #if UNITY_EDITOR
            set { this.useP4IgnoreFile = value; }
            #endif
        }

        public string P4IgnoreFileName
        {
            get { return this.p4IgnoreFileName; }

            #if UNITY_EDITOR
            set { this.p4IgnoreFileName = value; }
            #endif
        }

        public bool SetP4IgnoreVariableAtStartup
        {
            get { return this.setP4IgnoreVariableAtStartup; }

            #if UNITY_EDITOR
            set { this.setP4IgnoreVariableAtStartup = value; }
            #endif
        }

        // Asset Bundles
        public bool BuildAssetBundles
        {
            get { return this.buildAssetBundles; }

            #if UNITY_EDITOR
            set { this.buildAssetBundles = value; }
            #endif
        }
        
        public bool CopyToStreamingAssets
        {
            get { return this.copyToStreamingAssets; }

            #if UNITY_EDITOR
            set { this.copyToStreamingAssets = value; }
            #endif
        }
        
        public bool UploadAssetBundles
        {
            get { return this.uploadAssetBundles; }

            #if UNITY_EDITOR
            set { this.uploadAssetBundles = value; }
            #endif
        }

        public AssetBundleUploadType UploadType
        {
            get { return this.uploadType; }

            #if UNITY_EDITOR
            set { this.uploadType = value; }
            #endif
        }

        //// TODO implement later when have custom property drawers
        //// public LocLanguage DefaultLanguage;
        //// public LocLanguage[] SupportedLanguages;
        //// 
        //// [Serializable]
        //// public class Version
        //// {
        ////     #pragma warning disable 0649
        ////     [FormerlySerializedAs("VersionString")]
        ////     [SerializeField] private string versionString;
        ////     #pragma warning restore 0649
        //// 
        ////     public string VersionString
        ////     {
        ////         get { return this.versionString; }
        ////     }
        //// 
        ////     //// TODO implement later when have custom property drawers
        ////     //// public LocString Summary;
        ////     //// public LocString[] Features;
        ////     //// public DateTime ReleaseDate;
        //// }

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
        public class BuildConfig
        {
            #pragma warning disable 0649
            [SerializeField] private string name;
            [SerializeField] private bool isActive;
            [SerializeField] private bool appendCommitToVersion;
            [SerializeField] private bool disableBitCode;
            
            #if USE_PLAYFAB_SDK
            [SerializeField] private string playfabTitleId;
            #if UNITY_EDITOR
            [SerializeField] private string playfabSecretId;
            #endif
            #endif

            #if USE_PLAYFAB_ANDROID_SDK
            [SerializeField] private string googleAppId;
            #endif
            
            [SerializeField] private List<Define> defines = new List<Define>();
            #pragma warning restore 0649

            public bool IsActive
            {
                get { return this.isActive; }
                set { this.isActive = value; }
            }

            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            public bool AppendCommitToVersion
            {
                get { return this.appendCommitToVersion; }
                set { this.appendCommitToVersion = value; }
            }

            public bool DisableBitCode
            {
                get { return this.disableBitCode; }
                set { this.disableBitCode = value; }
            }

            #if USE_PLAYFAB_SDK
            public string PlayfabTitleId
            {
                get { return this.playfabTitleId; }
                set { this.playfabTitleId = value; }
            }
            
            #if UNITY_EDITOR
            public string PlayfabSecretId
            {
                get { return this.playfabSecretId; }
                set { this.playfabSecretId = value; }
            }
            #endif
            #endif

            #if USE_PLAYFAB_ANDROID_SDK
            public string GoogleAppId
            {
                get { return this.googleAppId; }
                set { this.googleAppId = value; }
            }
            #endif

            public List<Define> Defines
            {
                get { return this.defines; }
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
