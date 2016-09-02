//-----------------------------------------------------------------------
// <copyright file="CloudBuildHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    
    public static class CloudBuildHelper
    {
        public static void DisableBitCode(BuildTarget buildTarget, string path)
        {
            #if UNITY_IOS
            if (buildTarget == BuildTarget.iOS)
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

        public static void AppendCommitIdToBundleVersion()
        {
            var cloudBuildManifest = Lost.CloudBuildManifest.Find();
            PlayerSettings.bundleVersion = string.Format("{0}.{1}", PlayerSettings.bundleVersion, cloudBuildManifest.ScmCommitId);
            Debug.LogFormat("PlayerSettings.bundleVersion: {0}", PlayerSettings.bundleVersion);
        }

        public static void WriteBundleVersionToVersionResourceFile()
        {
            // writing the version to a text file in the resources directory
            var asset = "Assets/Resources/version.txt";
            File.WriteAllText(asset, PlayerSettings.bundleVersion);
            AssetDatabase.ImportAsset(asset);
            AssetDatabase.Refresh();
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
    }
}
