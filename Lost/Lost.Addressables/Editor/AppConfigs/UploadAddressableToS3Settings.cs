//-----------------------------------------------------------------------
// <copyright file="UploadAddressableToS3Settings.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Addressables
{
    using System;
    using System.IO;
    using System.Linq;
    using Lost.AppConfig;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Build.Reporting;

    [AppConfigSettingsOrder(90)]
    public class UploadAddressableToS3Settings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] public string AssetBundleFolderName = "S3";
        [SerializeField] public string BucketName;  // myGamesAssetBundlesBucket
        [SerializeField] public string KeyPrefix;   // Whatever you want
        [SerializeField] public string AccessKeyId; // blah...
        [SerializeField] public string SecretKeyId; // blah...
        [SerializeField] public string DownloadUrl; // http://www.myassets.com
        #pragma warning restore 0649

        public override string DisplayName => "Upload Addressables To S3";
        public override bool IsInline => false;

        // [Lost.Addressables.UploadAddressableToS3Settings.BuildPath]
        public static string BuildPath
        {
            get
            {
                var s3Settings = EditorAppConfig.ActiveAppConfig?.GetSettings<UploadAddressableToS3Settings>();
                return s3Settings?.AssetBundleFolderName;
            }
        }

        // [Lost.Addressables.UploadAddressableToS3Settings.LoadPath]
        public static string LoadPath
        {
            get
            {
                var s3Settings = EditorAppConfig.ActiveAppConfig?.GetSettings<UploadAddressableToS3Settings>();
                return s3Settings != null ? (s3Settings.DownloadUrl + "/" + GetKeyPrefix(s3Settings)) : null;
            }
        }

        public override void OnPostprocessBuild(AppConfig appConfig, BuildReport buildReport)
        {
            var s3Settings = GetSettings();

            var s3Config = new S3.Config
            {
                AccessKeyId = s3Settings.AccessKeyId,
                BucketName = s3Settings.BucketName,
                SecretKeyId = s3Settings.SecretKeyId,
            };

            foreach (var filePath in Directory.GetFiles(s3Settings.AssetBundleFolderName, "*", SearchOption.AllDirectories))
            {
                S3.UploadFile(s3Config, s3Settings.KeyPrefix + Path.GetFileName(filePath), File.ReadAllBytes(filePath), false);
            }
        }

        private static string GetKeyPrefix(UploadAddressableToS3Settings settings)
        {
            string prefix = string.IsNullOrWhiteSpace(settings.KeyPrefix) ? string.Empty : (settings.KeyPrefix + "/");

            var cloudBuildConfig = CloudBuildManifest.Find();
            string buildFolder = cloudBuildConfig != null ?
                "cloud_build/" + cloudBuildConfig.ScmCommitId + "/" :
                "local_build/" + Environment.MachineName + "/"; // Make sure to sanitize for special characters;

            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

            return prefix + buildFolder + buildTargetGroup;
        }

        private static UploadAddressableToS3Settings GetSettings()
        {
            return EditorAppConfig.ActiveAppConfig?.GetSettings<UploadAddressableToS3Settings>();
        }

        private bool IsBeingUsedByAddressableSystem()
        {
            #if USING_UNITY_ADDRESSABLES
            UnityEditor.AddressableAssets.AddressableAssetSettings settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            string buildPath = this.GetType().FullName + "." + nameof(BuildPath);

            if (settings == null)
            {
                return false;
            }

            foreach (var group in settings.groups.Where(x => x.entries.IsNullOrEmpty() == false))
            {
                foreach (var schema in group.Schemas.OfType<UnityEditor.AddressableAssets.BundledAssetGroupSchema>())
                {
                    if (schema?.BuildPath?.GetValue(settings) == buildPath)
                    {
                        return true;
                    }
                }
            }
            #endif

            return false;
        }
    }
}
