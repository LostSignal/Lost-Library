//-----------------------------------------------------------------------
// <copyright file="AppConfigCustomEditor.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AppConfig))]
    public class AppConfigCustomEditor : Editor
    {
        private static List<Type> BuildSettingsTypes;

        private static EditorGrid definesGrid;
        private static EditorGridDefinition definesGridDefinition;

        static AppConfigCustomEditor()
        {
            BuildSettingsTypes = new List<Type>();

            foreach (var t in TypeUtil.GetAllTypesOf<AppConfigSettings>())
            {
                BuildSettingsTypes.Add(t);
            }

            BuildSettingsTypes = BuildSettingsTypes.OrderBy((t) =>
            {
                var attribute = t.GetCustomAttributes(typeof(AppConfigSettingsOrderAttribute), true).FirstOrDefault() as AppConfigSettingsOrderAttribute;
                return attribute == null ? 1000 : attribute.Order;
            }).ToList();
        }

        public override void OnInspectorGUI()
        {
            float currentViewWidth = EditorGUIUtility.currentViewWidth - 20;
            var appConfig = this.target as AppConfig;

            bool appConfigPropertiesVisible = false;
            using (new FoldoutHelper(5545418, "App Config Properties", out appConfigPropertiesVisible))
            {
                if (appConfigPropertiesVisible)
                {
                    appConfig.Parent = (AppConfig)EditorGUILayout.ObjectField("Parent", appConfig.Parent, typeof(AppConfig), false);
                    appConfig.SupportedPlatform = (DevicePlatform)EditorGUILayout.EnumPopup("Supported Platforms", appConfig.SupportedPlatform);

                    this.DrawDefines(appConfig, currentViewWidth);
                }
            }

            bool appConfigSettingsVisible = false;
            using (new FoldoutHelper(5545416, "App Config Settings", out appConfigSettingsVisible))
            {
                if (appConfigSettingsVisible)
                {
                    this.DrawAppConfigSettings(appConfig, currentViewWidth);
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this.target);
            }
        }

        private void DrawAppConfigSettings(AppConfig appConfig, float currentViewWidth)
        {
            appConfig.ShowInherited = EditorGUILayout.Toggle("Show Inherited", appConfig.ShowInherited);

            List<Type> settingsToAdd = new List<Type>();

            foreach (var settingsType in BuildSettingsTypes)
            {
                bool isInherited = false;

                AppConfigSettings settings = appConfig.GetSettings(settingsType, out isInherited);

                if (settings == null)
                {
                    settingsToAdd.Add(settingsType);
                }
                else
                {
                    // Checking if we should skip this
                    if (isInherited && appConfig.ShowInherited == false)
                    {
                        continue;
                    }

                    bool didDeleteSettings = false;
                    this.DrawAppConfigSettings(appConfig, settings, isInherited, out didDeleteSettings);

                    if (didDeleteSettings)
                    {
                        break;
                    }
                }
            }

            int buttonWidth = (int)(currentViewWidth / 2.0f);

            for (int i = 0; i < settingsToAdd.Count; i += 2)
            {
                using (new BeginHorizontalHelper())
                {
                    for (int j = 0; j < 2; j++)
                    {
                        int index = i + j;

                        if (index >= settingsToAdd.Count)
                        {
                            break;
                        }

                        var settingsType = settingsToAdd[index];

                        string buttonName = settingsType.Name;

                        if (buttonName.EndsWith("Settings"))
                        {
                            buttonName = buttonName.Replace("Settings", string.Empty);
                        }

                        if (GUILayout.Button(buttonName, GUILayout.Width(buttonWidth)))
                        {
                            var newSettings = ScriptableObject.CreateInstance(settingsType) as AppConfigSettings;
                            newSettings.name = newSettings.DisplayName;

                            appConfig.Settings.Add(newSettings);

                            string path = appConfig.GetPath();
                            AssetDatabase.AddObjectToAsset(newSettings, path);
                            AssetDatabase.ImportAsset(path);
                        }
                    }
                }
            }
        }

        private void DrawDefines(AppConfig appConfig, float width)
        {
            definesGridDefinition[0].Width = (int)(width - 90.0f);

            bool definesVisible = false;
            using (new FoldoutHelper(564185451, "Defines", out definesVisible, true))
            {
                if (definesVisible == false)
                {
                    return;
                }

                using (new BeginGridHelper(definesGrid))
                {
                    for (int i = 0; i < appConfig.Defines.Count; i++)
                    {
                        using (new BeginGridRowHelper(definesGrid))
                        {
                            appConfig.Defines[i] = definesGrid.DrawString(appConfig.Defines[i]);
                        }

                        EditorGrid.UpdateList<string>(definesGrid.RowButtonPressed, appConfig.Defines, i);
                    }

                    if (definesGrid.DrawAddButton())
                    {
                        appConfig.Defines.Add(string.Empty);
                    }
                }
            }
        }

        private void DrawAppConfigSettings(AppConfig appConfig, AppConfigSettings settings, bool isInherited, out bool didDeleteSettings)
        {
            didDeleteSettings = false;

            float currentViewWidth = EditorGUIUtility.currentViewWidth - 20;
            bool foldoutVisible = true;
            Rect boxRect;

            var verticleHelper = settings.IsInline ?
                (IDisposable)new BeginVerticalHelper(out boxRect, "box") :
                (IDisposable)new FoldoutHelper(settings.DisplayName.GetHashCode(), settings.DisplayName, out foldoutVisible, out boxRect, false);

            using (verticleHelper)
            {
                // Drawing the button
                float buttonSize = 14;
                float rightPadding = 2;
                float topPadding = 1;

                Rect buttonRect = new Rect(new Vector2(currentViewWidth - rightPadding, boxRect.y + topPadding), new Vector2(buttonSize, buttonSize));

                if (isInherited)
                {
                    if (ButtonUtil.DrawAddButton(buttonRect))
                    {
                        var newSettings = UnityEngine.Object.Instantiate(settings) as AppConfigSettings;
                        appConfig.Settings.Add(newSettings);

                        string path = appConfig.GetPath();
                        AssetDatabase.AddObjectToAsset(newSettings, path);
                        AssetDatabase.ImportAsset(path);
                    }
                }
                else
                {
                    if (ButtonUtil.DrawDeleteButton(buttonRect))
                    {
                        appConfig.Settings.Remove(settings);
                        GameObject.DestroyImmediate(settings, true);
                        AssetDatabase.ImportAsset(appConfig.GetPath());
                        didDeleteSettings = true;
                        return;
                    }
                }

                // Breaking out if we're not suppose to show the foldout
                if (foldoutVisible == false)
                {
                    return;
                }

                // Iterating and displaying all properties
                using (new BeginDisabledGroupHelper(isInherited))
                {
                    float width = currentViewWidth - (settings.IsInline ? 24 : 6);
                    settings.DrawSettings(settings, width);
                }
            }
        }

        private void OnEnable()
        {
            if (definesGrid == null)
            {
                definesGridDefinition = new EditorGridDefinition();
                definesGridDefinition.AddColumn("Define", 200);
                definesGridDefinition.RowButtons = GridButton.All;
                definesGridDefinition.DrawHeader = false;

                definesGrid = new EditorGrid(definesGridDefinition);
            }
        }
    }
}
