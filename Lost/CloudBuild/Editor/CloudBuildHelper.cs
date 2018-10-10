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

            Debug.LogFormat("Setting BuildNumber/BundleVersionCode to {0}", AppSettings.Instance.BuildNumber);
            PlayerSettings.iOS.buildNumber = AppSettings.Instance.BuildNumber.ToString();
            PlayerSettings.Android.bundleVersionCode = AppSettings.Instance.BuildNumber;

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

            if (Platform.IsUnityCloudBuild)
            {
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }

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
            Debug.LogFormat("DisableBitCode = {0}", AppSettings.ActiveConfig.DisableIOSBitCode);

            if (buildTarget == BuildTarget.iOS && AppSettings.ActiveConfig.DisableIOSBitCode)
            {
                #if UNITY_IOS
                string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

                var pbxProject = new UnityEditor.iOS.Xcode.PBXProject();
                pbxProject.ReadFromFile(projectPath);

                string target = pbxProject.TargetGuidByName("Unity-iPhone");
                pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

                pbxProject.WriteToFile(projectPath);
                #endif
            }
        }

        // http://answers.unity3d.com/questions/1225564/enable-unity-uses-remote-notifications.html
        // https://answers.unity.com/questions/1224123/enable-push-notification-in-xcode-project-by-defau.html
        // https://forum.unity3d.com/threads/how-to-put-ios-entitlements-file-in-a-unity-project.442277/
        [PostProcessBuild]
        private static void EnableIOSPushNotifications(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS || AppSettings.ActiveConfig.IOSPushNotificationType == IOSPushNotificationType.None)
            {
                return;
            }

            #if UNITY_IOS

            // making sure remote notifications is turned on
            string preprocessorPath = buildPath + "/Classes/Preprocessor.h";
            string text = File.ReadAllText(preprocessorPath);
            text = text.Replace("UNITY_USES_REMOTE_NOTIFICATIONS 0", "UNITY_USES_REMOTE_NOTIFICATIONS 1");
            File.WriteAllText(preprocessorPath, text);

            // var capManager = new UnityEditor.iOS.Xcode.ProjectCapabilityManager(buildPath, "com.example");
            // 
            // if (AppSettings.ActiveConfig.IOSPushNotificationType == IOSPushNotificationType.Development)
            // {
            //     capManager.AddPushNotifications(true);
            // }
            // else if (AppSettings.ActiveConfig.IOSPushNotificationType == IOSPushNotificationType.Production)
            // {
            //     capManager.AddPushNotifications(false);
            // }

            // // creating the entitlements file
            var entitlements = new System.Text.StringBuilder();
            entitlements.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            entitlements.AppendLine("<!DOCTYPE plist PUBLIC \" -//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
            entitlements.AppendLine("<plist version=\"1.0\">");
            entitlements.AppendLine("<dict>");
            entitlements.AppendLine("<key>aps-environment</key>");
            entitlements.AppendLine(string.Format("<string>{0}</string>", AppSettings.ActiveConfig.IOSPushNotificationType.ToString().ToLower()));
            entitlements.AppendLine("</dict>");
            entitlements.AppendLine("</plist>");

            // loading teh pbx project
            var pbxProjectPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(buildPath);
            var pbxProject = new UnityEditor.iOS.Xcode.PBXProject();
            pbxProject.ReadFromFile(pbxProjectPath);

            string fileName = "Push.entitlements";
            string targetName = UnityEditor.iOS.Xcode.PBXProject.GetUnityTargetName();
            string targetGuid = pbxProject.TargetGuidByName(targetName);

            // pbxProject.AddCapability(targetGuid, UnityEditor.iOS.Xcode.PBXCapabilityType.PushNotifications);

            // writing out the entitlements file to disk
            string destinationFilePath = string.Format("{0}/{1}/{2}", buildPath, targetName, fileName);
            Debug.Log("Adding Entitlements " + destinationFilePath);
            Debug.Log(entitlements.ToString());

            File.Delete(destinationFilePath);
            File.WriteAllText(destinationFilePath, entitlements.ToString());

            // adding entitlements to pbx project
            string relativeFilePath = string.Format("{0}/{1}", targetName, fileName);
            pbxProject.AddFile(relativeFilePath, fileName);
            pbxProject.AddBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", relativeFilePath);

            pbxProject.GetBuildPropertyForConfig(targetGuid, "CODE_SIGN_ENTITLEMENTS");

            // saving the pbx project back to disk
            pbxProject.WriteToFile(pbxProjectPath);

            #endif
        }
    }
}


//   /BUILD_PATH/lost-signal-llc.tienlen.tienlen-ios-dev/TienLen_Unity/temp.XXXXXX20180823-5755-1c1vdfv/Unity-iPhone/Push.entitlements

//   /BUILD_PATH/lost-signal-llc.tienlen.tienlen-ios-dev/TienLen_Unity/temp.XXXXXX20180823-5755-1c1vdfv/TienLen.entitlements Unity-iPhone/Push.entitlements


// private static void OneSignalPostBuild(BuildTarget target, string buildPath)
// {
//     var capManager = new ProjectCapabilityManager(buildPath, "com.example");
//     capManager.AddPushNotifications(Debug.isDebugBuild);
// 
//     var project = capManager.PBXProject;
//     var targetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
//     project.AddFrameworkToProject(targetGuid, "UserNotifications.framework", false);
// 
//     capManager.Close();
// }
