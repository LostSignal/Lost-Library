//-----------------------------------------------------------------------
// <copyright file="CopyCloudBuildDLLSettings.cs" company="Lost Signal LLC">
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

    [AppConfigSettingsOrder(15)]
    public class CopyCloudBuildDLLSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool copyCloudBuildDLLToStreamingAssets = true;
        #pragma warning restore 0649

        public override string DisplayName => "Copy CloudBuild DLL To StreamingAssets";
        public override bool IsInline => true;

        public override void OnPreproccessBuild(AppConfig.AppConfig appConfig, BuildReport buildReport)
        {
            base.OnPreproccessBuild(appConfig, buildReport);

            if (this.copyCloudBuildDLLToStreamingAssets == false || Platform.IsUnityCloudBuild == false)
            {
                return;
            }

            foreach (var file in Directory.GetFiles(".", "*", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(file) == "UnityEditor.CloudBuild.dll")
                {
                    // Making sure the directory path exists and copying it over
                    string copyPath = "Assets/StreamingAssets/UnityEditor.CloudBuild.dll.copy";
                    Directory.CreateDirectory(Path.GetDirectoryName(copyPath));
                    File.Copy(file, copyPath);

                    // Importing the asset so it will be included in the build
                    AssetDatabase.ImportAsset(copyPath);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
