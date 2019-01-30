//-----------------------------------------------------------------------
// <copyright file="UploadAddressableToAzureSettings.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Addressables
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Lost.AppConfig;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Build.Reporting;

    ////
    //// TODO [bgish]: Since all AssetBundles have their Hash appended to the name, before uploading any files to Azure, we
    ////                 should get the list of all existing asset bundles and make sure to not send up any duplicates,
    ////

    [AppConfigSettingsOrder(95)]
    public class UploadAddressableToAzureSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private string assetBundleFolderName = "AssetBundles/Azure";
        [SerializeField] private string storageAccountName;
        [SerializeField] private string storageAccountKey;
        [SerializeField] private string containerName;
        [SerializeField] private string blobPrefix;
        [SerializeField] private string downloadUrl; // http://www.myassets.com
        #pragma warning restore 0649

        public override string DisplayName => "Upload Addressables To AzureStorage";
        public override bool IsInline => false;

        // [Lost.Addressables.UploadAddressableToAzureSettings.BuildPath]
        public static string BuildPath
        {
            get
            {
                var azureSettings = EditorAppConfig.ActiveAppConfig?.GetSettings<UploadAddressableToAzureSettings>();
                return azureSettings?.CalculateBuildPath();
            }
        }

        // [Lost.Addressables.UploadAddressableToAzureSettings.LoadPath]
        public static string LoadPath
        {
            get
            {
                var azureSettings = EditorAppConfig.ActiveAppConfig?.GetSettings<UploadAddressableToAzureSettings>();
                return azureSettings != null ? (SanitizeAndRemoveTrailingForwardSlash(azureSettings.downloadUrl) + "/" + GetBlobPrefix(azureSettings)) : null;
            }
        }

        public override void OnUnityCloudBuildInitiated(AppConfig appConfig)
        {
            #if USING_UNITY_ADDRESSABLES
            // Making sure the addressables are built
            UnityEditor.AddressableAssets.AddressableAssetSettings.BuildPlayerContent();
            #endif
        }

        public override void OnUserBuildInitiated(AppConfig appConfig)
        {
            #if USING_UNITY_ADDRESSABLES
            // Making sure the addressables are built
            UnityEditor.AddressableAssets.AddressableAssetSettings.BuildPlayerContent();
            #endif
        }

        public override void OnPostprocessBuild(AppConfig appConfig, BuildReport buildReport)
        {
            var uploadToAzureSettings = appConfig.GetSettings<UploadAddressableToAzureSettings>();

            var azureConfig = new AzureStorage.Config
            {
                StorageAccountName = uploadToAzureSettings.storageAccountName,
                StorageAccountKey = uploadToAzureSettings.storageAccountKey,
                ContainerName = uploadToAzureSettings.containerName,
            };

            // makding sure the asset bundle path doesn't end with a forward slash
            string assetBundlePath = uploadToAzureSettings.CalculateBuildPath();

            if (Directory.Exists(assetBundlePath) == false)
            {
                if (this.IsBeingUsedByAddressableSystem())
                {
                    Debug.LogErrorFormat("Unable to Upload AssetBundles To Azure.  Directory {0} does not exist.", assetBundlePath);
                }

                return;
            }

            string blobPrefix = GetBlobPrefix(uploadToAzureSettings);
            HashSet<string> existingFiles = GetExistingFiles(azureConfig, blobPrefix);

            foreach (var filePath in Directory.GetFiles(assetBundlePath, "*", SearchOption.AllDirectories))
            {
                string relativeFileName = filePath.Substring(assetBundlePath.Length + 1).Replace("\\", "/");
                string blobName = blobPrefix + "/" + relativeFileName;

                if (existingFiles.Contains(blobName))
                {
                    Debug.LogFormat("Skipping AssetBundle {0}, it already exists", blobName, azureConfig.StorageAccountName);
                    continue;
                }

                Debug.LogFormat("Uploading AssetBundle {0}", blobName, azureConfig.StorageAccountName);

                try
                {
                    AzureStorage.UploadFile(azureConfig, blobName, File.ReadAllBytes(filePath));
                }
                catch (Exception exception)
                {
                    Debug.LogErrorFormat("Error uploading AssetBundle {0} To Azure.", blobName);
                    Debug.LogException(exception);

                    #if UNITY_CLOUD_BUILD
                    EditorApplication.Exit(1);
                    #endif

                    return;
                }
            }
        }

        private static string GetBlobPrefix(UploadAddressableToAzureSettings settings)
        {
            string blobPrefix = string.Empty;

            // making sure it uses all forward slashes and doesn't start with a forward slash
            if (string.IsNullOrWhiteSpace(settings.blobPrefix) == false)
            {
                blobPrefix = SanitizeAndRemoveTrailingForwardSlash(settings.blobPrefix) + "/";
                blobPrefix = blobPrefix.StartsWith("/") ? blobPrefix.Substring(1) : blobPrefix;
            }

            var cloudBuildConfig = CloudBuildManifest.Find();

            string buildFolder = cloudBuildConfig != null ?
                "cloud_build/" + cloudBuildConfig.ScmCommitId + "/" :
                "local_build/" + Environment.MachineName + "/"; // TODO [bgish]: Make sure to sanitize for special characters;

            return blobPrefix + buildFolder + EditorUserBuildSettings.activeBuildTarget;
        }

        private static string SanitizeAndRemoveTrailingForwardSlash(string str)
        {
            if (str == null)
            {
                return null;
            }

            str = str.Replace("\\", "/");
            return str.EndsWith("/") ? str.Substring(0, str.Length - 1) : str;
        }

        private static HashSet<string> GetExistingFiles(AzureStorage.Config azureConfig, string blobPrefix)
        {
            HashSet<string> result = new HashSet<string>();

            if (blobPrefix.EndsWith("/") == false)
            {
                blobPrefix += "/";
            }

            foreach (string file in AzureStorage.ListFiles(azureConfig, blobPrefix))
            {
                result.Add(file);
            }

            return result;
        }

        private string CalculateBuildPath()
        {
            return SanitizeAndRemoveTrailingForwardSlash(this.assetBundleFolderName) + "/" + EditorUserBuildSettings.activeBuildTarget;
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
