//-----------------------------------------------------------------------
// <copyright file="AppSettingsEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AppSettings))]
    public class AppSettingsEditor : Editor
    {
        private static int foldoutId = 98734656;
        private static Grid oneColumnGrid;
        private static Grid twoColumnGrid;
        private static Grid twoColumnGridWithHeader;

        public void OnEnable()
        {
            if (oneColumnGrid == null)
            {
                var gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("Define", 250);
                gridDefinition.RowButtons = GridButton.All;
                gridDefinition.DrawHeader = false;

                oneColumnGrid = new Grid(gridDefinition);
            }

            if (twoColumnGrid == null)
            {
                var gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("Define", 200);
                gridDefinition.AddColumn("Enabled", 50);
                gridDefinition.RowButtons = GridButton.None;
                gridDefinition.DrawHeader = false;

                twoColumnGrid = new Grid(gridDefinition);
            }

            if (twoColumnGridWithHeader == null)
            {
                var gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("Define", 200);
                gridDefinition.AddColumn("Enabled", 50);
                gridDefinition.RowButtons = GridButton.None;

                twoColumnGridWithHeader = new Grid(gridDefinition);
            }
        }
        
        public override void OnInspectorGUI()
        {
            var appSettings = this.target as AppSettings;

            GUILayout.Label("");

            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("App Version", GUILayout.Width(100));
                appSettings.Version = EditorGUILayout.TextField(GUIContent.none, appSettings.Version, GUILayout.Width(200));
            }
            
            GUILayout.Label("");

            this.DrawProjectSettings(foldoutId + 0, appSettings);
            this.DrawSupportedPlatforms(foldoutId + 1, appSettings);
            this.DrawLostDefines(foldoutId + 2, appSettings);
            this.DrawProjectDefines(foldoutId + 3, appSettings);
            this.DrawSourceControlSettings(foldoutId + 4, appSettings);
            this.DrawCloudBuildSettings(foldoutId + 5, appSettings);

            if (appSettings.BuildConfigs.Count > 0)
            {
                this.DrawBuildConfigs(foldoutId + 6, appSettings);
            }

            if (GUILayout.Button("Add Config"))
            {
                appSettings.BuildConfigs.Add(new AppSettings.BuildConfig());
            }

            GUILayout.Label("");

            using (new GuiBackgroundHelper(Color.red))
            {
                // only showing the update defines button if they actually need updating
                if (this.UpdateProjectDefines(appSettings, false) && GUILayout.Button("Update Defines"))
                {
                    this.UpdateProjectDefines(appSettings, true);
                }
            }
            
            // making sure we mark the data dirty if it changed
            if (GUI.changed)
            {
                EditorUtility.SetDirty(appSettings);
            }
        }
        
        private void DrawProjectSettings(int foldoutId, AppSettings appSettings)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Project Settings", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                int labelWidth = 170;

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("App Company", GUILayout.Width(labelWidth));
                    PlayerSettings.companyName = EditorGUILayout.TextField(GUIContent.none, PlayerSettings.companyName, GUILayout.Width(200));
                }

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("App Namespace", GUILayout.Width(labelWidth));
                    EditorSettings.projectGenerationRootNamespace = EditorGUILayout.TextField(GUIContent.none, EditorSettings.projectGenerationRootNamespace, GUILayout.Width(200));
                }

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("Project Line Endings", GUILayout.Width(labelWidth));
                    appSettings.ProjectLineEndings = (LineEndings)EditorGUILayout.EnumPopup(appSettings.ProjectLineEndings, GUILayout.Width(150));
                }

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("Overwrite Template Files", GUILayout.Width(labelWidth));
                    appSettings.OverrideTemplateCShardFiles = EditorGUILayout.Toggle(appSettings.OverrideTemplateCShardFiles);
                }

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("Warnings As Errors", GUILayout.Width(labelWidth));
                    bool newValue = EditorGUILayout.Toggle(appSettings.WarningsAsErrors);

                    if (newValue != appSettings.WarningsAsErrors)
                    {
                        appSettings.WarningsAsErrors = newValue;

                        if (appSettings.WarningsAsErrors)
                        {
                            AppSettingsHelper.GenerateWarngingsAsErrorsFile();
                        }
                        else
                        {
                            AppSettingsHelper.RemoveWarngingsAsErrorsFile();
                        }
                    }
                }
            }
        }

        private void DrawSupportedPlatforms(int foldoutId, AppSettings appSettings)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Supported Platforms", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                using (new BeginGridHelper(twoColumnGrid))
                {
                    foreach (var supportedPlatform in Enum.GetValues(typeof(DevicePlatform)).Cast<DevicePlatform>())
                    {
                        using (new BeginGridRowHelper(twoColumnGrid))
                        {
                            twoColumnGrid.DrawLabel(supportedPlatform.ToString());

                            bool isOnBefore = (appSettings.SupoortedPlatforms & supportedPlatform) != 0;
                            bool isOnAfter = twoColumnGrid.DrawBool(isOnBefore);

                            if (isOnBefore != isOnAfter)
                            {
                                appSettings.SupoortedPlatforms = isOnAfter ? (appSettings.SupoortedPlatforms | supportedPlatform) : (appSettings.SupoortedPlatforms & ~supportedPlatform);
                            }
                        }
                    }
                }
            }
        }

        private void DrawLostDefines(int foldoutId, AppSettings appSettings)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Lost Defines", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                using (new BeginGridHelper(twoColumnGrid))
                {
                    foreach (var define in appSettings.LostDefines)
                    {
                        using (new BeginGridRowHelper(twoColumnGrid))
                        {
                            twoColumnGrid.DrawLabel(define.Name);
                            define.Enabled = twoColumnGrid.DrawBool(define.Enabled);
                        }
                    }
                }
            }
        }

        private void DrawProjectDefines(int foldoutId, AppSettings appSettings)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Project Defines", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                using (new BeginGridHelper(oneColumnGrid))
                {
                    int index = 0;
                    foreach (var define in appSettings.ProjectDefines)
                    {
                        using (new BeginGridRowHelper(oneColumnGrid))
                        {
                            appSettings.ProjectDefines[index] = oneColumnGrid.DrawString(define);
                        }

                        if (oneColumnGrid.RowButtonPressed == GridButton.Delete)
                        {
                            appSettings.ProjectDefines.Remove(define);
                            break;
                        }
                        else if (oneColumnGrid.RowButtonPressed == GridButton.MoveUp && index != 0)
                        {
                            appSettings.ProjectDefines.Remove(define);
                            appSettings.ProjectDefines.Insert(index - 1, define);
                            break;
                        }
                        else if (oneColumnGrid.RowButtonPressed == GridButton.MoveDown && index != appSettings.ProjectDefines.Count - 1)
                        {
                            appSettings.ProjectDefines.Remove(define);
                            appSettings.ProjectDefines.Insert(index + 1, define);
                            break;
                        }

                        index++;
                    }
                }

                if (oneColumnGrid.DrawAddButton())
                {
                    appSettings.ProjectDefines.Add(string.Empty);
                }
            }
        }

        private void DrawSourceControlSettings(int foldoutId, AppSettings appSettings)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Source Control", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                int labelWidth = 215;

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("Source Control", GUILayout.Width(labelWidth));
                    appSettings.SourceControl = (SourceControlType)EditorGUILayout.EnumPopup(appSettings.SourceControl, GUILayout.Width(150));
                }

                if (appSettings.SourceControl == SourceControlType.Perforce)
                {
                    this.DrawPerforceSettings(labelWidth, appSettings);
                }
            }
        }

        private void DrawPerforceSettings(int labelWidth, AppSettings appSettings)
        {
            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("Use P4IGNORE File", GUILayout.Width(labelWidth));
                appSettings.UseP4IgnoreFile = EditorGUILayout.Toggle(appSettings.UseP4IgnoreFile);
            }
            
            if (appSettings.UseP4IgnoreFile == false)
            {
                return;
            }
            
            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("Set P4IGNORE Variable At Startup", GUILayout.Width(labelWidth));
                appSettings.SetP4IgnoreVariableAtStartup = EditorGUILayout.Toggle(appSettings.SetP4IgnoreVariableAtStartup);
            }
            
            if (appSettings.SetP4IgnoreVariableAtStartup)
            {
                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("P4IGNORE File Name", GUILayout.Width(labelWidth));
                    appSettings.P4IgnoreFileName = EditorGUILayout.TextField(appSettings.P4IgnoreFileName, GUILayout.Width(150));
                }
            }

            GUILayout.Label("");

            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("", GUILayout.Width(1));

                string generateButtonText = File.Exists(appSettings.P4IgnoreFileName) ? "Overwrite P4IGNORE File" : "Generate P4IGNORE File";

                if (GUILayout.Button(generateButtonText, GUILayout.Width(180)))
                {
                    AppSettingsHelper.CreateOrOverwriteP4IgnoreFile();
                }

                GUILayout.Label("", GUILayout.Width(1));

                string p4ignoreFile = AppSettingsHelper.GetCurrentP4IgnoreVariable();

                if (appSettings.P4IgnoreFileName != p4ignoreFile)
                {
                    if (GUILayout.Button("Set P4IGNORE Variable", GUILayout.Width(180)))
                    {
                        AppSettingsHelper.SetP4IgnoreFileVariable(appSettings.P4IgnoreFileName);
                    }
                }
            }
            
            GUILayout.Label("");
        }

        private void DrawCloudBuildSettings(int foldoutId, AppSettings appSettings)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Unity Cloud Build Settings", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("", GUILayout.Width(20));
                    appSettings.BuildAssetBundles = EditorGUILayout.Toggle(GUIContent.none, appSettings.BuildAssetBundles, GUILayout.Width(25));
                    GUILayout.Label("Build Asset Bundles?");
                }

                if (appSettings.BuildAssetBundles == false)
                {
                    return;
                }

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("", GUILayout.Width(20));
                    appSettings.CopyToStreamingAssets = EditorGUILayout.Toggle(GUIContent.none, appSettings.CopyToStreamingAssets, GUILayout.Width(25));
                    GUILayout.Label("Copy Asset Bundles to StreamingAssets?");
                }

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("", GUILayout.Width(20));
                    appSettings.UploadAssetBundles = EditorGUILayout.Toggle(GUIContent.none, appSettings.UploadAssetBundles, GUILayout.Width(25));
                    GUILayout.Label("Upload Asset Bundles?");
                }

                if (appSettings.UploadAssetBundles == false)
                {
                    return;
                }

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("", GUILayout.Width(20));
                    appSettings.UploadType = (AssetBundleUploadType)EditorGUILayout.EnumPopup(appSettings.UploadType, GUILayout.Width(70));
                    GUILayout.Label(" Upload Type", GUILayout.Width(85));
                }
            }
        }
        
        private void DrawBuildConfigs(int foldoutId, AppSettings appSettings)
        {
            GUILayout.Label("");
            GUILayout.Label("Build Configs");

            this.ValidateBuildConfigs(appSettings);

            int index = 0;
            foreach (var config in appSettings.BuildConfigs)
            {
                bool isVisible = false;

                using (new FoldoutHelper(foldoutId + index, config.Name + (config.IsActive ? " (Active)" : string.Empty), out isVisible))
                {
                    if (isVisible == false)
                    {
                        index++;
                        continue;
                    }

                    GUILayout.Label("");

                    // name and button group
                    using (new BeginHorizontalHelper())
                    {
                        GUILayout.Label("Name", GUILayout.Width(60));
                        config.Name = EditorGUILayout.TextField(GUIContent.none, config.Name, GUILayout.Width(100));

                        GUILayout.Label("", GUILayout.Width(20));

                        if (GUILayout.Button("Delete", GUILayout.Width(80)))
                        {
                            appSettings.BuildConfigs.Remove(config);
                            break;
                        }

                        if (GUILayout.Button("Move Up", GUILayout.Width(80)) && index != 0)
                        {
                            appSettings.BuildConfigs.Remove(config);
                            appSettings.BuildConfigs.Insert(index - 1, config);
                            break;
                        }

                        if (GUILayout.Button("Move Down", GUILayout.Width(80)) && index != appSettings.BuildConfigs.Count - 1)
                        {
                            appSettings.BuildConfigs.Remove(config);
                            appSettings.BuildConfigs.Insert(index + 1, config);
                            break;
                        }

                        if (config.IsActive == false && GUILayout.Button("Set Active", GUILayout.Width(80)))
                        {
                            appSettings.BuildConfigs.ForEach(x => x.IsActive = false);
                            config.IsActive = true;
                        }
                    }

                    this.DrawBuildConfig(config);
                }
                
                index++;
            }
        }

        private void DrawBuildConfig(AppSettings.BuildConfig config)
        {
            #if USE_PLAYFAB_SDK || USE_PLAYFAB_ANDROID_SDK
            GUILayout.Label("");
            #endif

            #if USE_PLAYFAB_SDK
            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("PlayFab Title Id", GUILayout.Width(120));
                config.PlayfabTitleId = EditorGUILayout.TextField(GUIContent.none, config.PlayfabTitleId, GUILayout.Width(250));
            }
            
            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("PlayFab Secret Id", GUILayout.Width(120));
                config.PlayfabSecretId = EditorGUILayout.TextField(GUIContent.none, config.PlayfabSecretId, GUILayout.Width(250));
            }
            #endif
            
            #if USE_PLAYFAB_ANDROID_SDK
            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("Google App Id", GUILayout.Width(120));
                config.GoogleAppId = EditorGUILayout.TextField(GUIContent.none, config.GoogleAppId, GUILayout.Width(250));
            }
            #endif

            GUILayout.Label("");
            GUILayout.Label("Unity Cloud Build Settings");

            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("", GUILayout.Width(10));
                config.AppendCommitToVersion = EditorGUILayout.Toggle(GUIContent.none, config.AppendCommitToVersion, GUILayout.Width(25));
                GUILayout.Label("Append Commit Number To App Version");
            }

            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("", GUILayout.Width(10));
                config.DisableBitCode = EditorGUILayout.Toggle(GUIContent.none, config.DisableBitCode, GUILayout.Width(25));
                GUILayout.Label("Disable BitCode (iOS Only)");
            }
            
            GUILayout.Label("");

            // drawing defines
            using (new BeginGridHelper(twoColumnGridWithHeader))
            {
                foreach (var define in config.Defines)
                {
                    using (new BeginGridRowHelper(twoColumnGridWithHeader))
                    {
                        twoColumnGridWithHeader.DrawLabel(define.Name);
                        define.Enabled = twoColumnGridWithHeader.DrawBool(define.Enabled);
                    }
                }
            }
        }
        
        private void ValidateBuildConfigs(AppSettings appSettings)
        {
            // making sure a config is set active
            if (appSettings.BuildConfigs.Count > 0 && appSettings.BuildConfigs.All(x => x.IsActive == false))
            {
                appSettings.BuildConfigs.FirstOrDefault().IsActive = true;
            }

            // making sure config defines are set correctly
            foreach (var config in appSettings.BuildConfigs)
            {
                // adding all missing defines
                foreach (var projectDefine in appSettings.ProjectDefines)
                {
                    var define = config.Defines.FirstOrDefault(x => x.Name == projectDefine);
                    if (define == null)
                    {
                        config.Defines.Add(new AppSettings.Define(projectDefine));
                    }
                }

                // remove all non-existent defines
                if (config.Defines.Count != appSettings.ProjectDefines.Count)
                {
                    var definesToRemove = new List<AppSettings.Define>();

                    foreach (var define in config.Defines)
                    {
                        if (appSettings.ProjectDefines.Contains(define.Name) == false)
                        {
                            definesToRemove.Add(define);
                        }
                    }

                    foreach (var defineToRemove in definesToRemove)
                    {
                        config.Defines.Remove(defineToRemove);
                    }
                }
            }
        }

        private bool UpdateProjectDefines(AppSettings appSettings, bool performUpdate)
        {
            List<AppSettings.Define> defines = new List<AppSettings.Define>();
            defines.AddRange(appSettings.LostDefines);

            // adding all the defines that belong to the active build config
            if (appSettings.BuildConfigs.Count > 0)
            {
                var activeConfig = appSettings.BuildConfigs.FirstOrDefault(x => x.IsActive);

                if (activeConfig != null)
                {
                    defines.AddRange(activeConfig.Defines);
                }
            }

            foreach (var supportedPlatform in Enum.GetValues(typeof(DevicePlatform)).Cast<DevicePlatform>())
            {
                if ((appSettings.SupoortedPlatforms & supportedPlatform) == 0)
                {
                    continue;
                }

                var buildTargetGroup = this.GetBuildTargetGroup(supportedPlatform);
                var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').ToList();
                var needsUpdating = false;

                foreach (var define in defines)
                {
                    if (define.Enabled && currentDefines.Contains(define.Name) == false)
                    {
                        needsUpdating = true;
                        currentDefines.Add(define.Name);
                    }
                    else if (define.Enabled == false && currentDefines.Contains(define.Name))
                    {
                        needsUpdating = true;
                        currentDefines.Remove(define.Name);
                    }
                }

                // early out if this function is only suppose to return if updates are needed
                if (needsUpdating && performUpdate == false)
                {
                    return true;
                }

                if (needsUpdating)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", currentDefines.OrderBy(x => x).ToArray()));
                }
            }

            return false;
        }
        
        private BuildTargetGroup GetBuildTargetGroup(DevicePlatform devicePlatform)
        {
            switch (devicePlatform)
            {
                case DevicePlatform.Windows:
                case DevicePlatform.Mac:
                case DevicePlatform.Linux:
                    return BuildTargetGroup.Standalone;

                case DevicePlatform.Android:
                    return BuildTargetGroup.Android;

                case DevicePlatform.iOS:
                    return BuildTargetGroup.iOS;

                case DevicePlatform.WebGL:
                    return BuildTargetGroup.WebGL;

                case DevicePlatform.WindowsUniversal:
                    return BuildTargetGroup.WSA;

                case DevicePlatform.XboxOne:
                    return BuildTargetGroup.XboxOne;

                default:
                    Debug.LogErrorFormat("Found Unknown DevicePlatform \"{0}\" to update defines for.", devicePlatform);
                    return BuildTargetGroup.Unknown;
            }
        }
    }
}
