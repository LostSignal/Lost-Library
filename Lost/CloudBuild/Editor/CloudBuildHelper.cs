//-----------------------------------------------------------------------
// <copyright file="CloudBuildHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;

    public static class CloudBuildHelper
    {
        public static void PreExport(string configName)
        {
            Debug.LogFormat("CloudBuildHelper.PreExport({0})", configName);

            UpdateActiveConfig(configName);

            if (AppSettings.ActiveConfig.Name != configName)
            {
                Debug.LogErrorFormat("Unable to set AppSettings.ActiveConfig to {0}", configName);
                return;
            }

            Debug.LogFormat("Active Config = {0}", AppSettings.ActiveConfig.Name);

            AppendCommitIdToBundleVersion();

            // TODO [bgish]: need to take into account building asset bundles
        }

        public static string BuildAssetBundles(bool buildToStreamingAssets)
        {
            var assetBundlePath = buildToStreamingAssets ? GetAssetBundleStreamingAssetsPath() : GetAssetBundlePath();

            // creating the platform directory inside the asset bundle directory
            var assetBundlePlatformPath = Path.Combine(assetBundlePath, AssetBundleUtility.GetPlatformName()).Replace("\\", "/");
            Directory.CreateDirectory(assetBundlePlatformPath);

            // building asset bundles
            BuildAssetBundleOptions options = GetAssetBundleBuildOptions();
            BuildPipeline.BuildAssetBundles(assetBundlePlatformPath, options, EditorUserBuildSettings.activeBuildTarget);

            // no need to import asset bundles if they don't live in the assets folder
            if (buildToStreamingAssets)
            {
                // making sure all those asset bundles get imported and unity is aware of them
                foreach (string assetBundleFile in Directory.GetFiles(assetBundlePath, "*", SearchOption.AllDirectories))
                {
                    var assetPath = assetBundleFile.Replace("\\", "/");

                    if (assetPath.EndsWith(".meta"))
                    {
                        continue;
                    }

                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }

                AssetDatabase.Refresh();
            }

            return assetBundlePath;
        }

        public static BuildAssetBundleOptions GetAssetBundleBuildOptions()
        {
            var options = BuildAssetBundleOptions.None;

            #if UNITY_CLOUD_BUILD
            options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            #endif

            // The following code used to live inside AssetBundleManager, but I moved it out to here
            bool shouldCheckODR = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS || EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS;
            
            if (shouldCheckODR)
            {
                #if ENABLE_IOS_ON_DEMAND_RESOURCES
                if (PlayerSettings.iOS.useOnDemandResources)
                {
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                }
                #endif
            
                #if ENABLE_IOS_APP_SLICING
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                #endif
            }
            
            return options;
        }

        private static string GetAssetBundleStreamingAssetsPath()
        {
            var streamingAssetsPath = "Assets/StreamingAssets";
            Directory.CreateDirectory(streamingAssetsPath);

            var streamingAssetsBundlesPath = streamingAssetsPath + "/" + AssetBundleUtility.AssetBundlesFolderName;
            Directory.CreateDirectory(streamingAssetsPath);

            return streamingAssetsBundlesPath;
        }

        private static string GetAssetBundlePath()
        {
            Directory.CreateDirectory(AssetBundleUtility.AssetBundlesFolderName);
            return AssetBundleUtility.AssetBundlesFolderName;
        }
        
        private static void AppendCommitIdToBundleVersion()
        {
            Debug.Log("CloudBuildHelper.AppendCommitIdToBundleVersion()");
            Debug.LogFormat("AppSettings.ActiveConfig.AppendCommitToVersion = {0}", AppSettings.ActiveConfig.AppendCommitToVersion);

            if (AppSettings.ActiveConfig.AppendCommitToVersion == false)
            {
                return;
            }

            var cloudBuildManifest = Lost.CloudBuildManifest.Find();

            if (cloudBuildManifest == null)
            {
                Debug.LogError("Cloud Build Manifest is NULL!");
                return;
            }
            else if (string.IsNullOrEmpty(cloudBuildManifest.ScmCommitId))
            {
                Debug.LogError("Cloud Build Manifest has a Null or Empty ScmCommitId!");
                return;
            }

            Debug.LogFormat("PlayerSettings.bundleVersion = {0}", PlayerSettings.bundleVersion);
            Debug.LogFormat("cloudBuildManifest.ScmCommitId = {0}", cloudBuildManifest.ScmCommitId);

            if (AppSettings.ActiveConfig.AppendCommitToVersion)
            {
                PlayerSettings.bundleVersion = string.Format("{0}.{1}", PlayerSettings.bundleVersion, cloudBuildManifest.ScmCommitId);
                Debug.LogFormat("New PlayerSettings.bundleVersion: {0}", PlayerSettings.bundleVersion);
            }
        }

        private static void WriteBundleVersionToVersionResourceFile()
        {
            // writing the version to a text file in the resources directory
            var asset = "Assets/Resources/version.txt";
            File.WriteAllText(asset, PlayerSettings.bundleVersion);
            AssetDatabase.ImportAsset(asset);
            AssetDatabase.Refresh();
        }

        private static void UpdateActiveConfig(string configName)
        {
            Debug.LogFormat("CloudBuildHelper.UpdateActiveConfig({0})", configName);

            // setting all the build configs to inactive
            AppSettings.Instance.BuildConfigs.ForEach(x => x.IsActive = false);

            // getting the one config that should be active
            var activeConfig = AppSettings.Instance.BuildConfigs.FirstOrDefault(x => x.Name == configName);

            if (activeConfig == null)
            {
                Debug.LogErrorFormat("Unable to find BuildConfig {0}!", configName);
                return;
            }
            
            // updating AppSettings and saving
            activeConfig.IsActive = true;
            EditorUtility.SetDirty(AppSettings.Instance);
            AssetDatabase.SaveAssets();

            // updating defines
            AppSettingsEditor.UpdateProjectDefines();

            //// TODO [bgish]: need to find a way to save the new project settings
        }

        [PostProcessBuild]
        private static void DisableBitCode(BuildTarget buildTarget, string path)
        {
            Debug.LogFormat("CloudBuildHelper.DisableBitCode({0}, {1})", buildTarget, path);
            Debug.LogFormat("Active Config = {0}", AppSettings.ActiveConfig.Name);
            Debug.LogFormat("DisableBitCode = {0}", AppSettings.ActiveConfig.DisableBitCode);
            
            #if UNITY_IOS
            if (buildTarget == BuildTarget.iOS && AppSettings.ActiveConfig.DisableBitCode)
            {
                string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

                var pbxProject = new UnityEditor.iOS.Xcode.PBXProject();
                pbxProject.ReadFromFile(projectPath);

                string target = pbxProject.TargetGuidByName("Unity-iPhone");
                pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

                pbxProject.WriteToFile(projectPath);
            }
            #endif
        }
    }
}
