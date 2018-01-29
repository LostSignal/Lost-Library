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

    public enum BuildNumberType
    {
        CommitNumber,
        BuildNumber,
        None,
    }

    public enum IOSPushNotificationType
    {
        None,
        Development,
        Production,
    }

    public enum AppOrientation
    {
        Portrait,
        Landscape,
    }

    [CreateAssetMenu(fileName = "AppSettings")]
    public class AppSettings : SingletonScriptableObjectResource<AppSettings>
    {
        private static BuildConfig activeConfig = null;

        #pragma warning disable 0649
        [SerializeField] private string bundleIdentifier;
        [SerializeField] private AppOrientation appOrientation;
        [SerializeField] private DevicePlatform supportedPlatforms;
        [SerializeField] private List<BuildConfig> buildConfigs = new List<BuildConfig>();
        [SerializeField] private List<Define> lostDefines = new List<Define>();
        [SerializeField] private List<string> projectDefines = new List<string>();
        
        // project
        [SerializeField] private BuildNumberType buildNumberType;
        [SerializeField] private bool warningsAsErrors = false;
        [SerializeField] private LineEndings projectLineEndings = LineEndings.Windows;
        [SerializeField] private bool overrideTemplateCShardFiles = false;

        // asset bundles
        [SerializeField] private bool buildAssetBundles;
        [SerializeField] private bool copyToStreamingAssets;
        [SerializeField] private bool uploadAssetBundles;
        [SerializeField] private AssetBundleUploadType uploadType;
        
        // Source Control
        [SerializeField] private SourceControlType sourceControl = SourceControlType.None;

        // Perforce
        [SerializeField] private bool useP4IgnoreFile = false;
        [SerializeField] private string p4IgnoreFileName = ".p4ignore";
        [SerializeField] private bool setP4IgnoreVariableAtStartup = false;
        
        //// TODO [bgish]:  Need add these settings
        //// ftp -  (string assetBundleRelativeDirectory, string ftpUrl, string username, string password)
        //// s3 -   (string accessKey, string secretKey, string bucket, string folder, string key)
        //// playfab - (string directoryName)
        #pragma warning restore 0649
            
        private CloudBuildManifest cloudBuildManifest;
        private bool configuredLostDefines;
        private string versionAndBuildNumber;

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
            #if UNITY_EDITOR
            get { return UnityEditor.PlayerSettings.bundleVersion; }
            set { UnityEditor.PlayerSettings.bundleVersion = value; }
            #else
            get { return Application.version; }
            #endif
        }
        
        public string BundleIdentifier
        {
            get { return this.bundleIdentifier; }
            
            #if UNITY_EDITOR
            set { this.bundleIdentifier = value; }
            #endif
        }
        
        public AppOrientation AppOrientation
        {
            get { return this.appOrientation; }
            
            #if UNITY_EDITOR
            set { this.appOrientation = value; }
            #endif
        }

        public BuildNumberType BuildNumberType
        {
            get { return this.buildNumberType; }
            
            #if UNITY_EDITOR
            set { this.buildNumberType = value; }
            #endif
        }

        public int BuildNumber
        {
            get
            {
                if (this.buildNumberType == BuildNumberType.None)
                {
                    return 0;
                }

                if (this.cloudBuildManifest == null)
                {
                    this.cloudBuildManifest = CloudBuildManifest.Find();
                }
                
                if (this.cloudBuildManifest == null)
                {
                    return 0;
                }
                else if (this.buildNumberType == BuildNumberType.BuildNumber)
                {
                    return this.cloudBuildManifest.BuildNumber;
                }
                else if (this.buildNumberType == BuildNumberType.CommitNumber)
                {
                    string commitId = cloudBuildManifest.ScmCommitId;
                    int commitNumber;

                    if (int.TryParse(commitId, out commitNumber) == false)
                    {
                        Debug.LogErrorFormat("AppSettings.BuildNumber couldn't parse build number {0}.  It is not a valid integer!", commitId);
                        return 0;
                    }

                    return commitNumber;
                }

                Debug.LogErrorFormat("Found unknown BuildNumberType {0}", this.buildNumberType.ToString());
                return 0;
            }
        }
        
        public string VersionAndBuildNumber
        {
            get
            {
                if (this.versionAndBuildNumber == null)
                {
                    this.versionAndBuildNumber = this.BuildNumber == 0 ? this.Version : string.Format("{0} ({1})", this.Version, this.BuildNumber);
                }

                return this.versionAndBuildNumber;
            }
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

            [Header("iOS Related Properties")]
            [SerializeField] private bool disableIOSBitCode;
            [SerializeField] private IOSPushNotificationType iosPushNotificationType;
            
            #if USE_PLAYFAB_SDK
            [SerializeField] private string playfabTitleId;
            [SerializeField] private string catalogVersion;
            [SerializeField] private int cloudScriptRevision;
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
            
            public bool DisableIOSBitCode
            {
                get { return this.disableIOSBitCode; }
                set { this.disableIOSBitCode = value; }
            }

            public IOSPushNotificationType IOSPushNotificationType
            {
                get { return this.iosPushNotificationType; }
                set { this.iosPushNotificationType = value; }
            }

            #if USE_PLAYFAB_SDK
            public string PlayfabTitleId
            {
                get { return this.playfabTitleId; }
                set { this.playfabTitleId = value; }
            }

            public string CatalogVersion
            {
                get { return this.catalogVersion; }
                set { this.catalogVersion = value; }
            }

            public int CloudScriptRevision
            {
                get { return this.cloudScriptRevision; }
                set { this.cloudScriptRevision = value; }
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
