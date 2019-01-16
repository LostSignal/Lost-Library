//-----------------------------------------------------------------------
// <copyright file="CloudBuildTest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    public class CloudBuildTestSettings : AppConfigSettings
    {
        public override string DisplayName => "Cloud Build Test";
        public override bool IsInline => false;

        public override void InitializeOnLoad(AppConfig.AppConfig appConfig)
        {
            Debug.Log("AppConfigSettings.InitializeOnLoad");

            var buildJsonPath = FindJson(Directory.GetCurrentDirectory(), "build.json");

            if (buildJsonPath != null)
            {
                Debug.Log("build.json");
                Debug.Log(File.ReadAllText(buildJsonPath));
            }

            var buildManifestJsonPath = FindJson(Directory.GetCurrentDirectory(), "build_manifest.json");

            if (buildManifestJsonPath != null)
            {
                Debug.Log("build_manifest.json");
                Debug.Log(File.ReadAllText(buildManifestJsonPath));
            }
        }

        public override void OnUnityCloudBuildInitiated(AppConfig.AppConfig appConfig)
        {
            Debug.Log("AppConfigSettings.OnUnityCloudBuildInitiated");
        }

        public override void OnUserBuildInitiated(AppConfig.AppConfig appConfig)
        {
            Debug.Log("AppConfigSettings.OnUserBuildInitiated");
        }

        public override void OnPreproccessBuild(AppConfig.AppConfig appConfig, BuildReport buildReport)
        {
            Debug.Log("AppConfigSettings.OnPreproccessBuild");

            // Disabling the code that copys the CloudBuild editor dll into StreamingAssets
            // string dllPath = "Assets/__UnityCloud__/Scripts/Editor/UnityEditor.CloudBuild.dll";
            // string newPath = "Assets/StreamingAssets/CloudBuildDll.txt";
            //
            // if (File.Exists(dllPath))
            // {
            //     Debug.Log("Found UnityEditor.CloudBuild.dll");
            //     Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            //     File.Copy(dllPath, newPath);
            //     AssetDatabase.ImportAsset(newPath);
            //     AssetDatabase.Refresh();
            // }
        }

        public override void OnPostprocessBuild(AppConfig.AppConfig appConfig, BuildReport buildReport)
        {
            Debug.Log("AppConfigSettings.OnPostprocessBuild");
        }

        public override BuildPlayerOptions ChangeBuildPlayerOptions(AppConfig.AppConfig appConfig, BuildPlayerOptions options)
        {
            Debug.Log("AppConfigSettings.ChangeBuildPlayerOptions");
            return options;
        }

        private static string FindJson(string directory, string filename)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return null;
            }

            string filePath = Path.Combine(directory, filename);

            if (File.Exists(filePath))
            {
                return filePath;
            }
            else
            {
                return FindJson(Path.GetDirectoryName(directory), filename);
            }
        }
    }
}
