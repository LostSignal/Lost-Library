//-----------------------------------------------------------------------
// <copyright file="AppSettingsEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AppSettings))]
    public class AppSettingsEditor : Editor
    {
        private static Grid lostDefinesGrid;
        private static Grid customDefinesGrid;
        
        public void OnEnable()
        {
            if (lostDefinesGrid == null)
            {
                var gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("Lost Define", 200);
                gridDefinition.AddColumn("Enabled", 50);

                lostDefinesGrid = new Grid(gridDefinition);
            }
            
            if (customDefinesGrid == null)
            {
                var gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("Custom Define", 200);
                gridDefinition.AddColumn("Enabled", 50);
                gridDefinition.RowButtons = GridButton.All;
            
                customDefinesGrid = new Grid(gridDefinition);
            }
        }

        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector();

            GUILayout.Label("");
            GUILayout.Label("");

            var appSettings = this.target as AppSettings;
            
            using (new BeginGridHelper(lostDefinesGrid))
            {
                foreach (var define in appSettings.LostDefines)
                {
                    using (new BeginGridRowHelper(lostDefinesGrid))
                    {
                        lostDefinesGrid.DrawReadOnlyString(define.Name);
                        define.Enabled = lostDefinesGrid.DrawBool(define.Enabled);
                    }
                }
            }

            GUILayout.Label("");
            GUILayout.Label("");
            
            using (new BeginGridHelper(customDefinesGrid))
            {
                int index = 0;
                foreach (var define in appSettings.CustomDefines)
                {
                    using (new BeginGridRowHelper(customDefinesGrid))
                    {
                        define.Name = customDefinesGrid.DrawString(define.Name);
                        define.Enabled = customDefinesGrid.DrawBool(define.Enabled);
                    }
                    
                    if (customDefinesGrid.RowButtonPressed == GridButton.Delete)
                    {
                        appSettings.CustomDefines.Remove(define);
                        break;
                    }
                    else if (customDefinesGrid.RowButtonPressed == GridButton.MoveUp && index != 0)
                    {
                        appSettings.CustomDefines.Remove(define);
                        appSettings.CustomDefines.Insert(index - 1, define);
                        break;
                    }
                    else if (customDefinesGrid.RowButtonPressed == GridButton.MoveDown && index != appSettings.CustomDefines.Count - 1)
                    {
                        appSettings.CustomDefines.Remove(define);
                        appSettings.CustomDefines.Insert(index + 1, define);
                        break;
                    }

                    index++;
                }
            }
            
            if (customDefinesGrid.DrawAddButton())
            {
                appSettings.CustomDefines.Add(new AppSettings.Define(string.Empty));
            }

            GUILayout.Label("");
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

        private bool UpdateProjectDefines(AppSettings appSettings, bool performUpdate)
        {
            List<AppSettings.Define> defines = new List<AppSettings.Define>();
            defines.AddRange(appSettings.LostDefines);
            defines.AddRange(appSettings.CustomDefines);

            foreach (var supportedPlatform in appSettings.SupoortedPlatforms)
            {
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
